using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using System.Runtime.InteropServices;
using Xunit;

namespace HotelProyecto.Tests
{
    public class PruebasFuncionalesUIDesktop : IDisposable
    {
        private Application? app;
        private UIA3Automation? automation;

        private string GetExePath()
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "CapaPresentacion", "bin", "Debug", "net9.0-windows", "CapaPresentacion.exe"));
        }

        private Window LaunchAndGetMain()
        {
            var exePath = GetExePath();
            Assert.True(File.Exists(exePath), $"Exe not found at {exePath}");
            app = Application.Launch(exePath);
            automation = new UIA3Automation();
            var main = app.GetMainWindow(automation, TimeSpan.FromSeconds(10));
            Assert.NotNull(main);
            try { main.Focus(); } catch { }
            try
            {
                var proc = Process.GetProcessById(app.ProcessId);
                var h = proc.MainWindowHandle;
                if (h != IntPtr.Zero)
                {
                    ShowWindow(h, SW_RESTORE);
                    SetForegroundWindow(h);
                    try
                    {
                        SetWindowPos(h, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                        Thread.Sleep(100);
                        SetWindowPos(h, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                    }
                    catch { }
                }
            }
            catch { }
            return main;
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_SHOWWINDOW = 0x0040;

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private bool TryInvokeButton(Button? btn, int attempts = 4, int delayMs = 300)
        {
            if (btn == null) return false;
            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    if (!btn.IsEnabled)
                    {
                        Thread.Sleep(delayMs);
                        continue;
                    }
                    btn.Invoke();
                    return true;
                }
                catch (COMException)
                {
                    Thread.Sleep(delayMs);
                }
                catch (Exception)
                {
                    Thread.Sleep(delayMs);
                }
            }
            return false;
        }

        private bool TryInvokeElement(AutomationElement? elem, int attempts = 4, int delayMs = 300)
        {
            if (elem == null) return false;
            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    if (elem.Patterns.Invoke.IsSupported)
                    {
                        elem.Patterns.Invoke.Pattern.Invoke();
                        return true;
                    }
                    var btn = elem.AsButton();
                    if (btn != null && btn.IsEnabled)
                    {
                        btn.Invoke();
                        return true;
                    }
                    Thread.Sleep(delayMs);
                }
                catch (COMException)
                {
                    Thread.Sleep(delayMs);
                }
                catch (Exception)
                {
                    Thread.Sleep(delayMs);
                }
            }
            return false;
        }

        private Button? FindButtonByPossibleTexts(AutomationElement root, params string[] texts)
        {
            foreach (var t in texts)
            {
                var btn = root.FindFirstDescendant(cf => cf.ByText(t))?.AsButton();
                if (btn != null && btn.IsEnabled) return btn;
            }
            var candidates = root.FindAllDescendants(cf => cf.ByControlType(ControlType.Button)).Select(e => e.AsButton()).Where(b => b != null).ToArray();
            foreach (var b in candidates)
            {
                var name = b.Name ?? string.Empty;
                foreach (var t in texts)
                {
                    if (name.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0) return b;
                }
            }
            return null;
        }

        private bool WaitUntil(Func<bool> condition, TimeSpan timeout)
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < timeout)
            {
                try
                {
                    if (condition()) return true;
                }
                catch { }
                Thread.Sleep(50);
            }
            return false;
        }

        private TextBox? FindEditByLabel(AutomationElement root, params string[] labelTexts)
        {
            foreach (var label in labelTexts)
            {
                var lbl = root.FindFirstDescendant(cf => cf.ByText(label)) ?? root.FindFirstDescendant(cf => cf.ByName(label));
                if (lbl == null) continue;

                // 1) Prefer edits within the same parent container
                var parent = lbl.Parent;
                if (parent != null)
                {
                    var editsInParent = parent.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit)).Select(e => e.AsTextBox()).Where(t => t != null).ToArray();
                    var chosen = ChooseNearestEdit(lbl, editsInParent);
                    if (chosen != null) return chosen;
                }

                // 2) Walk ancestors and search their descendants
                var anc = lbl.Parent;
                while (anc != null)
                {
                    var editsInAncestor = anc.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit)).Select(e => e.AsTextBox()).Where(t => t != null).ToArray();
                    var chosen = ChooseNearestEdit(lbl, editsInAncestor);
                    if (chosen != null) return chosen;
                    anc = anc.Parent;
                }

                // 3) fallback to global search under root
                var allEdits = root.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit)).Select(e => e.AsTextBox()).Where(t => t != null).ToArray();
                var fallback = ChooseNearestEdit(lbl, allEdits);
                if (fallback != null) return fallback;
            }
            return null;
        }

        private TextBox? ChooseNearestEdit(AutomationElement lbl, TextBox[] edits)
        {
            if (edits == null || edits.Length == 0) return null;
            if (edits.Length == 1) return edits[0];
            var lblRect = lbl.BoundingRectangle;
            var labelRight = (int)lblRect.Right;
            var labelCenterY = (int)(lblRect.Top + lblRect.Height / 2);
            TextBox? best = null;
            double bestScore = double.MaxValue;
            const double rowThreshold = 18.0;
            // tokens from label name for fuzzy matching
            var tokens = (lbl.Name ?? string.Empty).Split(new[] { ' ', '_', ':' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.ToLowerInvariant()).ToArray();
            foreach (var e in edits)
            {
                try
                {
                    var er = e.BoundingRectangle;
                    var centerX = (int)(er.Left + er.Width / 2);
                    var centerY = (int)(er.Top + er.Height / 2);
                    var vertDelta = Math.Abs(centerY - labelCenterY);
                    var horizDelta = Math.Abs(centerX - labelRight);

                    // prefer edits that are to the right of the label (centerX >= labelRight - smallOverlap)
                    bool toRight = centerX >= labelRight - 8;

                    // score: vertical delta prioritized, then horizontal; penalize edits left of label
                    double score = vertDelta * 1.0 + horizDelta * 0.5;
                    if (!toRight) score += 2000; // large penalty if edit is left of label
                    if (vertDelta > rowThreshold) score += 500; // de-prioritize very different rows

                    // boost score if edit's automation id or name contains label tokens
                    var name = string.Empty;
                    try { name = e.Name ?? string.Empty; } catch { }
                    var aid = string.Empty;
                    try { aid = e.AutomationId ?? string.Empty; } catch { }
                    var hay = (name + " " + aid).ToLowerInvariant();
                    foreach (var t in tokens)
                    {
                        if (!string.IsNullOrWhiteSpace(t) && hay.Contains(t))
                        {
                            score -= 400; // strong preference
                            break;
                        }
                    }

                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = e;
                    }
                }
                catch { }
            }
            return best;
        }

        private TextBox[] GetEditsRowMajor(AutomationElement root)
        {
            var edits = root.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit)).Select(e => e.AsTextBox()).Where(t => t != null).ToArray();
            var rects = edits.Select(e => new { e, Top = e.BoundingRectangle.Top, Left = e.BoundingRectangle.Left }).OrderBy(x => x.Top).ToArray();
            var rows = new System.Collections.Generic.List<System.Collections.Generic.List<AutomationElement>>();
            double? currentTop = null;
            const double rowThreshold = 12.0;
            foreach (var r in rects)
            {
                if (currentTop == null || Math.Abs(r.Top - currentTop.Value) > rowThreshold)
                {
                    rows.Add(new System.Collections.Generic.List<AutomationElement>());
                    currentTop = r.Top;
                }
                rows.Last().Add(r.e);
            }
            var ordered = new System.Collections.Generic.List<TextBox>();
            foreach (var row in rows)
            {
                var sortedRow = row.OrderBy(e => e.BoundingRectangle.Left).Select(e => e.AsTextBox()).Where(t => t != null);
                ordered.AddRange(sortedRow);
            }
            return ordered.ToArray();
        }

        

        [Fact(DisplayName = "Huesped_Create")]
        public void Huesped_Create()
        {
            var main = LaunchAndGetMain();

            
            var btnGuest = FindButtonByPossibleTexts(main, "Gestion de huespedes", "Gestión de huéspedes", "Gestion de huéspedes");
            if (btnGuest != null)
            {
                var rect = btnGuest.BoundingRectangle;
                Mouse.MoveTo(new System.Drawing.Point((int)(rect.Left + rect.Width / 2), (int)(rect.Top + rect.Height / 2)));
                Mouse.Click(MouseButton.Left);
            }
            else
            {
                var any = main.FindFirstDescendant(cf => cf.ByText("Huesped")) ?? main.FindFirstDescendant(cf => cf.ByText("Huésped")) ?? main.FindFirstDescendant(cf => cf.ByText("Huespedes")) ?? main.FindFirstDescendant(cf => cf.ByText("Huéspedes"));
                Assert.NotNull(any);
                var rect = any.BoundingRectangle;
                Mouse.MoveTo(new System.Drawing.Point((int)(rect.Left + rect.Width / 2), (int)(rect.Top + rect.Height / 2)));
                Mouse.Click(MouseButton.Left);
            }

            // wait for the Nuevo button to appear (active polling)
            Button? btnNuevo = null;
            WaitUntil(() =>
            {
                try
                {
                    var candidate = automation!.GetDesktop().FindFirstDescendant(cf => cf.ByText("Nuevo"));
                    if (candidate != null)
                    {
                        btnNuevo = candidate.AsButton();
                        if (btnNuevo != null && btnNuevo.IsEnabled) return true;
                    }
                    var allButtonEls = automation.GetDesktop().FindAllDescendants(cf => cf.ByControlType(ControlType.Button));
                    foreach (var el in allButtonEls)
                    {
                        try
                        {
                            var b = el.AsButton();
                            if (b == null) continue;
                            var name = string.Empty;
                            try { name = b.Name ?? string.Empty; } catch { continue; }
                            if (name.IndexOf("Nuevo", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                btnNuevo = b;
                                return true;
                            }
                        }
                        catch { continue; }
                    }
                    foreach (var w in app!.GetAllTopLevelWindows(automation!))
                    {
                        var foundBtn = w.FindFirstDescendant(cf => cf.ByText("Nuevo"));
                        if (foundBtn != null)
                        {
                            btnNuevo = foundBtn.AsButton();
                            if (btnNuevo != null && btnNuevo.IsEnabled) return true;
                        }
                    }
                }
                catch { }
                return false;
            }, TimeSpan.FromSeconds(20));
            Assert.NotNull(btnNuevo);
            // prefer the top-level guest management window as root for field lookups
            Window? guestWindow = null;
            WaitUntil(() =>
            {
                try
                {
                    guestWindow = app.GetAllTopLevelWindows(automation).FirstOrDefault(w =>
                        (w.Title ?? string.Empty).IndexOf("Gestión", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (w.Title ?? string.Empty).IndexOf("Huésped", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        w.FindFirstDescendant(cf => cf.ByText("Información de los huéspedes")) != null);
                    return guestWindow != null;
                }
                catch { return false; }
            }, TimeSpan.FromSeconds(6));
            var guestRoot = (AutomationElement?)(guestWindow ?? btnNuevo.Parent);
            var rectNuevo = btnNuevo.BoundingRectangle;
            Mouse.MoveTo(new System.Drawing.Point((int)(rectNuevo.Left + rectNuevo.Width / 2), (int)(rectNuevo.Top + rectNuevo.Height / 2)));
            Mouse.Click(MouseButton.Left);

            
            // wait for edit controls to appear
            WaitUntil(() =>
            {
                var foundEdits = guestRoot.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
                return foundEdits != null && foundEdits.Length >= 6;
            }, TimeSpan.FromSeconds(6));

            // prefer finding edits by their labels to map data correctly
            // Diagnostic dump to help map labels -> edits when mapping fails
            void DumpControls(AutomationElement r)
            {
                try
                {
                    Console.WriteLine("--- Diagnostic dump for guestRoot ---");
                    Console.WriteLine($"Root: {(r?.Name ?? "<null>")} Bounds: {r?.BoundingRectangle}");
                    var labels = r.FindAllDescendants(cf => cf.ByControlType(ControlType.Text));
                    Console.WriteLine($"Found labels: {labels.Length}");
                    for (int i = 0; i < labels.Length; i++)
                    {
                        try
                        {
                            var l = labels[i];
                            Console.WriteLine($"LBL[{i}] Name='{l.Name}' AutomationId='{l.AutomationId}' Bounds={l.BoundingRectangle}");
                        }
                        catch { }
                    }
                    var edits = r.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit));
                    Console.WriteLine($"Found edits: {edits.Length}");
                    for (int i = 0; i < edits.Length; i++)
                    {
                        try
                        {
                            var e = edits[i].AsTextBox();
                            Console.WriteLine($"EDT[{i}] Name='{e?.Name}' AutomationId='{edits[i].AutomationId}' Bounds={e?.BoundingRectangle} Text='{e?.Text}'");
                        }
                        catch { }
                    }
                    Console.WriteLine("--- End dump ---");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DumpControls failed: {ex.Message}");
                }
            }

            DumpControls(guestRoot);
            var nameEdit = FindEditByLabel(guestRoot, "Nombre", "Nombre:", "name");
            var apellido1Edit = FindEditByLabel(guestRoot, "Apellido paterno", "apellido_1", "Apellido Paterno");
            var apellido2Edit = FindEditByLabel(guestRoot, "Apellido materno", "apellido_2", "Apellido Materno");
            var correoEdit = FindEditByLabel(guestRoot, "Correo", "correo", "Correo electrónico", "Email");
            var calleEdit = FindEditByLabel(guestRoot, "Calle");
            var coloniaEdit = FindEditByLabel(guestRoot, "Colonia");
            var cpEdit = FindEditByLabel(guestRoot, "Código postal", "Codigo postal", "CP");
            var ciudadEdit = FindEditByLabel(guestRoot, "Ciudad");
            var celularEdit = FindEditByLabel(guestRoot, "Número de celular", "Numero de celular", "Número celular", "numero_celular");

            // fallback to positional mapping if label lookup failed - use row-major ordering
            var editsOrdered = GetEditsRowMajor(guestRoot);
            // UI layout has two rows: [ID, Nombre, Apellido paterno, Apellido materno, Correo]
            // and [Calle, Colonia, Código postal, Ciudad, Número de celular]
            if (editsOrdered.Length >= 10)
            {
                // map with ID present
                if (nameEdit == null) nameEdit = editsOrdered[1];
                if (apellido1Edit == null) apellido1Edit = editsOrdered[2];
                if (apellido2Edit == null) apellido2Edit = editsOrdered[3];
                if (correoEdit == null) correoEdit = editsOrdered[4];
                if (calleEdit == null) calleEdit = editsOrdered[5];
                if (coloniaEdit == null) coloniaEdit = editsOrdered[6];
                if (cpEdit == null) cpEdit = editsOrdered[7];
                if (ciudadEdit == null) ciudadEdit = editsOrdered[8];
                if (celularEdit == null) celularEdit = editsOrdered[9];
            }
            else if (editsOrdered.Length >= 9)
            {
                // no ID field present; assume name starts at index 0
                if (nameEdit == null) nameEdit = editsOrdered[0];
                if (apellido1Edit == null) apellido1Edit = editsOrdered[1];
                if (apellido2Edit == null) apellido2Edit = editsOrdered[2];
                if (correoEdit == null) correoEdit = editsOrdered[3];
                if (calleEdit == null) calleEdit = editsOrdered[4];
                if (coloniaEdit == null) coloniaEdit = editsOrdered[5];
                if (cpEdit == null) cpEdit = editsOrdered[6];
                if (ciudadEdit == null) ciudadEdit = editsOrdered[7];
                if (celularEdit == null) celularEdit = editsOrdered[8];
            }

            // definitive values requested by user
            var val_nombre = "Manuel Antonio";
            var val_apellido1 = "Ramirez";
            var val_apellido2 = "Estrada";
            var val_calle = "Churrubusco";
            var val_colonia = "Condesa";
            var val_cp = "21467";
            var val_ciudad = "Tangamandapio";
            var val_correo = "manuel.antonio@example.com";
            var val_celular = "6861404265";
            var val_contrasena = "password";

            // helper to set value robustly
            void SetValue(TextBox? tb, string v)
            {
                if (tb == null) return;
                try
                {
                    tb.Click();
                    tb.Text = v;
                }
                catch
                {
                    try { tb.Click(); } catch { }
                    try { Keyboard.Type(v); } catch { }
                }
            }

            SetValue(nameEdit, val_nombre);
            SetValue(apellido1Edit, val_apellido1);
            SetValue(apellido2Edit, val_apellido2);
            SetValue(calleEdit, val_calle);
            SetValue(coloniaEdit, val_colonia);
            SetValue(cpEdit, val_cp);
            SetValue(ciudadEdit, val_ciudad);
            SetValue(correoEdit, val_correo);
            SetValue(celularEdit, val_celular);

            // password field: try to find by label, else use positional fallback (last field)
            var passEdit = FindEditByLabel(guestRoot, "Contrasena", "Contraseña", "Password", "contrasena") ?? guestRoot.FindFirstDescendant(cf => cf.ByAutomationId("password"))?.AsTextBox();
            if (passEdit == null)
            {
                var allEd = GetEditsRowMajor(guestRoot);
                if (allEd.Length >= 10) passEdit = allEd[9];
                else if (allEd.Length > 0) passEdit = allEd.Last();
            }
            SetValue(passEdit, val_contrasena);

            // ensure any remaining edit controls are not empty (fill with placeholder)
            var allEditsFinal = GetEditsRowMajor(guestRoot);
            for (int i = 0; i < allEditsFinal.Length; i++)
            {
                try
                {
                    var tb = allEditsFinal[i];
                    var txt = tb.Text ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(txt))
                    {
                        // attempt to infer field by index if possible
                        if (i == 0 && string.IsNullOrWhiteSpace(tb.Text)) SetValue(tb, "0");
                        else SetValue(tb, "-");
                    }
                }
                catch { }
            }

            var btnGuardar = guestRoot.FindFirstDescendant(cf => cf.ByText("Guardar"))?.AsButton();
            Assert.NotNull(btnGuardar);

            // ensure required fields are filled before saving
            var required = new[] { nameEdit, apellido1Edit, apellido2Edit, correoEdit, calleEdit, coloniaEdit, cpEdit, ciudadEdit, celularEdit };
            foreach (var tb in required)
            {
                if (tb == null) continue;
                try
                {
                    var cur = tb.Text ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(cur))
                    {
                        tb.Click();
                        // fill sensible defaults per field by heuristics on automation id or placeholder
                        var label = tb.Name ?? string.Empty;
                        if (label.IndexOf("correo", StringComparison.OrdinalIgnoreCase) >= 0 || label.IndexOf("email", StringComparison.OrdinalIgnoreCase) >= 0)
                            tb.Text = "manuel.antonio@example.com";
                        else if (label.IndexOf("celular", StringComparison.OrdinalIgnoreCase) >= 0 || label.IndexOf("numero", StringComparison.OrdinalIgnoreCase) >= 0)
                            tb.Text = "6861404265";
                        else if (label.IndexOf("codigo", StringComparison.OrdinalIgnoreCase) >= 0 || label.IndexOf("cp", StringComparison.OrdinalIgnoreCase) >= 0)
                            tb.Text = "21467";
                        else if (label.IndexOf("calle", StringComparison.OrdinalIgnoreCase) >= 0)
                            tb.Text = "Av Reforma";
                        else if (label.IndexOf("colonia", StringComparison.OrdinalIgnoreCase) >= 0)
                            tb.Text = "Condesa";
                        else if (label.IndexOf("nombre", StringComparison.OrdinalIgnoreCase) >= 0)
                            tb.Text = "Manuel Antonio";
                        else if (label.IndexOf("apellido", StringComparison.OrdinalIgnoreCase) >= 0)
                            tb.Text = tb == apellido2Edit ? "Estrada" : "Ramirez";
                        else if (label.IndexOf("ciudad", StringComparison.OrdinalIgnoreCase) >= 0)
                            tb.Text = "Mexico";
                        else
                            tb.Text = "x";
                    }
                }
                catch
                {
                    try { tb.Click(); } catch { }
                    try { Keyboard.Type(" "); } catch { }
                }
            }

            // force-correct specific problematic fields by clicking relative to their labels
            try
            {
                var apEdit = FindEditByLabel(guestRoot, "Apellido paterno", "apellido_1", "Apellido Paterno");
                SetValue(apEdit, val_apellido1);
            }
            catch { }

            try
            {
                var correoEd = FindEditByLabel(guestRoot, "Correo", "correo", "Correo electrónico", "Email");
                SetValue(correoEd, val_correo);
            }
            catch { }
            var rectSave = btnGuardar.BoundingRectangle;
            Mouse.MoveTo(new System.Drawing.Point((int)(rectSave.Left + rectSave.Width / 2), (int)(rectSave.Top + rectSave.Height / 2)));
            Mouse.Click(MouseButton.Left);

            // allow UI to process the save action (wait for grid to update)
            WaitUntil(() => main.FindFirstDescendant(cf => cf.ByControlType(ControlType.DataGrid)) != null || main.FindFirstDescendant(cf => cf.ByControlType(ControlType.Table)) != null, TimeSpan.FromSeconds(6));

            // try to select the top row in the data grid to confirm the created guest
            var grid = main.FindFirstDescendant(cf => cf.ByControlType(ControlType.DataGrid)) ?? main.FindFirstDescendant(cf => cf.ByControlType(ControlType.Table));
            if (grid != null)
            {
                var firstRow = grid.FindFirstDescendant(cf => cf.ByControlType(ControlType.DataItem)) ?? grid.FindAllDescendants(cf => cf.ByControlType(ControlType.DataItem)).FirstOrDefault();
                if (firstRow != null)
                {
                    var rectRow = firstRow.BoundingRectangle;
                    Mouse.MoveTo(new System.Drawing.Point((int)(rectRow.Left + 10), (int)(rectRow.Top + rectRow.Height / 2)));
                    Mouse.Click(MouseButton.Left);
                }
            }

            var found = main.FindFirstDescendant(cf => cf.ByText("6861404265")) ?? main.FindFirstDescendant(cf => cf.ByText("manuel.antonio@example.com"));
            Assert.NotNull(found);

            app.Close();
            app = null;
        }

        public void Dispose()
        {
            try { automation?.Dispose(); } catch { }
            try { app?.Close(); } catch { }
        }
    }
}
