using System;
using System.Linq;
using System.Windows.Forms;

namespace CapaPresentacion
{
    internal static class KeyboardNavigation
    {
        public static void Enable(Form form, params Button[] buttons)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (buttons == null || buttons.Length == 0) return;

            form.KeyPreview = true;

            form.KeyDown += (s, e) =>
            {
                // Navigate with arrows
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left)
                {
                    MoveToPrevious(form, buttons);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right)
                {
                    MoveToNext(form, buttons);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (form.ActiveControl is Button btn)
                    {
                        btn.PerformClick();
                        e.Handled = true;
                    }
                }
            };
        }

        public static void EnableCrudShortcuts(Form form,
            Action guardar,
            Action agregar,
            Action editar,
            Action eliminar,
            Action actualizar,
            Action regresar,
            DataGridView grid = null)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            form.KeyPreview = true;
            form.KeyDown += (s, e) =>
            {
                // Don't intercept when typing multiline text
                if (form.ActiveControl is TextBoxBase tb && tb.Multiline)
                {
                    return;
                }

                if (e.Control && e.KeyCode == Keys.S)
                {
                    guardar?.Invoke();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.N)
                {
                    agregar?.Invoke();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.E)
                {
                    editar?.Invoke();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    eliminar?.Invoke();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.R)
                {
                    actualizar?.Invoke();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    regresar?.Invoke();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && form.ActiveControl is DataGridView)
                {
                    // When hitting Enter on the grid, open/edit selected row
                    if (grid != null)
                    {
                        editar?.Invoke();
                        e.Handled = true;
                    }
                }
            };
        }

        private static void MoveToNext(Form form, Button[] buttons)
        {
            int idx = GetFocusedButtonIndex(form, buttons);
            int next = (idx + 1) % buttons.Length;
            buttons[next].Focus();
        }

        private static void MoveToPrevious(Form form, Button[] buttons)
        {
            int idx = GetFocusedButtonIndex(form, buttons);
            int prev = (idx - 1 + buttons.Length) % buttons.Length;
            buttons[prev].Focus();
        }

        private static int GetFocusedButtonIndex(Form form, Button[] buttons)
        {
            var focused = form.ActiveControl as Button;
            int idx = Array.IndexOf(buttons, focused);
            if (idx >= 0) return idx;

            // If focus isn't on one of the buttons, try to return nearest by TabIndex
            var ordered = buttons.OrderBy(b => b.TabIndex).ToArray();
            return Array.IndexOf(ordered, ordered.First());
        }
    }
}
