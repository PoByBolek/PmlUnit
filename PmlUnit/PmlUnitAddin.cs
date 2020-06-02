// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Drawing;
using System.Drawing.Text;
using Aveva.ApplicationFramework;

#if PDMS || E3D_11
using IAddin = Aveva.ApplicationFramework.IAddin;
using ICommandManager = Aveva.ApplicationFramework.Presentation.CommandManager;
using IWindowManager = Aveva.ApplicationFramework.Presentation.WindowManager;
#else
using IAddin = Aveva.ApplicationFramework.IAddinInjected;
using ICommandManager = Aveva.ApplicationFramework.Presentation.ICommandManager;
using IWindowManager = Aveva.ApplicationFramework.Presentation.IWindowManager;
#endif

namespace PmlUnit
{
    public class PmlUnitAddin : IAddin, IDisposable
    {
        private readonly AsyncTestRunner TestRunner;
        private readonly TestCaseProvider TestCaseProvider;
        private readonly TestRunnerControl TestRunnerControl;
        private readonly AboutDialog AboutDialog;

        public PmlUnitAddin()
        {
            Font font = null;
            try
            {
                font = GetDefaultFont();

                var index = new FileIndex();
                TestRunner = new PmlTestRunner(
                    new ControlMethodInvoker(() => TestRunnerControl),
                    new FileIndexEntryPointResolver(index)
                );
                TestCaseProvider = new FileIndexTestCaseProvider(index);
                TestRunnerControl = new TestRunnerControl(TestCaseProvider, TestRunner, new RegistrySettingsProvider());
                TestRunnerControl.Font = font;
                AboutDialog = new AboutDialog();
                AboutDialog.Font = font;
            }
            catch
            {
                if (font != null)
                    font.Dispose();
                if (TestRunner != null)
                    TestRunner.Dispose();
                if (TestRunnerControl != null)
                    TestRunnerControl.Dispose();
                if (AboutDialog != null)
                    AboutDialog.Dispose();
                throw;
            }
        }

        internal PmlUnitAddin(TestCaseProvider testProvider, AsyncTestRunner runner, SettingsProvider settings)
        {
            if (testProvider == null)
                throw new ArgumentNullException(nameof(testProvider));
            if (runner == null)
                throw new ArgumentNullException(nameof(runner));
            if (runner == null)
                throw new ArgumentNullException(nameof(settings));

            Font font = null;
            TestCaseProvider = testProvider;
            TestRunner = runner;
            try
            {
                font = GetDefaultFont();
                TestRunnerControl = new TestRunnerControl(TestCaseProvider, TestRunner, settings);
                TestRunnerControl.Font = font;
                AboutDialog = new AboutDialog();
                AboutDialog.Font = font;
            }
            catch
            {
                if (font != null)
                    font.Dispose();
                if (TestRunnerControl != null)
                    TestRunnerControl.Dispose();
                if (AboutDialog != null)
                    AboutDialog.Dispose();
                throw;
            }
        }

        ~PmlUnitAddin()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                TestRunner.Dispose();
                TestRunnerControl.Dispose();
                AboutDialog.Dispose();
            }
        }

        public string Description
        {
            get { return "PML Unit Test Runner Addin"; }
        }

        public string Name
        {
            get { return "PML Unit"; }
        }

        [CLSCompliant(false)]
        public void Start(ServiceManager serviceManager)
        {
            Start(new ProxyServiceProvider(serviceManager));
        }

#if E3D_21
        [CLSCompliant(false)]
        public void Start(IDependencyResolver resolver)
        {
            Start(new DependencyResolverServiceProvider(resolver));
        }
#endif

        internal void Start(ServiceProvider provider)
        {
            try
            {
                TestRunnerControl.LoadTests();

                var windowManager = provider.GetService<IWindowManager>();
                var commandManager = provider.GetService<ICommandManager>();
                if (windowManager != null && commandManager != null)
                {
                    commandManager.Commands.Add(new ShowTestRunnerCommand(windowManager, TestRunnerControl));
                    commandManager.Commands.Add(new ShowAboutDialogCommand(windowManager, AboutDialog));
                }
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Stop()
        {
            // PDMS crashes if we also dispose the TestRunnerControl here
            TestRunner.Dispose();
            AboutDialog.Dispose();
        }

        /// <summary>
        /// Gets the default font that we will be using for all controls and windows in PML Unit.
        /// <para>
        /// This is necessary because on Windows versions that don't use Latin script
        /// <code>Control.DefaultFont</code> may look weird with our Controls only displaying
        /// English text.
        /// </para>
        /// </summary>
        /// <remarks>
        /// TOOD: remove this when PML Unit is localized for languages with non-Latin script
        /// and specify the font families and sizes in resource files.
        /// 
        /// See https://stackoverflow.com/questions/61801155/default-font-for-windows-forms-control-with-latin-script-on-a-chinese-windows
        /// </remarks>
        private static Font GetDefaultFont()
        {
            // Although the menus and ribbons of PDMS and E3D use SystemFonts.MenuFont
            // (and even respond to font changes in the system settings), almost all
            // other forms and controls (especially PML forms) use a fixed font which
            // almost certainly is Microsoft Sans Serif. So we try that one first to
            // better match the overall look and feel.
            Font result;
            if (TryGetFont("Microsoft Sans Serif", 8.25f, out result))
                return result;
            else if (TryGetFont("Segoe UI", 9f, out result))
                return result;
            else if (TryGetFont("Arial", 8.25f, out result))
                return result;
            else if (TryGetFont(GenericFontFamilies.SansSerif, 9f, out result))
                return result;
            else
                // clone the system font because our callers may dispose it
                return (Font)SystemFonts.MenuFont.Clone();
        }

        private static bool TryGetFont(string familyName, float size, out Font result)
        {
            FontFamily family = null;
            try
            {
                family = new FontFamily(familyName);
            }
            catch (ArgumentException)
            {
                result = null;
                return false;
            }

            using (family)
            {
                return TryGetFont(family, size, out result);
            }
        }

        private static bool TryGetFont(GenericFontFamilies genericFamily, float size, out Font result)
        {
            using (var family = new FontFamily(genericFamily))
            {
                return TryGetFont(family, size, out result);
            }
        }

        private static bool TryGetFont(FontFamily family, float size, out Font result)
        {
            try
            {
                result = new Font(family, size);
                return true;
            }
            catch (ArgumentException)
            {
                result = null;
                return false;
            }
        }
    }
}
