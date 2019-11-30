// Copyright (c) 2019 Florian Zimmermann.
// Licensed under the MIT License: https://opensource.org/licenses/MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using NUnit.Framework;

namespace PmlUnit.Tests
{
    static class ControlExtensions
    {
        public static T FindControl<T>(this Control parent, string name) where T : Control
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var controls = new Stack<Control>(parent.Controls.OfType<Control>());
            while (controls.Count > 0)
            {
                var control = controls.Pop();
                var casted = control as T;
                if (casted != null && casted.Name == name)
                {
                    return casted;
                }

                foreach (Control child in control.Controls)
                {
                    controls.Push(child);
                }
            }

            Assert.Fail("Unable to find a {0} named \"{1}\".", typeof(T), name);
            return null;
        }

        public static TestListViewModel GetModel(this TestListView view)
        {
            return view.GetField<TestListViewModel>("Model");
        }

        public static TestListViewController GetController(this TestListView view)
        {
            return view.GetField<TestListViewController>("Controller");
        }

        public static T GetField<T>(this object value, string name) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var field = value.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field, "{} has non prive instance field named \"{}\".", value.GetType(), name);

            var result = field.GetValue(value);
            Assert.NotNull(result, "{}.{} is null.", value.GetType(), name);
            Assert.IsInstanceOf(typeof(T), result, "{}.{} is not an instance of {}.", value.GetType(), name, typeof(T));

            return result as T;
        }
    }
}
