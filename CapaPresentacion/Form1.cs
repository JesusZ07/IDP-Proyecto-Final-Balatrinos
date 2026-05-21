using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ConfigurarDisenoPrincipal();

            btn_GestionHabitaciones.Click += (_, __) => AbrirFormulario(new Form2());
            btn_GestionReservaciones.Click += (_, __) => AbrirFormulario(new Form3());
            btn_GestionHuespedes.Click += (_, __) => AbrirFormulario(new Form4());
            // Habilitar navegación por teclado (flechas + Enter)
            KeyboardNavigation.Enable(this, btn_GestionHabitaciones, btn_GestionReservaciones, btn_GestionHuespedes);
        }

        private void ConfigurarDisenoPrincipal()
        {
            BackColor = Color.Black;
            MinimumSize = new Size(900, 540);
            panelIzquierdo.BackColor = Color.Black;
            panelDerecho.BackColor = Color.Black;

            EstilizarBoton(btn_GestionHabitaciones);
            EstilizarBoton(btn_GestionReservaciones);
            EstilizarBoton(btn_GestionHuespedes);

            Resize += (_, __) => AjustarLayoutResponsivo();
            Shown += (_, __) => AjustarLayoutResponsivo();

            string rutaLogo = Path.Combine(Application.StartupPath, "Assets", "LogoHotel.png");
            if (!File.Exists(rutaLogo))
            {
                rutaLogo = Path.Combine(Application.StartupPath, "Assets", "LogoOasis.png");
            }

            if (File.Exists(rutaLogo))
            {
                picLogo.Image = Image.FromFile(rutaLogo);
            }

            AjustarLayoutResponsivo();
        }

        private void AjustarLayoutResponsivo()
        {
            panelIzquierdo.Width = ClientSize.Width / 2;

            int anchoBoton = Math.Max(240, Math.Min(360, panelDerecho.ClientSize.Width - 110));
            int altoBoton = 72;
            int separacion = 24;
            int bloqueAlto = (altoBoton * 3) + (separacion * 2);

            int x = (panelDerecho.ClientSize.Width - anchoBoton) / 2;
            int yInicio = (panelDerecho.ClientSize.Height - bloqueAlto) / 2;

            btn_GestionHabitaciones.SetBounds(x, yInicio, anchoBoton, altoBoton);
            btn_GestionReservaciones.SetBounds(x, yInicio + altoBoton + separacion, anchoBoton, altoBoton);
            btn_GestionHuespedes.SetBounds(x, yInicio + (altoBoton + separacion) * 2, anchoBoton, altoBoton);
        }

        private static void EstilizarBoton(Button boton)
        {
            boton.FlatAppearance.BorderSize = 0;
            boton.Cursor = Cursors.Hand;
            boton.BackColor = Color.FromArgb(211, 171, 77);
            boton.ForeColor = Color.FromArgb(17, 17, 17);
        }

        private void AbrirFormulario(Form formulario)
        {
            formulario.StartPosition = FormStartPosition.CenterScreen;
            // When running automated tests set TEST_MODE=1 in the environment so this uses non-blocking Show()
            if (Environment.GetEnvironmentVariable("TEST_MODE") == "1")
            {
                // Force the new form to appear in foreground for automated tests
                formulario.TopMost = true;
                formulario.Show(this);
                formulario.BringToFront();
                formulario.Activate();
                // remove TopMost after shown so it doesn't stay above other windows
                formulario.TopMost = false;
            }
            else
            {
                formulario.TopMost = true;
                formulario.ShowDialog(this);
                // ensure it's active when shown modally
                formulario.Activate();
                formulario.TopMost = false;
            }
        }
    }
}
