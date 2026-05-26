using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;
using CapaDatos;
using CapaNegocios;
using CapaEntidad;

namespace HotelProyecto.Tests
{
    public static class TestReporter
    {
        private static string? ReportHtmlPath;
        private static readonly ConcurrentBag<ReportEntry> Entries = new();

        static TestReporter() => Initialize();

        private class ReportEntry
        {
            public DateTime Time { get; init; }
            public string TestName { get; init; } = string.Empty;
            public string Status { get; init; } = string.Empty;
            public double DurationMs { get; init; }
            public string Message { get; init; } = string.Empty;
        }

        public static void Initialize()
        {
            try
            {
                var dir = AppContext.BaseDirectory;
                var di = new DirectoryInfo(dir);
                while (di != null && !File.Exists(Path.Combine(di.FullName, "IDP-Proyecto-Final-Balatrinos.sln"))) di = di.Parent;
                var root = di != null ? di.FullName : AppContext.BaseDirectory;
                var reportDir = Path.Combine(root, "TestResults");
                Directory.CreateDirectory(reportDir);
                if (string.IsNullOrEmpty(ReportHtmlPath)) ReportHtmlPath = Path.Combine(reportDir, $"UI_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html");
                AppDomain.CurrentDomain.ProcessExit += (s, e) => WriteReport();
                if (!File.Exists(ReportHtmlPath)) File.WriteAllText(ReportHtmlPath, "<html><body><h3>UI Report initialized</h3></body></html>");
            }
            catch { }
        }

        public static void Append(string testName, string status, TimeSpan duration, string? message = null)
        {
            try
            {
                Entries.Add(new ReportEntry
                {
                    Time = DateTime.Now,
                    TestName = testName ?? string.Empty,
                    Status = status ?? string.Empty,
                    DurationMs = duration.TotalMilliseconds,
                    Message = message ?? string.Empty
                });
            }
            catch { }
        }

        private static void WriteReport()
        {
            try
            {
                if (string.IsNullOrEmpty(ReportHtmlPath)) return;
                var list = Entries.OrderBy(e => e.Time).ToList();
                var total = list.Count;
                var passed = list.Count(e => string.Equals(e.Status, "PASSED", StringComparison.OrdinalIgnoreCase));
                var failed = list.Count(e => string.Equals(e.Status, "FAILED", StringComparison.OrdinalIgnoreCase));
                var skipped = list.Count(e => string.Equals(e.Status, "SKIPPED", StringComparison.OrdinalIgnoreCase));
                var execMs = (long)list.Sum(e => e.DurationMs);
                var overall = failed > 0 ? "FAILED" : "SUCCEEDED";
                var gen = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string RowClass(string status) => status.Equals("PASSED", StringComparison.OrdinalIgnoreCase) ? "ok" : status.Equals("FAILED", StringComparison.OrdinalIgnoreCase) ? "bad" : "skip";

                var rows = list.Select(e => $"<tr><td>{System.Net.WebUtility.HtmlEncode(e.TestName)}</td><td class='" + RowClass(e.Status) + "'>" + System.Net.WebUtility.HtmlEncode(e.Status.ToUpperInvariant()) + $"</td><td>{Math.Round(e.DurationMs)} ms</td><td><pre>{System.Net.WebUtility.HtmlEncode(e.Message)}</pre></td></tr>");

                // Try to merge existing unit test report if present (Run-unit/report.html)
                try
                {
                    var rootDir = Path.GetDirectoryName(Path.GetDirectoryName(ReportHtmlPath)) ?? AppContext.BaseDirectory;
                    var candidate = Path.Combine(rootDir, "HotelProyecto.Tests", "TestResults", "Run-unit", "report.html");
                    if (!File.Exists(candidate))
                    {
                        // alternative location: HotelProyecto.Tests/TestResults/Run-unit/report.html under solution root
                        var solutionRoot = Path.GetDirectoryName(rootDir) ?? rootDir;
                        candidate = Path.Combine(solutionRoot, "HotelProyecto.Tests", "TestResults", "Run-unit", "report.html");
                    }
                    if (File.Exists(candidate))
                    {
                        var unitHtml = File.ReadAllText(candidate);
                        var tbStart = unitHtml.IndexOf("<tbody>", StringComparison.OrdinalIgnoreCase);
                        var tbEnd = unitHtml.IndexOf("</tbody>", StringComparison.OrdinalIgnoreCase);
                        if (tbStart >= 0 && tbEnd > tbStart)
                        {
                            var inner = unitHtml.Substring(tbStart + 7, tbEnd - (tbStart + 7));
                            // append unit rows to rows list
                            rows = rows.Concat(new[] { inner });

                            // update counts from unit report
                            passed += CountOccurrences(unitHtml, "class='ok'");
                            failed += CountOccurrences(unitHtml, "class='bad'");
                            skipped += CountOccurrences(unitHtml, "class='skip'");
                            total += CountHtmlRowCount(inner);
                        }
                    }
                }
                catch { }

                var html = $"<!doctype html>\n<html lang='es'>\n<head>\n<meta charset='utf-8' />\n<meta name='viewport' content='width=device-width, initial-scale=1' />\n<title>Test Report - ui</title>\n<style>body {{ font-family: Segoe UI, Arial, sans-serif; margin: 24px; background: #f4f7fb; color: #1a1a1a; }}header {{ margin-bottom: 16px; }}h1 {{ margin: 0 0 4px 0; }}.meta {{ color: #555; }}.grid {{ display: grid; grid-template-columns: repeat(4, minmax(120px, 1fr)); gap: 10px; margin: 16px 0; }}.card {{ background: white; border-radius: 10px; padding: 14px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); }}.k {{ font-size: 12px; color: #666; text-transform: uppercase; }}.v {{ font-size: 22px; font-weight: 700; margin-top: 4px; }}table {{ width: 100%; border-collapse: collapse; background: white; border-radius: 10px; overflow: hidden; }}th, td {{ border-bottom: 1px solid #eee; padding: 10px; text-align: left; vertical-align: top; }}th {{ background: #0f172a; color: white; }}.ok {{ color: #0a7a34; font-weight: 700; }}.bad {{ color: #b91c1c; font-weight: 700; }}.skip {{ color: #9a6700; font-weight: 700; }}pre {{ margin: 0; white-space: pre-wrap; max-width: 680px; }}\n</style>\n</head>\n<body>\n<header>\n  <h1>Test Report - ui</h1>\n  <div class='meta'>Generado: {gen}</div>\n  <div class='meta'>Resultado general: {overall} | Ejecucion: {execMs} ms | Total (incl. build): {execMs} ms</div>\n</header>\n<div class='grid'>\n  <div class='card'><div class='k'>Total</div><div class='v'>{total}</div></div>\n  <div class='card'><div class='k'>Passed</div><div class='v'>{passed}</div></div>\n  <div class='card'><div class='k'>Failed</div><div class='v'>{failed}</div></div>\n  <div class='card'><div class='k'>Skipped</div><div class='v'>{skipped}</div></div>\n</div>\n<table>\n  <thead>\n    <tr><th>Test</th><th>Status</th><th>Duration</th><th>Error</th></tr>\n  </thead>\n  <tbody>\n    {string.Join("\n", rows)}\n  </tbody>\n</table>\n</body>\n</html>";

                File.WriteAllText(ReportHtmlPath, html);
            }
            catch { }
        }

        private static int CountOccurrences(string text, string pattern)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern)) return 0;
            int count = 0, idx = 0;
            while ((idx = text.IndexOf(pattern, idx, StringComparison.OrdinalIgnoreCase)) >= 0) { count++; idx += pattern.Length; }
            return count;
        }

        private static int CountHtmlRowCount(string rowsHtml)
        {
            if (string.IsNullOrEmpty(rowsHtml)) return 0;
            int count = 0, idx = 0;
            while ((idx = rowsHtml.IndexOf("<tr", idx, StringComparison.OrdinalIgnoreCase)) >= 0) { count++; idx += 3; }
            return count;
        }
        }

    public class PruebasFuncionalesUIDesktop : IDisposable
    {
        private Process? appProc;
        private static int createdHuespedId = 0;
        private static string? createdHuespedCorreo = null;
        private static string? createdHuespedNombreCompleto = null;
        private static int createdHabitacionNumero = 0;
        private static int createdReservacionId = 0;

        private string GetExePath()
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CapaPresentacion", "bin", "Debug", "net9.0-windows", "CapaPresentacion.exe"));
        }

        private void LaunchApp()
        {
            var exe = GetExePath();
            Assert.True(File.Exists(exe), $"Exe not found at {exe}");
            var psi = new ProcessStartInfo(exe) { UseShellExecute = true };
            appProc = Process.Start(psi);
            Assert.NotNull(appProc);
            for (int i = 0; i < 40 && (appProc.MainWindowHandle == IntPtr.Zero); i++)
            {
                Thread.Sleep(250);
                appProc.Refresh();
            }
            Assert.True(appProc.MainWindowHandle != IntPtr.Zero, "Main window did not appear.");
            SetForegroundWindow(appProc.MainWindowHandle);
            Thread.Sleep(300);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        private void CloseApp()
        {
            try
            {
                if (appProc != null && !appProc.HasExited)
                {
                    appProc.CloseMainWindow();
                    appProc.WaitForExit(2000);
                    if (!appProc.HasExited) appProc.Kill(true);
                }
            }
            catch { }
            finally { appProc = null; }
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        private void ClickRelative(int x, int y)
        {
            if (appProc == null) throw new InvalidOperationException("App not started");
            var h = appProc.MainWindowHandle;
            if (h == IntPtr.Zero) throw new InvalidOperationException("Main window handle missing");
            GetWindowRect(h, out RECT r);
            int cx = r.Left + x;
            int cy = r.Top + y;
            SetCursorPos(cx, cy);
            Thread.Sleep(80);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(40);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(200);
        }

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        private void TypeText(string text)
        {
            System.Windows.Forms.SendKeys.SendWait(text);
            Thread.Sleep(120);
        }

        private void Press(string keys)
        {
            System.Windows.Forms.SendKeys.SendWait(keys);
            Thread.Sleep(120);
        }

        [Fact, Trait("Category", "UI")]
        public void ObtenerConexion()
        {
            var sw = Stopwatch.StartNew();
            bool passed = false;
            try
            {
                using var conexion = ConexionBD.obtenerConexion();
                conexion.Open();
                Assert.Equal(System.Data.ConnectionState.Open, conexion.State);
                conexion.Close();
                Assert.Equal(System.Data.ConnectionState.Closed, conexion.State);
                passed = true;
            }
            catch (Exception ex)
            {
                TestReporter.Append(nameof(ObtenerConexion), "FAILED", sw.Elapsed, ex.ToString());
                throw;
            }
            finally
            {
                if (passed) TestReporter.Append(nameof(ObtenerConexion), "PASSED", sw.Elapsed, null);
            }
        }

        [Fact, Trait("Category", "UI")]
        public void Huesped_Create()
        {
            var sw = Stopwatch.StartNew();
            bool passed = false;
            LaunchApp();
            try
            {
                ClickRelative(120, 80);
                ClickRelative(100, 180);
                ClickRelative(180, 240); TypeText("Manuel Antonio");
                ClickRelative(420, 240); TypeText("Ramirez");
                ClickRelative(620, 240); TypeText("Estrada");
                ClickRelative(180, 280); TypeText($"manuel.antonio{Guid.NewGuid().ToString().Substring(0,8)}@example.com");
                ClickRelative(420, 280); TypeText("6861404265");
                ClickRelative(620, 280); TypeText("password");
                ClickRelative(180, 340);
                Thread.Sleep(800);
                var huespedBLL = new HuespedBLL();
                var tabla = huespedBLL.ObtenerTodos();
                bool found = false;
                foreach (System.Data.DataRow row in tabla.Rows)
                {
                    if (row["numero_celular"].ToString() == "6861404265" && row["nombre"].ToString() == "Manuel Antonio") { found = true; break; }
                }
                Assert.True(found, "El huésped no fue encontrado tras el flujo UI.");
                passed = true;
            }
            catch (Exception ex)
            {
                TestReporter.Append(nameof(Huesped_Create), "FAILED", sw.Elapsed, ex.ToString());
                throw;
            }
            finally
            {
                CloseApp();
                if (passed) TestReporter.Append(nameof(Huesped_Create), "PASSED", sw.Elapsed, null);
            }
        }

        [Fact, Trait("Category", "UI")]
        public void Huesped_Delete()
        {
            var sw = Stopwatch.StartNew();
            bool passed = false;
            LaunchApp();
            try
            {
                ClickRelative(120, 80);
                Thread.Sleep(400);
                ClickRelative(200, 420);
                Thread.Sleep(300);
                Press("^(f)");
                TypeText("6861404265");
                Press("{ENTER}");
                Thread.Sleep(400);
                ClickRelative(480, 420);
                Thread.Sleep(400);
                var huespedBLL = new HuespedBLL();
                var tabla = huespedBLL.ObtenerTodos();
                bool exists = false;
                foreach (System.Data.DataRow row in tabla.Rows)
                {
                    if (row["numero_celular"].ToString() == "6861404265") { exists = true; break; }
                }
                Assert.False(exists, "El huésped todavía existe después de intentar eliminarlo vía UI.");
                passed = true;
            }
            catch (Exception ex)
            {
                TestReporter.Append(nameof(Huesped_Delete), "FAILED", sw.Elapsed, ex.ToString());
                throw;
            }
            finally
            {
                CloseApp();
                if (passed) TestReporter.Append(nameof(Huesped_Delete), "PASSED", sw.Elapsed, null);
            }
        }

        private void EnsureHuespedExists()
        {
            if (createdHuespedId > 0) return;
            var huespedBLL = new HuespedBLL();
            var huesped = new Huesped
            {
                nombre = "Manuel Antonio",
                apellido_1 = "Ramirez",
                apellido_2 = "Estrada",
                calle = "Churrubusco",
                colonia = "Condesa",
                codigo_postal = 21467,
                ciudad = "Tangamandapio",
                correo = $"manuel.antonio{Guid.NewGuid().ToString().Substring(0,8)}@example.com",
                numero_celular = "6861404265",
                contrasena = "password"
            };

            bool creado = huespedBLL.Agregar(huesped);
            Assert.True(creado, "No se pudo crear el huésped de prueba en EnsureHuespedExists.");

            var tabla = huespedBLL.ObtenerTodos();
            foreach (System.Data.DataRow row in tabla.Rows)
            {
                if (row["numero_celular"].ToString() == "6861404265" && row["nombre"].ToString() == "Manuel Antonio")
                {
                    createdHuespedId = Convert.ToInt32(row["huesped_id"]);
                    createdHuespedCorreo = row["correo"].ToString();
                    createdHuespedNombreCompleto = string.Concat(row["nombre"].ToString().Trim(), " ", row["apellido_1"].ToString().Trim(), " ", row["apellido_2"].ToString().Trim()).Trim();
                    break;
                }
            }
            Assert.True(createdHuespedId > 0, "EnsureHuespedExists no capturó el ID del huésped creado.");
        }

        private void EnsureHabitacionExists()
        {
            if (createdHabitacionNumero > 0) return;
            var habitacionBLL = new HabitacionBLL();
            var rnd = new Random();
            int numero = rnd.Next(1000, 9999);
            var habitacion = new Habitacion
            {
                numero_habitacion = numero,
                tipo_habitacion = "Doble",
                piso = 2,
                estatus = "Disponible"
            };

            bool creado = habitacionBLL.Agregar(habitacion);
            Assert.True(creado, "No se pudo crear la habitación de prueba en EnsureHabitacionExists.");
            createdHabitacionNumero = numero;
        }

        private void EnsureReservacionExists()
        {
            if (createdReservacionId > 0) return;
            EnsureHuespedExists();

            var reservacionBLL = new ReservacionBLL();
            var reservacion = new CapaEntidad.Reservacion
            {
                estatus = "Confirmada",
                fecha_entrada = DateTime.Today.AddDays(7),
                fecha_salida = DateTime.Today.AddDays(10),
                nombre_huesped = createdHuespedNombreCompleto,
                numero_personas = 2
            };

            bool creado = reservacionBLL.Agregar(reservacion);
            Assert.True(creado, "No se pudo crear la reservación de prueba en EnsureReservacionExists.");

            var tabla = reservacionBLL.ObtenerTodos();
            foreach (System.Data.DataRow row in tabla.Rows)
            {
                if (row["nombre_huesped"].ToString().Trim() == createdHuespedNombreCompleto &&
                    Convert.ToDateTime(row["fecha_entrada"]).Date == DateTime.Today.AddDays(7) &&
                    Convert.ToInt32(row["numero_personas"]) == 2)
                {
                    createdReservacionId = Convert.ToInt32(row["reservacion_id"]);
                    break;
                }
            }
            Assert.True(createdReservacionId > 0, "EnsureReservacionExists no capturó el ID de la reservación creada.");
        }

        [Fact, Trait("Category", "UI")] 
        public void Huesped_Read()
        {
            var sw = Stopwatch.StartNew(); bool passed = false; LaunchApp();
            try
            {
                var huespedBLL = new HuespedBLL();
                var tabla = huespedBLL.ObtenerTodos();
                Assert.NotNull(tabla);

                Huesped encontrado = null;
                foreach (System.Data.DataRow row in tabla.Rows)
                {
                    if (row["nombre"].ToString() == "Manuel Antonio" &&
                        row["apellido_1"].ToString() == "Ramirez" &&
                        row["apellido_2"].ToString() == "Estrada" &&
                        row["numero_celular"].ToString() == "6861404265")
                    {
                        encontrado = new Huesped
                        {
                            huesped_id = Convert.ToInt32(row["huesped_id"]),
                            nombre = row["nombre"].ToString(),
                            apellido_1 = row["apellido_1"].ToString()
                        };
                        break;
                    }
                }

                Assert.NotNull(encontrado);
                Assert.Equal("Manuel Antonio", encontrado.nombre);
                passed = true;
            }
            catch (Exception ex) { TestReporter.Append(nameof(Huesped_Read), "FAILED", sw.Elapsed, ex.ToString()); throw; }
            finally { CloseApp(); if (passed) TestReporter.Append(nameof(Huesped_Read), "PASSED", sw.Elapsed, null); }
        }

        [Fact, Trait("Category", "UI")]
        public void Huesped_Update()
        {
            var sw = Stopwatch.StartNew(); bool passed = false; LaunchApp();
            try
            {
                EnsureHuespedExists();
                var huespedBLL = new HuespedBLL();
                var existente = huespedBLL.Obtener(createdHuespedId);
                Assert.NotNull(existente);

                existente.ciudad = "Ensenada";
                existente.contrasena = "password123456";

                bool actualizado = huespedBLL.Actualizar(existente);
                Assert.True(actualizado, "La actualización debería retornar true.");

                var obtenido = huespedBLL.Obtener(createdHuespedId);
                Assert.NotNull(obtenido);
                Assert.Equal("Ensenada", obtenido.ciudad);
                passed = true;
            }
            catch (Exception ex) { TestReporter.Append(nameof(Huesped_Update), "FAILED", sw.Elapsed, ex.ToString()); throw; }
            finally { CloseApp(); if (passed) TestReporter.Append(nameof(Huesped_Update), "PASSED", sw.Elapsed, null); }
        }

        [Fact, Trait("Category", "UI")]
        public void Habitaciones_Create()
        {
            var sw = Stopwatch.StartNew(); bool passed = false; LaunchApp();
            try
            {
                var habitacionBLL = new HabitacionBLL();
                var rnd = new Random();
                int numero = rnd.Next(1000, 9999);
                var habitacion = new Habitacion
                {
                    numero_habitacion = numero,
                    tipo_habitacion = "Doble",
                    piso = 2,
                    estatus = "Disponible"
                };

                bool creado = habitacionBLL.Agregar(habitacion);
                Assert.True(creado, "La creación de la habitación debería retornar true.");
                createdHabitacionNumero = numero;
                passed = true;
            }
            catch (Exception ex) { TestReporter.Append(nameof(Habitaciones_Create), "FAILED", sw.Elapsed, ex.ToString()); throw; }
            finally { CloseApp(); if (passed) TestReporter.Append(nameof(Habitaciones_Create), "PASSED", sw.Elapsed, null); }
        }

        [Fact, Trait("Category", "UI")]
        public void Habitaciones_Read()
        {
            var sw = Stopwatch.StartNew(); bool passed = false; LaunchApp();
            try
            {
                var habitacionBLL = new HabitacionBLL();
                var tabla = habitacionBLL.ObtenerTodos();
                Assert.NotNull(tabla);

                int numeroEncontrado = 0;
                foreach (System.Data.DataRow row in tabla.Rows)
                {
                    if (row["tipo_habitacion"].ToString() == "Doble" &&
                        Convert.ToInt32(row["piso"]) == 2 &&
                        row["estatus"].ToString() == "Disponible")
                    {
                        numeroEncontrado = Convert.ToInt32(row["numero_habitacion"]);
                        break;
                    }
                }

                Assert.True(numeroEncontrado > 0, "No se encontró la habitación creada anteriormente.");

                var habitacion = habitacionBLL.Obtener(numeroEncontrado);
                Assert.NotNull(habitacion);
                Assert.Equal("Doble", habitacion.tipo_habitacion);
                passed = true;
            }
            catch (Exception ex) { TestReporter.Append(nameof(Habitaciones_Read), "FAILED", sw.Elapsed, ex.ToString()); throw; }
            finally { CloseApp(); if (passed) TestReporter.Append(nameof(Habitaciones_Read), "PASSED", sw.Elapsed, null); }
        }

        [Fact, Trait("Category", "UI")]
        public void Habitaciones_Update()
        {
            var sw = Stopwatch.StartNew(); bool passed = false; LaunchApp();
            try
            {
                EnsureHabitacionExists();
                var habitacionBLL = new HabitacionBLL();
                var existente = habitacionBLL.Obtener(createdHabitacionNumero);
                Assert.NotNull(existente);

                existente.piso = 4;
                existente.estatus = "Ocupada";

                bool actualizado = habitacionBLL.Actualizar(existente);
                Assert.True(actualizado, "La actualización debería retornar true.");

                var obtenido = habitacionBLL.Obtener(createdHabitacionNumero);
                Assert.NotNull(obtenido);
                Assert.Equal(4, obtenido.piso);
                Assert.Equal("Ocupada", obtenido.estatus);
                passed = true;
            }
            catch (Exception ex) { TestReporter.Append(nameof(Habitaciones_Update), "FAILED", sw.Elapsed, ex.ToString()); throw; }
            finally { CloseApp(); if (passed) TestReporter.Append(nameof(Habitaciones_Update), "PASSED", sw.Elapsed, null); }
        }

        [Fact, Trait("Category", "UI")]
        public void Reservaciones_Create()
        {
            var sw = Stopwatch.StartNew(); bool passed = false; LaunchApp();
            try
            {
                EnsureHuespedExists();
                var reservacionBLL = new ReservacionBLL();

                var reservacion = new CapaEntidad.Reservacion
                {
                    estatus = "Confirmada",
                    fecha_entrada = DateTime.Today.AddDays(7),
                    fecha_salida = DateTime.Today.AddDays(10),
                    nombre_huesped = createdHuespedNombreCompleto,
                    numero_personas = 2
                };

                bool creado = reservacionBLL.Agregar(reservacion);
                Assert.True(creado, "La creación de la reservación debería retornar true.");

                var tabla = reservacionBLL.ObtenerTodos();
                foreach (System.Data.DataRow row in tabla.Rows)
                {
                    if (createdHuespedNombreCompleto != null && row["nombre_huesped"].ToString().Trim() == createdHuespedNombreCompleto &&
                        Convert.ToDateTime(row["fecha_entrada"]).Date == DateTime.Today.AddDays(7) &&
                        Convert.ToInt32(row["numero_personas"]) == 2)
                    {
                        createdReservacionId = Convert.ToInt32(row["reservacion_id"]);
                        break;
                    }
                }
                Assert.True(createdReservacionId > 0, "No se pudo capturar el ID de la reservación creada.");
                passed = true;
            }
            catch (Exception ex) { TestReporter.Append(nameof(Reservaciones_Create), "FAILED", sw.Elapsed, ex.ToString()); throw; }
            finally { CloseApp(); if (passed) TestReporter.Append(nameof(Reservaciones_Create), "PASSED", sw.Elapsed, null); }
        }

        [Fact, Trait("Category", "UI")]
        public void Reservaciones_Read()
        {
            var sw = Stopwatch.StartNew(); bool passed = false; LaunchApp();
            try
            {
                EnsureReservacionExists();
                var reservacionBLL = new ReservacionBLL();
                int idEncontrado = createdReservacionId;
                if (idEncontrado == 0)
                {
                    var tabla = reservacionBLL.ObtenerTodos();
                    Assert.NotNull(tabla);
                    foreach (System.Data.DataRow row in tabla.Rows)
                    {
                        if (createdHuespedNombreCompleto != null && row["nombre_huesped"].ToString().Trim() == createdHuespedNombreCompleto &&
                            row["estatus"].ToString() == "Confirmada" &&
                            row["numero_personas"] != DBNull.Value &&
                            Convert.ToInt32(row["numero_personas"]) == 2)
                        {
                            idEncontrado = Convert.ToInt32(row["reservacion_id"]);
                            break;
                        }
                    }
                }

                Assert.True(idEncontrado > 0, "No se encontró la reservación creada anteriormente.");

                var reserv = reservacionBLL.Obtener(idEncontrado);
                Assert.NotNull(reserv);
                Assert.Equal(createdHuespedNombreCompleto, reserv.nombre_huesped);
                passed = true;
            }
            catch (Exception ex) { TestReporter.Append(nameof(Reservaciones_Read), "FAILED", sw.Elapsed, ex.ToString()); throw; }
            finally { CloseApp(); if (passed) TestReporter.Append(nameof(Reservaciones_Read), "PASSED", sw.Elapsed, null); }
        }

        [Fact, Trait("Category", "UI")]
        public void Reservaciones_Update()
        {
            var sw = Stopwatch.StartNew(); bool passed = false; LaunchApp();
            try
            {
                var reservacionBLL = new ReservacionBLL();
                EnsureReservacionExists();
                int idEncontrado = createdReservacionId;
                Assert.True(idEncontrado > 0, "No existe reservación capturada para actualizar.");

                var existente = reservacionBLL.Obtener(idEncontrado);
                Assert.NotNull(existente);

                existente.numero_personas = 5;

                bool actualizado = reservacionBLL.Actualizar(existente);
                Assert.True(actualizado, "La actualización debería retornar true.");

                var obtenido = reservacionBLL.Obtener(idEncontrado);
                Assert.NotNull(obtenido);
                Assert.Equal(5, obtenido.numero_personas);
                passed = true;
            }
            catch (Exception ex) { TestReporter.Append(nameof(Reservaciones_Update), "FAILED", sw.Elapsed, ex.ToString()); throw; }
            finally { CloseApp(); if (passed) TestReporter.Append(nameof(Reservaciones_Update), "PASSED", sw.Elapsed, null); }
        }

        [Fact, Trait("Category", "UI")]
        public void Reservaciones_Delete()
        {
            var sw = Stopwatch.StartNew(); bool passed = false; LaunchApp();
            try
            {
                EnsureReservacionExists();
                var reservacionBLL = new ReservacionBLL();
                int idAEliminar = createdReservacionId;
                if (idAEliminar == 0)
                {
                    var tabla = reservacionBLL.ObtenerTodos();
                    Assert.NotNull(tabla);
                    foreach (System.Data.DataRow row in tabla.Rows)
                    {
                        if (createdHuespedNombreCompleto != null && row["nombre_huesped"].ToString().Trim() == createdHuespedNombreCompleto &&
                            row["estatus"].ToString() == "Confirmada")
                        {
                            int id = Convert.ToInt32(row["reservacion_id"]);
                            if (id > idAEliminar) idAEliminar = id;
                        }
                    }
                }

                Assert.True(idAEliminar > 0, "No se encontró una reservación de prueba para eliminar.");

                bool eliminado = reservacionBLL.Eliminar(idAEliminar);
                Assert.True(eliminado, "La eliminación debería retornar true.");

                var obtenido = reservacionBLL.Obtener(idAEliminar);
                Assert.NotNull(obtenido);
                Assert.Equal(0, obtenido.reservacion_id);

                if (createdReservacionId == idAEliminar) createdReservacionId = 0;
                passed = true;
            }
            catch (Exception ex) { TestReporter.Append(nameof(Reservaciones_Delete), "FAILED", sw.Elapsed, ex.ToString()); throw; }
            finally { CloseApp(); if (passed) TestReporter.Append(nameof(Reservaciones_Delete), "PASSED", sw.Elapsed, null); }
        }

        [Fact, Trait("Category", "UI")]
        public void Habitaciones_Delete()
        {
            var sw = Stopwatch.StartNew(); bool passed = false; LaunchApp();
            try
            {
                var habitacionBLL = new HabitacionBLL();
                int numeroAEliminar = createdHabitacionNumero;
                if (numeroAEliminar == 0)
                {
                    var tabla = habitacionBLL.ObtenerTodos();
                    Assert.NotNull(tabla);
                    foreach (System.Data.DataRow row in tabla.Rows)
                    {
                        if (row["tipo_habitacion"].ToString() == "Doble" &&
                            Convert.ToInt32(row["piso"]) == 4 &&
                            row["estatus"].ToString() == "Ocupada")
                        {
                            numeroAEliminar = Convert.ToInt32(row["numero_habitacion"]);
                            break;
                        }
                    }
                }

                Assert.True(numeroAEliminar > 0, "No se encontró una habitación de prueba para eliminar.");

                bool eliminado = habitacionBLL.Eliminar(numeroAEliminar);
                Assert.True(eliminado, "La eliminación debería retornar true.");

                var obtenido = habitacionBLL.Obtener(numeroAEliminar);
                Assert.NotNull(obtenido);
                Assert.Equal(0, obtenido.numero_habitacion);

                if (createdHabitacionNumero == numeroAEliminar) createdHabitacionNumero = 0;
                passed = true;
            }
            catch (Exception ex) { TestReporter.Append(nameof(Habitaciones_Delete), "FAILED", sw.Elapsed, ex.ToString()); throw; }
            finally { CloseApp(); if (passed) TestReporter.Append(nameof(Habitaciones_Delete), "PASSED", sw.Elapsed, null); }
        }

        public void Dispose() { CloseApp(); }
    }
}
