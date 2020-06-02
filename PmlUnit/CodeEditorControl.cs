// Copyright (c) 2020 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PmlUnit
{
    partial class CodeEditorControl : UserControl
    {
        private string InitialFilter;

        public CodeEditorControl()
        {
            InitializeComponent();

            InitialFilter = FileDialog.Filter;

            var atom = new EditorItem(CodeEditorKind.Atom, "Atom", "atom.exe");
            atom.SearchPaths.AddPath(LocalAppDataPath, "atom");
            EditorKindComboBox.Items.Add(atom);

            var notepad = new EditorItem(CodeEditorKind.NotepadPlusPlus, "Notepad++", "notepad++.exe");
            notepad.SearchPaths.AddPath(ProgramFilesPath, "Notepad++");
            notepad.SearchPaths.AddPath(ProgramFiles32BitPath, "Notepad++");
            EditorKindComboBox.Items.Add(notepad);

            var sublime = new EditorItem(CodeEditorKind.SublimeText, "Sublime Text", "subl.exe");
            sublime.SearchPaths.AddPath(ProgramFilesPath, "Sublime Text 3");
            sublime.SearchPaths.AddPath(ProgramFiles32BitPath, "Sublime Text 3");
            EditorKindComboBox.Items.Add(sublime);

            var ultradEdit = new EditorItem(CodeEditorKind.UltraEdit, "UltraEdit", "uedit64.exe");
            ultradEdit.FileNames.Add("uedit32.exe");
            ultradEdit.SearchPaths.AddPath(ProgramFilesPath, "IDM Computer Soltions", "UltraEdit");
            ultradEdit.SearchPaths.AddPath(ProgramFiles32BitPath, "IDM Computer Soltions", "UltraEdit");
            ultradEdit.SearchPaths.AddPath(DocumentsPath, "IDM Computer Soltions", "UltraEdit");
            EditorKindComboBox.Items.Add(ultradEdit);

            var vsCode = new EditorItem(CodeEditorKind.VisualStudioCode, "Visual Studio Code", "Code.exe");
            vsCode.SearchPaths.AddPath(LocalAppDataPath, "Programs", "Microsoft VS Code");
            vsCode.SearchPaths.AddPath(ProgramFilesPath, "Microsoft VS Code");
            vsCode.SearchPaths.AddPath(ProgramFiles32BitPath, "Microsoft VS Code");
            EditorKindComboBox.Items.Add(vsCode);

            var other = new EditorItem(CodeEditorKind.Other, "Other");
            other.SearchPaths.Add(ProgramFilesPath);
            EditorKindComboBox.Items.Add(other);

            EditorKindComboBox.SelectedItem = other;
            foreach (EditorItem item in EditorKindComboBox.Items)
            {
                if (!string.IsNullOrEmpty(item.TryFindFile()))
                {
                    EditorKindComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private static string ProgramFilesPath
        {
            get
            {
                // PDMS and E3D are 32 bit processes (even on 64 bit Windows) and so
                // Environment.SpecialFolder.ProgramFiles and the ProgramFiles
                // environment variable will always return the location of the 32 bit
                // program files folder. However, if we are on a 64 bit Windows, there
                // should be a ProgramW6432 environment variable that points to
                // the 64 bit program files folder.
                // see https://weblog.west-wind.com/posts/2019/Feb/05/Finding-the-ProgramFiles64-Folder-in-a-32-Bit-App#the-workaround---using-environment-var
                string result = Environment.GetEnvironmentVariable("ProgramW6432");
                if (string.IsNullOrEmpty(result))
                    result = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                return result;
            }
        }

        private static string ProgramFiles32BitPath
        {
            get
            {
                string result = "";
#if E3D
                if (string.IsNullOrEmpty(result))
                    result = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
#endif
                if (string.IsNullOrEmpty(result))
                    result = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                if (string.IsNullOrEmpty(result))
                    result = Environment.GetEnvironmentVariable("ProgramFiles");
                return result;
            }
        }

        private static string DocumentsPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
        }

        private static string LocalAppDataPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); }
        }

        public CodeEditorDescriptor Descriptor
        {
            get
            {
                var item = EditorKindComboBox.SelectedItem as EditorItem;
                if (item == null)
                    return null;

                return new CodeEditorDescriptor(item.Kind, PathComboBox.Text, ArgumentsTextBox.Text);
            }
        }

        private void OnEditorKindChanged(object sender, EventArgs e)
        {
            var item = EditorKindComboBox.SelectedItem as EditorItem;
            if (item != null)
            {
                if (item.Kind == CodeEditorKind.Other)
                {
                    ArgumentsLabel.Text = "Arguments:";
                    ArgumentsHintLabel.Visible = true;
                    FileDialog.Title = "Find executable";
                }
                else
                {
                    ArgumentsLabel.Text = "Extra Arguments:";
                    ArgumentsHintLabel.Visible = false;
                    FileDialog.Title = "Find " + item.Name;
                }

                string path = item.TryFindFile();
                if (string.IsNullOrEmpty(path))
                {
                    FileDialog.FileName = item.DefaultFileName;
                    if (string.IsNullOrEmpty(item.DefaultDirectory) || !Directory.Exists(item.DefaultDirectory))
                        FileDialog.InitialDirectory = ProgramFilesPath;
                    else
                        FileDialog.InitialDirectory = item.DefaultDirectory;
                    PathComboBox.Text = "";
                }
                else
                {
                    FileDialog.FileName = Path.GetFileName(path);
                    FileDialog.InitialDirectory = Path.GetDirectoryName(path);
                    PathComboBox.Text = path;
                }

                if (string.IsNullOrEmpty(item.FileNameFilter))
                    FileDialog.Filter = InitialFilter;
                else
                    FileDialog.Filter = string.Format("{0}|{1}|{2}", item.Name, item.FileNameFilter, InitialFilter);
            }
        }

        private void OnBrowseButtonClick(object sender, EventArgs e)
        {
            if (FileDialog.ShowDialog(this) == DialogResult.OK)
            {
                PathComboBox.Text = FileDialog.FileName;
            }
        }

        public override bool ValidateChildren()
        {
            // We are not using the Validating events here because they don't allow us
            // to validate one child control at a time. The base ValidateChildren() method
            // would validate all child controls (even if the first control fails validation)
            // which is very annyoing when we display message boxes during validation.
            //
            // Also, we are changing the focus in the validation methods which
            // *MUST NOT* happen with event driven validation.
            return ValidateEditorKind()
                && ValidatePath()
                && ValidateArguments();
        }

        private bool ValidateEditorKind()
        {
            var item = EditorKindComboBox.SelectedItem as EditorItem;
            if (item == null)
            {
                MessageBox.Show(
                    this, "Select an editor.", ParentForm.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning
                );
                EditorKindComboBox.Focus();
                return false;
            }

            return true;
        }

        private bool ValidatePath()
        {
            string path = PathComboBox.Text;
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show(
                    this, "Specify a path to the editor executable.", ParentForm.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning
                );
                PathComboBox.Focus();
                return false;
            }
            else if (!File.Exists(path))
            {
                var result = MessageBox.Show(
                    this, path + "\nEditor does not exist. Do you want to use it anyway?", ParentForm.Text,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );
                if (result != DialogResult.Yes)
                {
                    PathComboBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool ValidateArguments()
        {
            var item = EditorKindComboBox.SelectedItem as EditorItem;
            if (item != null && item.Kind == CodeEditorKind.Other && !ArgumentsTextBox.Text.Contains("$fileName"))
            {
                MessageBox.Show(
                    this, "Arguments must contain $fileName.", ParentForm.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning
                );
                ArgumentsTextBox.Focus();
                return false;
            }

            return true;
        }

        private class EditorItem
        {
            public CodeEditorKind Kind { get; }
            public string Name { get; }
            public SearchStringCollection FileNames { get; }
            public SearchStringCollection SearchPaths { get; }

            public EditorItem(CodeEditorKind kind, string name)
                : this(kind, name, null)
            {
            }

            public EditorItem(CodeEditorKind kind, string name, string defaultFileName)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException(nameof(name));

                Kind = kind;
                Name = name;
                FileNames = new SearchStringCollection();
                SearchPaths = new SearchStringCollection();
                if (!string.IsNullOrEmpty(defaultFileName))
                    FileNames.Add(defaultFileName);
            }

            public string DefaultFileName
            {
                get { return FileNames.Count > 0 ? FileNames[0] : ""; }
            }

            public string DefaultDirectory
            {
                get { return SearchPaths.Count > 0 ? SearchPaths[0] : ""; }
            }

            public string FileNameFilter
            {
                get { return string.Join(";", FileNames.ToArray()); }
            }

            public string TryFindFile()
            {
                foreach (string fileName in FileNames)
                {
                    foreach (string directory in SearchPaths)
                    {
                        string path = Path.Combine(directory, fileName);
                        if (File.Exists(path))
                            return path;
                    }
                }
                return "";
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private class SearchStringCollection : ICollection<string>
        {
            private readonly List<string> Paths;

            public SearchStringCollection()
            {
                Paths = new List<string>();
            }

            public int Count => Paths.Count;

            bool ICollection<string>.IsReadOnly => false;

            public string this[int index]
            {
                get { return Paths[index]; }
                set
                {
                    if (string.IsNullOrEmpty(value))
                        throw new ArgumentNullException(nameof(value));
                    Paths[index] = value;
                }
            }

            /// <summary>
            /// Combines the specified path and the additional segments to one path
            /// and adds it to the collection.
            /// </summary>
            /// <remarks>
            /// This method exists because .NET 3.5 does not have a Path.Combine()
            /// method that accepts more than two paths.
            /// </remarks>
            public void AddPath(string path, params string[] segments)
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException(nameof(path));

                foreach (var segment in segments)
                {
                    if (string.IsNullOrEmpty(segment))
                        throw new ArgumentException("Additional path segments must not be null or empty.", nameof(segments));
                    path = Path.Combine(path, segment);
                }

                Add(path);
            }

            public void Add(string item)
            {
                if (string.IsNullOrEmpty(item))
                    throw new ArgumentNullException(nameof(item));
                Paths.Add(item);
            }

            public void Clear()
            {
                Paths.Clear();
            }

            public bool Contains(string item)
            {
                return Paths.Contains(item);
            }

            public void CopyTo(string[] array, int arrayIndex)
            {
                Paths.CopyTo(array, arrayIndex);
            }

            public IEnumerator<string> GetEnumerator()
            {
                return Paths.GetEnumerator();
            }

            public bool Remove(string item)
            {
                return Paths.Remove(item);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
