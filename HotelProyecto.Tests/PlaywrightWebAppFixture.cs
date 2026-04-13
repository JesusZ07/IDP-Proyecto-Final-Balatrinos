using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

namespace HotelProyecto.Tests;

public class PlaywrightWebAppFixture : IAsyncLifetime
{
    private Process? _webProcess;

    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;
    public string BaseUrl { get; } = "http://127.0.0.1:5199";

    public async Task InitializeAsync()
    {
        string root = FindSolutionRoot();
        string webProject = Path.Combine(root, "HotelProyecto", "HotelProyecto.csproj");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{webProject}\" --no-build --urls {BaseUrl}",
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        _webProcess = Process.Start(startInfo);
        if (_webProcess == null)
        {
            throw new InvalidOperationException("No se pudo iniciar la aplicación web para las pruebas de UI.");
        }

        await WaitForWebReady();

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        bool headless = string.Equals(
            Environment.GetEnvironmentVariable("UI_HEADLESS"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = headless,
            SlowMo = headless ? 0 : 150
        });
    }

    public async Task DisposeAsync()
    {
        if (Browser != null)
        {
            await Browser.CloseAsync();
        }

        Playwright?.Dispose();

        if (_webProcess != null && !_webProcess.HasExited)
        {
            _webProcess.Kill(entireProcessTree: true);
            _webProcess.Dispose();
        }
    }

    private async Task WaitForWebReady()
    {
        using HttpClient client = new HttpClient();

        for (int i = 0; i < 40; i++)
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync($"{BaseUrl}/Home/Index");
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch
            {
            }

            await Task.Delay(500);
        }

        string salida = _webProcess?.StandardOutput.ReadToEnd() ?? string.Empty;
        string errores = _webProcess?.StandardError.ReadToEnd() ?? string.Empty;
        throw new TimeoutException($"La aplicación web no inició a tiempo. Output: {salida} Error: {errores}");
    }

    private static string FindSolutionRoot()
    {
        string? current = AppContext.BaseDirectory;

        while (!string.IsNullOrWhiteSpace(current))
        {
            string slnPath = Path.Combine(current, "IDP-Proyecto-Final-Balatrinos.sln");
            if (File.Exists(slnPath))
            {
                return current;
            }

            DirectoryInfo? parent = Directory.GetParent(current);
            if (parent == null)
            {
                break;
            }

            current = parent.FullName;
        }

        throw new DirectoryNotFoundException("No se encontró la raíz de la solución para ejecutar pruebas UI.");
    }
}
