using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using Xunit;

namespace HotelProyecto.Tests
{
    public class PruebasFuncionalesUIDesktop_FlaUI : IDisposable
    {
        private Application? app;
        private UIA3Automation? automation;

        [Fact]
        public void CRUD_With_FlaUI()
        {
            // locate the desktop app executable relative to the test output
            var exePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CapaPresentacion", "bin", "Debug", "net9.0-windows", "CapaPresentacion.exe"));
            Assert.True(File.Exists(exePath), $"Exe not found at {exePath}");

            app = Application.Launch(exePath);
            automation = new UIA3Automation();
            var main = app.GetMainWindow(automation, TimeSpan.FromSeconds(10));
            Assert.NotNull(main);

            // Try to find Gestion de huespedes button (accent/without-accent fallback) with retry
            FlaUI.Core.AutomationElements.Button? btnGuest = null;
            var swBtn = Stopwatch.StartNew();
            while (swBtn.Elapsed < TimeSpan.FromSeconds(10))
            {
                btnGuest = main.FindFirstDescendant(cf => cf.ByText("Gestion de huespedes"))?.AsButton()
                         ?? main.FindFirstDescendant(cf => cf.ByText("Gestión de huéspedes"))?.AsButton()
                         ?? main.FindFirstDescendant(cf => cf.ByText("Gestion de huéspedes"))?.AsButton();
                if (btnGuest != null && btnGuest.IsEnabled) break;
                System.Threading.Thread.Sleep(200);
            }
            if (btnGuest == null) throw new Exception("Botón 'Gestion de huespedes' no encontrado");
            btnGuest.Invoke();

            // The management UI may be hosted inside the main window; wait for a 'Nuevo' button to appear under main
            FlaUI.Core.AutomationElements.Button? btnNuevo = null;
            var swNuevo = Stopwatch.StartNew();
            while (swNuevo.Elapsed < TimeSpan.FromSeconds(12))
            {
                var candidates = main.FindAllDescendants(cf => cf.ByControlType(ControlType.Button))
                                    .Select(e => e.AsButton())
                                    .Where(b => b != null && string.Equals(b.Name, "Nuevo", StringComparison.OrdinalIgnoreCase))
                                    .ToArray();
                // prefer visible & enabled
                btnNuevo = candidates.FirstOrDefault(c => c.IsEnabled && !c.IsOffscreen)
                         ?? candidates.FirstOrDefault();
                if (btnNuevo != null && btnNuevo.IsEnabled) break;
                System.Threading.Thread.Sleep(250);
            }
            if (btnNuevo == null) throw new Exception("Botón 'Nuevo' no encontrado dentro del área del main");
            var guestRoot = btnNuevo.Parent;
            btnNuevo.Invoke();


            // Wait for edit controls to appear after clicking Nuevo
            FlaUI.Core.AutomationElements.TextBox[] edits = Array.Empty<FlaUI.Core.AutomationElements.TextBox>();
            var swEd = Stopwatch.StartNew();
            while (swEd.Elapsed < TimeSpan.FromSeconds(6))
            {
                edits = guestRoot.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit)).Select(e => e.AsTextBox()).ToArray();
                if (edits.Length >= 1) break;
                System.Threading.Thread.Sleep(150);
            }
            if (edits.Length >= 3)
            {
                edits[0].Enter("Manuel Antonio");
                edits[1].Enter("manuel@example.com");
                edits[2].Enter("6861404265");
            }

            var btnGuardar = guestRoot.FindFirstDescendant(cf => cf.ByText("Guardar"))?.AsButton();
            var swSave = Stopwatch.StartNew();
            while (swSave.Elapsed < TimeSpan.FromSeconds(5))
            {
                if (btnGuardar != null && btnGuardar.IsEnabled) break;
                btnGuardar = guestRoot.FindFirstDescendant(cf => cf.ByText("Guardar"))?.AsButton();
                System.Threading.Thread.Sleep(150);
            }
            if (btnGuardar == null) throw new Exception("Botón 'Guardar' no encontrado");
            btnGuardar.Invoke();

            // Small wait to allow save
            System.Threading.Thread.Sleep(500);

            // Try to close the management UI via 'Cerrar' button if present
            var btnCerrar = guestRoot.FindFirstDescendant(cf => cf.ByText("Cerrar"))?.AsButton();
            if (btnCerrar != null && btnCerrar.IsEnabled)
            {
                btnCerrar.Invoke();
            }

            // Close app
            app.Close();
            app = null;
        }

        private Window WaitForWindowContaining(UIA3Automation automation, string partialTitle, TimeSpan timeout)
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < timeout)
            {
                var windows = automation.GetDesktop().FindAllChildren(cf => cf.ByControlType(ControlType.Window));
                var w = windows.FirstOrDefault(win => (win.Name ?? string.Empty).IndexOf(partialTitle, StringComparison.OrdinalIgnoreCase) >= 0);
                if (w != null)
                {
                    return w.AsWindow();
                }
                System.Threading.Thread.Sleep(200);
            }
            throw new Exception($"No se encontró ventana con título parcial '{partialTitle}' dentro de {timeout.TotalSeconds}s");
        }

        public void Dispose()
        {
            try
            {
                automation?.Dispose();
            }
            catch { }
            try
            {
                app?.Close();
            }
            catch { }
        }
    }
}
