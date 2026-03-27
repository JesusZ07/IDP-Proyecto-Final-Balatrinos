using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocios;

namespace CapaPresentacion
{
    public partial class Form3 : Form
    {
        private readonly ReservacionBLL reservacionBLL = new ReservacionBLL();

        public Form3()
        {
            InitializeComponent();
            UiTheme.ApplyTo(this);
            UiTheme.StyleGrid(dgv_Reservaciones);
            UiTheme.StyleTitle(lblTotalRegistros);
            AplicarDisenoSimetrico();

            Load += (_, __) => CargarReservaciones();
            btn_Regresar.Click += (_, __) => Close();
            btn_Agregar.Click += (_, __) => LimpiarFormulario();
            btn_Limpiar.Click += (_, __) => LimpiarFormulario();
            btn_Actualizar.Click += (_, __) => CargarReservaciones();
            btn_Editar.Click += (_, __) => CargarDesdeFilaSeleccionada();
            btn_Guardar.Click += (_, __) => GuardarReservacion();
            btn_Eliminar.Click += (_, __) => EliminarReservacion();
            dgv_Reservaciones.SelectionChanged += (_, __) => CargarDesdeFilaSeleccionada();
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

            UbicarCampo(label3, txt_NombreHuesped, 0, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label1, txt_ReservacionID, 1, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label5, txt_NumeroPersonas, 2, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label4, txt_Estatus, 0, fila2Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label2, dtp_FechaEntrada, 1, fila2Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label6, dtp_FechaSalida, 2, fila2Y, interior, separacion, colAncho, etiquetaH, campoH);

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
            dgv_Reservaciones.SetBounds(margen, gridY, anchoDisponible, gridH);
        }

        private static void UbicarCampo(Label label, Control input, int columna, int yBase, int interior, int separacion, int colAncho, int etiquetaH, int campoH)
        {
            int x = interior + (columna * (colAncho + separacion));
            label.SetBounds(x, yBase, colAncho, etiquetaH);
            input.SetBounds(x, yBase + etiquetaH, colAncho, campoH);
        }

        private void CargarReservaciones()
        {
            try
            {
                DataTable datos = reservacionBLL.ObtenerTodos();
                dgv_Reservaciones.DataSource = datos;
                lblTotalRegistros.Text = $"Reservaciones existentes: {datos.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudieron cargar las reservaciones.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDesdeFilaSeleccionada()
        {
            if (dgv_Reservaciones.CurrentRow == null)
            {
                return;
            }

            txt_ReservacionID.Text = dgv_Reservaciones.CurrentRow.Cells["reservacion_id"].Value?.ToString() ?? "0";
            txt_Estatus.Text = dgv_Reservaciones.CurrentRow.Cells["estatus"].Value?.ToString() ?? string.Empty;
            txt_NombreHuesped.Text = dgv_Reservaciones.CurrentRow.Cells["nombre_huesped"].Value?.ToString() ?? string.Empty;
            txt_NumeroPersonas.Text = dgv_Reservaciones.CurrentRow.Cells["numero_personas"].Value?.ToString() ?? string.Empty;

            DateTime fechaEntrada = DateTime.Today;
            DateTime fechaSalida = DateTime.Today.AddDays(1);

            DateTime.TryParse(dgv_Reservaciones.CurrentRow.Cells["fecha_entrada"].Value?.ToString(), out fechaEntrada);
            DateTime.TryParse(dgv_Reservaciones.CurrentRow.Cells["fecha_salida"].Value?.ToString(), out fechaSalida);

            dtp_FechaEntrada.Value = fechaEntrada;
            dtp_FechaSalida.Value = fechaSalida;
        }

        private bool ValidarFormulario(out int numeroPersonas)
        {
            numeroPersonas = 0;

            if (string.IsNullOrWhiteSpace(txt_NombreHuesped.Text) ||
                string.IsNullOrWhiteSpace(txt_Estatus.Text) ||
                string.IsNullOrWhiteSpace(txt_NumeroPersonas.Text))
            {
                MessageBox.Show("Completa todos los campos obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txt_NumeroPersonas.Text, out numeroPersonas) || numeroPersonas <= 0)
            {
                MessageBox.Show("El número de personas debe ser mayor a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtp_FechaSalida.Value.Date <= dtp_FechaEntrada.Value.Date)
            {
                MessageBox.Show("La fecha de salida debe ser posterior a la fecha de entrada.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void GuardarReservacion()
        {
            if (!ValidarFormulario(out int numeroPersonas))
            {
                return;
            }

            try
            {
                bool actualizar = int.TryParse(txt_ReservacionID.Text, out int reservacionId) && reservacionId > 0;

                Reservacion reservacion = new Reservacion
                {
                    reservacion_id = reservacionId,
                    nombre_huesped = txt_NombreHuesped.Text.Trim(),
                    estatus = txt_Estatus.Text.Trim(),
                    fecha_entrada = dtp_FechaEntrada.Value.Date,
                    fecha_salida = dtp_FechaSalida.Value.Date,
                    numero_personas = numeroPersonas
                };

                bool exito = actualizar ? reservacionBLL.Actualizar(reservacion) : reservacionBLL.Agregar(reservacion);

                if (!exito)
                {
                    MessageBox.Show("No fue posible guardar la reservación.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Operación realizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarReservaciones();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la reservación.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarReservacion()
        {
            if (!int.TryParse(txt_ReservacionID.Text, out int reservacionId) || reservacionId <= 0)
            {
                MessageBox.Show("Selecciona una reservación válida para eliminar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmacion = MessageBox.Show("¿Deseas eliminar la reservación seleccionada?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool exito = reservacionBLL.Eliminar(reservacionId);
                if (!exito)
                {
                    MessageBox.Show("No fue posible eliminar la reservación.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Reservación eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarReservaciones();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la reservación.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            txt_ReservacionID.Text = "0";
            txt_NombreHuesped.Clear();
            txt_Estatus.Clear();
            txt_NumeroPersonas.Clear();
            dtp_FechaEntrada.Value = DateTime.Today;
            dtp_FechaSalida.Value = DateTime.Today.AddDays(1);
            txt_NombreHuesped.Focus();
        }
    }
}
