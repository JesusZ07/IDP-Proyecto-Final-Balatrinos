using System.Drawing;
using System.Windows.Forms;

namespace CapaPresentacion
{
    internal static class UiTheme
    {
        public static readonly Color AccentGold = Color.FromArgb(211, 171, 77);
        public static readonly Color DeepBlack = Color.FromArgb(17, 17, 17);
        public static readonly Color SoftBackground = Color.FromArgb(247, 245, 240);
        public static readonly Color Surface = Color.White;
        public static readonly Color MutedText = Color.FromArgb(90, 90, 90);

        public static void ApplyTo(Form form)
        {
            form.BackColor = SoftBackground;
            form.ForeColor = DeepBlack;
            form.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            form.StartPosition = FormStartPosition.CenterScreen;

            StyleControls(form.Controls);
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 248, 243);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 33, 33);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = DeepBlack;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = AccentGold;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }

        public static void StyleTitle(Label label)
        {
            label.ForeColor = DeepBlack;
            label.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
        }

        private static void StyleControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is GroupBox group)
                {
                    group.BackColor = Surface;
                    group.ForeColor = DeepBlack;
                    group.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point);
                }

                if (control is Label label)
                {
                    if (label.Font.Size < 12)
                    {
                        label.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
                    }

                    label.ForeColor = DeepBlack;
                }

                if (control is TextBox textBox)
                {
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    textBox.BackColor = Surface;
                    textBox.ForeColor = DeepBlack;
                }

                if (control is DateTimePicker picker)
                {
                    picker.CalendarForeColor = DeepBlack;
                    picker.CalendarMonthBackground = Surface;
                    picker.CalendarTitleBackColor = AccentGold;
                    picker.CalendarTitleForeColor = DeepBlack;
                }

                if (control is Button button)
                {
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.Cursor = Cursors.Hand;
                    button.BackColor = AccentGold;
                    button.ForeColor = DeepBlack;
                    button.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);

                    if (button.Name.Contains("Regresar") || button.Text.Contains("Cerrar"))
                    {
                        button.BackColor = DeepBlack;
                        button.ForeColor = AccentGold;
                    }
                }

                if (control.HasChildren)
                {
                    StyleControls(control.Controls);
                }
            }
        }
    }
}
