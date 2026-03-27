using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocios;

namespace CapaPresentacion
{
    public partial class Form4 : Form
    {
        private readonly HuespedBLL huespedBLL = new HuespedBLL();

        public Form4()
        {
            InitializeComponent();
            UiTheme.ApplyTo(this);
            UiTheme.StyleGrid(dgv_Huespedes);
            UiTheme.StyleTitle(lblTotalRegistros);
            AplicarDisenoSimetrico();

            Load += (_, __) => CargarHuespedes();
            btn_Regresar.Click += (_, __) => Close();
            btn_Agregar.Click += (_, __) => LimpiarFormulario();
            btn_Limpiar.Click += (_, __) => LimpiarFormulario();
            btn_Actualizar.Click += (_, __) => CargarHuespedes();
            btn_Editar.Click += (_, __) => CargarDesdeFilaSeleccionada();
            btn_Guardar.Click += (_, __) => GuardarHuesped();
            btn_Eliminar.Click += (_, __) => EliminarHuesped();
            dgv_Huespedes.SelectionChanged += (_, __) => CargarDesdeFilaSeleccionada();
        }

        private void AplicarDisenoSimetrico()
        {
            Color dorado = Color.FromArgb(211, 171, 77);

            BackColor = Color.Black;
            MinimumSize = new Size(980, 680);
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
            int separacion = 10;
            int anchoDisponible = Math.Max(900, ClientSize.Width - (margen * 2));

            int altoSuperior = Math.Max(270, Math.Min((int)(ClientSize.Height * 0.43), 360));
            groupBox1.SetBounds(margen, margen, anchoDisponible, altoSuperior);

            int interior = 14;
            int columnas = 5;
            int colAncho = (groupBox1.ClientSize.Width - (interior * 2) - (separacion * (columnas - 1))) / columnas;
            int etiquetaH = 20;
            int campoH = 28;
            int fila1Y = 30;
            int fila2Y = fila1Y + etiquetaH + campoH + 10;

            UbicarCampo(label1, txt_HuespedID, 0, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label3, txt_Nombre, 1, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label5, txt_ApellidoPaterno, 2, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label6, txt_ApellidoMaterno, 3, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label9, txt_Correo, 4, fila1Y, interior, separacion, colAncho, etiquetaH, campoH);

            UbicarCampo(label2, txt_Calle, 0, fila2Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label4, txt_Colonia, 1, fila2Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label7, txt_CodigoPostal, 2, fila2Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label8, txt_Ciudad, 3, fila2Y, interior, separacion, colAncho, etiquetaH, campoH);
            UbicarCampo(label10, txt_NumeroCelular, 4, fila2Y, interior, separacion, colAncho, etiquetaH, campoH);

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
            dgv_Huespedes.SetBounds(margen, gridY, anchoDisponible, gridH);
        }

        private static void UbicarCampo(Label label, Control input, int columna, int yBase, int interior, int separacion, int colAncho, int etiquetaH, int campoH)
        {
            int x = interior + (columna * (colAncho + separacion));
            label.SetBounds(x, yBase, colAncho, etiquetaH);
            input.SetBounds(x, yBase + etiquetaH, colAncho, campoH);
        }

        private void CargarHuespedes()
        {
            try
            {
                DataTable datos = huespedBLL.ObtenerTodos();
                dgv_Huespedes.DataSource = datos;
                lblTotalRegistros.Text = $"Huéspedes existentes: {datos.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudieron cargar los huéspedes.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDesdeFilaSeleccionada()
        {
            if (dgv_Huespedes.CurrentRow == null)
            {
                return;
            }

            txt_HuespedID.Text = dgv_Huespedes.CurrentRow.Cells["huesped_id"].Value?.ToString() ?? "0";
            txt_Nombre.Text = dgv_Huespedes.CurrentRow.Cells["nombre"].Value?.ToString() ?? string.Empty;
            txt_ApellidoPaterno.Text = dgv_Huespedes.CurrentRow.Cells["apellido_1"].Value?.ToString() ?? string.Empty;
            txt_ApellidoMaterno.Text = dgv_Huespedes.CurrentRow.Cells["apellido_2"].Value?.ToString() ?? string.Empty;
            txt_Calle.Text = dgv_Huespedes.CurrentRow.Cells["calle"].Value?.ToString() ?? string.Empty;
            txt_Colonia.Text = dgv_Huespedes.CurrentRow.Cells["colonia"].Value?.ToString() ?? string.Empty;
            txt_CodigoPostal.Text = dgv_Huespedes.CurrentRow.Cells["codigo_postal"].Value?.ToString() ?? string.Empty;
            txt_Ciudad.Text = dgv_Huespedes.CurrentRow.Cells["ciudad"].Value?.ToString() ?? string.Empty;
            txt_Correo.Text = dgv_Huespedes.CurrentRow.Cells["correo"].Value?.ToString() ?? string.Empty;
            txt_NumeroCelular.Text = dgv_Huespedes.CurrentRow.Cells["numero_celular"].Value?.ToString() ?? string.Empty;
        }

        private bool ValidarFormulario(out int codigoPostal)
        {
            codigoPostal = 0;

            if (string.IsNullOrWhiteSpace(txt_Nombre.Text) ||
                string.IsNullOrWhiteSpace(txt_ApellidoPaterno.Text) ||
                string.IsNullOrWhiteSpace(txt_Calle.Text) ||
                string.IsNullOrWhiteSpace(txt_Colonia.Text) ||
                string.IsNullOrWhiteSpace(txt_CodigoPostal.Text) ||
                string.IsNullOrWhiteSpace(txt_Ciudad.Text) ||
                string.IsNullOrWhiteSpace(txt_Correo.Text) ||
                string.IsNullOrWhiteSpace(txt_NumeroCelular.Text))
            {
                MessageBox.Show("Completa todos los campos obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txt_CodigoPostal.Text, out codigoPostal))
            {
                MessageBox.Show("El código postal debe ser numérico.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void GuardarHuesped()
        {
            if (!ValidarFormulario(out int codigoPostal))
            {
                return;
            }

            try
            {
                bool actualizar = int.TryParse(txt_HuespedID.Text, out int huespedId) && huespedId > 0;
                string correo = txt_Correo.Text.Trim();
                string contrasena = "123456";

                if (actualizar)
                {
                    Huesped existente = huespedBLL.ObtenerPorCorreo(correo);
                    if (existente != null && !string.IsNullOrWhiteSpace(existente.contrasena))
                    {
                        contrasena = existente.contrasena;
                    }
                }

                Huesped huesped = new Huesped
                {
                    huesped_id = huespedId,
                    nombre = txt_Nombre.Text.Trim(),
                    apellido_1 = txt_ApellidoPaterno.Text.Trim(),
                    apellido_2 = txt_ApellidoMaterno.Text.Trim(),
                    calle = txt_Calle.Text.Trim(),
                    colonia = txt_Colonia.Text.Trim(),
                    codigo_postal = codigoPostal,
                    ciudad = txt_Ciudad.Text.Trim(),
                    correo = correo,
                    numero_celular = txt_NumeroCelular.Text.Trim(),
                    contrasena = contrasena
                };

                bool exito = actualizar ? huespedBLL.Actualizar(huesped) : huespedBLL.Agregar(huesped);

                if (!exito)
                {
                    MessageBox.Show("No fue posible guardar el huésped.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Operación realizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarHuespedes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el huésped.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarHuesped()
        {
            if (!int.TryParse(txt_HuespedID.Text, out int huespedId) || huespedId <= 0)
            {
                MessageBox.Show("Selecciona un huésped válido para eliminar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmacion = MessageBox.Show("¿Deseas eliminar el huésped seleccionado?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool exito = huespedBLL.Eliminar(huespedId);
                if (!exito)
                {
                    MessageBox.Show("No fue posible eliminar el huésped.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show("Huésped eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarHuespedes();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el huésped.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            txt_HuespedID.Text = "0";
            txt_Nombre.Clear();
            txt_ApellidoPaterno.Clear();
            txt_ApellidoMaterno.Clear();
            txt_Calle.Clear();
            txt_Colonia.Clear();
            txt_CodigoPostal.Clear();
            txt_Ciudad.Clear();
            txt_Correo.Clear();
            txt_NumeroCelular.Clear();
            txt_Nombre.Focus();
        }
    }
}
