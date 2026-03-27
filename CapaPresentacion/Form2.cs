using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocios;

namespace CapaPresentacion
{
    public partial class Form2 : Form
    {
        private readonly HabitacionBLL habitacionBLL = new HabitacionBLL();

        public Form2()
        {
            InitializeComponent();
            UiTheme.ApplyTo(this);
            UiTheme.StyleGrid(dgv_Habitaciones);
            UiTheme.StyleTitle(lblTotalRegistros);
            AplicarDisenoSimetrico();

            Load += (_, __) => CargarHabitaciones();
            btn_Regresar.Click += (_, __) => Close();
            btn_Agregar.Click += (_, __) => LimpiarFormulario();
            btn_Limpiar.Click += (_, __) => LimpiarFormulario();
            btn_Actualizar.Click += (_, __) => CargarHabitaciones();
            btn_Editar.Click += (_, __) => CargarDesdeFilaSeleccionada();
            btn_Guardar.Click += (_, __) => GuardarHabitacion();
            btn_Eliminar.Click += (_, __) => EliminarHabitacion();
            dgv_Habitaciones.SelectionChanged += (_, __) => CargarDesdeFilaSeleccionada();
        }

        private void AplicarDisenoSimetrico()
        {
            Color dorado = Color.FromArgb(211, 171, 77);

            BackColor = Color.Black;
            MinimumSize = new Size(900, 620);
            AutoScroll = true;

            groupBox1.BackColor = Color.FromArgb(18, 18, 18);
            groupBox1.ForeColor = dorado;

            foreach (Control control in groupBox1.Controls)
            {
                if (control is Label label)
                {
                    label.ForeColor = dorado;
                }
            }

            lblTotalRegistros.ForeColor = dorado;
            lblTotalRegistros.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point);

            ConfigurarBotonesUniformes();
            Resize += (_, __) => AjustarLayoutResponsive();
            Shown += (_, __) => AjustarLayoutResponsive();
            AjustarLayoutResponsive();
        }

        private void ConfigurarBotonesUniformes()
        {
            Color dorado = Color.FromArgb(211, 171, 77);
            Color negro = Color.FromArgb(17, 17, 17);

            Button[] botonesAccion = { btn_Guardar, btn_Limpiar, btn_Eliminar, btn_Agregar, btn_Actualizar, btn_Editar };
            foreach (Button boton in botonesAccion)
            {
                boton.FlatStyle = FlatStyle.Flat;
                boton.FlatAppearance.BorderSize = 0;
                boton.BackColor = dorado;
                boton.ForeColor = negro;
                boton.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            }

            btn_Regresar.FlatStyle = FlatStyle.Flat;
            btn_Regresar.FlatAppearance.BorderSize = 0;
            btn_Regresar.BackColor = dorado;
            btn_Regresar.ForeColor = negro;
            btn_Regresar.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
        }

        private void AjustarLayoutResponsive()
        {
            int margen = 24;
            int separacion = 12;
            int anchoDisponible = Math.Max(820, ClientSize.Width - (margen * 2));

            int altoSuperior = Math.Max(230, Math.Min((int)(ClientSize.Height * 0.40), 320));
            groupBox1.SetBounds(margen, margen, anchoDisponible, altoSuperior);

            int interior = 16;
            int columnas = 3;
            int colAncho = (groupBox1.ClientSize.Width - (interior * 2) - (separacion * (columnas - 1))) / columnas;
            int etiquetaH = 20;
            int campoH = 28;
            int fila1Y = 30;
            int fila2Y = fila1Y + etiquetaH + campoH + 12;

            UbicarCampo(label3, txt_TipoHabitacion, 0, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label1, txt_HabitacionID, 1, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label5, txt_NumeroHabitacion, 2, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label2, txt_Piso, 0, fila2Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label4, txt_Estatus, 1, fila2Y, interior, separacion, colAncho, etiquetaH, campoH);

            Button[] botonesAccion = { btn_Guardar, btn_Limpiar, btn_Eliminar, btn_Agregar, btn_Actualizar, btn_Editar };
            int btnY = groupBox1.ClientSize.Height - 46;
            int btnAncho = (groupBox1.ClientSize.Width - (interior * 2) - (separacion * (botonesAccion.Length - 1))) / botonesAccion.Length;
            int btnX = interior;
            foreach (Button boton in botonesAccion)
            {
                boton.SetBounds(btnX, btnY, btnAncho, 34);
                btnX += btnAncho + separacion;
            }

            int yInfo = groupBox1.Bottom + 12;
            lblTotalRegistros.SetBounds(margen, yInfo, anchoDisponible - 150, 30);
            btn_Regresar.SetBounds(margen + anchoDisponible - 130, yInfo, 130, 34);

            int gridY = yInfo + 40;
            int gridH = Math.Max(180, ClientSize.Height - gridY - margen);
            dgv_Habitaciones.SetBounds(margen, gridY, anchoDisponible, gridH);
        }

        private static void UbicarCampo(Label label, Control input, int columna, int yBase, int interior, int separacion, int colAncho, int etiquetaH, int campoH)
        {
            int x = interior + (columna * (colAncho + separacion));
            label.SetBounds(x, yBase, colAncho, etiquetaH);
            input.SetBounds(x, yBase + etiquetaH, colAncho, campoH);
        }

        private void CargarHabitaciones()
        {
            try
            {
                DataTable datos = habitacionBLL.ObtenerTodos();
                dgv_Habitaciones.DataSource = datos;
                lblTotalRegistros.Text = $"Habitaciones existentes: {datos.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudieron cargar las habitaciones.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDesdeFilaSeleccionada()
        {
            if (dgv_Habitaciones.CurrentRow == null)
            {
                return;
            }

            txt_HabitacionID.Text = dgv_Habitaciones.CurrentRow.Cells["numero_habitacion"].Value?.ToString() ?? "0";
            txt_NumeroHabitacion.Text = dgv_Habitaciones.CurrentRow.Cells["numero_habitacion"].Value?.ToString() ?? string.Empty;
            txt_TipoHabitacion.Text = dgv_Habitaciones.CurrentRow.Cells["tipo_habitacion"].Value?.ToString() ?? string.Empty;
            txt_Piso.Text = dgv_Habitaciones.CurrentRow.Cells["piso"].Value?.ToString() ?? string.Empty;
            txt_Estatus.Text = dgv_Habitaciones.CurrentRow.Cells["estatus"].Value?.ToString() ?? string.Empty;
        }

        private bool ValidarFormulario(out int numeroHabitacion, out int piso)
        {
            numeroHabitacion = 0;
            piso = 0;

            if (string.IsNullOrWhiteSpace(txt_TipoHabitacion.Text) ||
                string.IsNullOrWhiteSpace(txt_Piso.Text) ||
                string.IsNullOrWhiteSpace(txt_Estatus.Text) ||
                string.IsNullOrWhiteSpace(txt_NumeroHabitacion.Text))
            {
                MessageBox.Show("Completa todos los campos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txt_NumeroHabitacion.Text, out numeroHabitacion))
            {
                MessageBox.Show("El número de habitación debe ser numérico.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txt_Piso.Text, out piso))
            {
                MessageBox.Show("El piso debe ser numérico.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void GuardarHabitacion()
        {
            if (!ValidarFormulario(out int numeroHabitacion, out int piso))
            {
                return;
            }

            try
            {
                Habitacion habitacion = new Habitacion
                {
                    numero_habitacion = numeroHabitacion,
                    tipo_habitacion = txt_TipoHabitacion.Text.Trim(),
                    piso = piso,
                    estatus = txt_Estatus.Text.Trim()
                };

                bool actualizar = int.TryParse(txt_HabitacionID.Text, out int idActual) && idActual > 0;
                bool exito = actualizar ? habitacionBLL.Actualizar(habitacion) : habitacionBLL.Agregar(habitacion);

                if (!exito)
                {
                    MessageBox.Show("No fue posible guardar la habitación.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Operación realizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarHabitaciones();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la habitación.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarHabitacion()
        {
            if (!int.TryParse(txt_NumeroHabitacion.Text, out int numeroHabitacion) || numeroHabitacion <= 0)
            {
                MessageBox.Show("Selecciona una habitación válida para eliminar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmacion = MessageBox.Show("¿Deseas eliminar la habitación seleccionada?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool exito = habitacionBLL.Eliminar(numeroHabitacion);
                if (!exito)
                {
                    MessageBox.Show("No fue posible eliminar la habitación.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Habitación eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarHabitaciones();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la habitación.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            txt_HabitacionID.Text = "0";
            txt_NumeroHabitacion.Clear();
            txt_TipoHabitacion.Clear();
            txt_Piso.Clear();
            txt_Estatus.Clear();
            txt_NumeroHabitacion.Focus();
        }
    }
}
