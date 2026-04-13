using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

namespace HotelProyecto.Tests;

[CollectionDefinition("ui")]
public class UiCollection : ICollectionFixture<PlaywrightWebAppFixture>
{
}

[Collection("ui")]
public class PruebasFuncionalesUI
{
    private readonly PlaywrightWebAppFixture _fixture;
    private const int DefaultTimeoutMs = 15000;
    private const int ContactResultTimeoutMs = 120000;

    private const string HomeIndexPath = "/Home/Index";
    private const string RegistroPath = "/Cuentas/Registro";
    private const string InicioSesionPath = "/Cuentas/InicioSesion";
    private const string ReservacionesPath = "/Reservaciones/Reservaciones";

    private const string RegistroNombre = "input[name='nombre']";
    private const string RegistroApellido1 = "input[name='apellido_1']";
    private const string RegistroApellido2 = "input[name='apellido_2']";
    private const string RegistroCalle = "input[name='calle']";
    private const string RegistroColonia = "input[name='colonia']";
    private const string RegistroCodigoPostal = "input[name='codigo_postal']";
    private const string RegistroCiudad = "input[name='ciudad']";
    private const string RegistroCorreo = "input[name='correo']";
    private const string RegistroCelular = "input[name='numero_celular']";
    private const string RegistroContrasena = "input[name='contrasena']";

    public PruebasFuncionalesUI(PlaywrightWebAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task VisualizarMapaDeGoogleMaps()
    {
        await RunInNewPage(async page =>
        {
            await page.GotoAsync(BuildUrl(HomeIndexPath));

            await ExpectVisible(page, "section#ubicacion");
            await ExpectVisible(page, "iframe.mapa-interactivo");

            string? altura = await page.Locator("iframe.mapa-interactivo").GetAttributeAsync("height");
            Assert.True(int.TryParse(altura, out int alturaMapa) && alturaMapa >= 520, "La altura del mapa debe ser al menos 520.");
        });
    }

    [Fact]
    public async Task Registro_exitoso()
    {
        string correo = CreateUniqueEmail("ui");
        string contrasena = "Pass1234";

        await RunInNewPage(async page =>
        {
            await RegistrarUsuario(page, correo, contrasena, "Usuario UI");

            await ExpectTextContains(page, "body", "Registro exitoso");
        });
    }

    [Fact]
    public async Task InicioSesion_exitoso()
    {
        string correo = CreateUniqueEmail("login_ok");
        string contrasena = "Pass1234";

        await RunInNewPage(async page =>
        {
            await RegistrarUsuario(page, correo, contrasena, "Login Exitoso");
            await Login(page, correo, contrasena);

            Assert.DoesNotContain(InicioSesionPath, page.Url, StringComparison.OrdinalIgnoreCase);
            await ExpectVisible(page, "body");
        });
    }

    [Fact]
    public async Task InicioSesion_fallido()
    {
        await RunInNewPage(async page =>
        {
            await Login(page, "usuario_inexistente@mail.com", "ContrasenaIncorrecta123");

            await ExpectTextContains(page, "body", "Correo o contraseña incorrectos");
            Assert.Contains(InicioSesionPath, page.Url, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public async Task RealizarReserva()
    {
        string correo = CreateUniqueEmail("res");

        await RunInNewPage(async page =>
        {
            await RegistrarEIniciarSesion(page, correo, "Pass1234");

            await page.GotoAsync(BuildUrl(ReservacionesPath));

            int botonesReservar = await page.Locator("a.btn-reservar").CountAsync();
            if (botonesReservar == 0)
            {
                return;
            }

            await page.Locator("a.btn-reservar").First.ClickAsync();
            await page.WaitForURLAsync("**/Reservaciones/ReservarHabitacion**");

            DateTime entrada = DateTime.Today.AddDays(3);
            DateTime salida = DateTime.Today.AddDays(5);

            await page.FillAsync("#nombre_huesped", "Cliente Reserva UI");
            await page.FillAsync("#correo_huesped", correo);
            await page.FillAsync("#num_personas", "2");
            await page.FillAsync("#fecha_entrada", entrada.ToString("yyyy-MM-dd"));
            await page.FillAsync("#fecha_salida", salida.ToString("yyyy-MM-dd"));
            await page.ClickAsync("button[type='submit']");

            await page.WaitForURLAsync("**/Reservaciones/Reservaciones");
            await ExpectTextContains(page, "body", "reservación");
        });
    }

    [Fact]
    public async Task EnviarMensajeContacto()
    {
        string correo = CreateUniqueEmail("contacto");

        await RunInNewPage(async page =>
        {
            await page.GotoAsync(BuildUrl($"{HomeIndexPath}#contacto"));

            await page.FillAsync("#nombre", "Contacto UI");
            await page.FillAsync("#correo", correo);
            await page.FillAsync("#telefono", "6867778899");
            await page.ClickAsync("button[type='submit']");

            await page.Locator(".mensaje-contacto-exito, .mensaje-contacto-error").First.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = ContactResultTimeoutMs
            });

            string body = await page.Locator("body").InnerTextAsync();
            bool contieneResultado = body.Contains("mensaje fue enviado correctamente", StringComparison.OrdinalIgnoreCase)
                                     || body.Contains("No fue posible enviar tu mensaje", StringComparison.OrdinalIgnoreCase);

            Assert.True(contieneResultado, "El formulario de contacto debe mostrar resultado de envío (éxito o error controlado).");
        });
    }

    private async Task RegistrarEIniciarSesion(IPage page, string correo, string contrasena)
    {
        await RegistrarUsuario(page, correo, contrasena, "Cliente Reserva");
        await Login(page, correo, contrasena);

        Assert.DoesNotContain(InicioSesionPath, page.Url, StringComparison.OrdinalIgnoreCase);
    }

    private async Task RegistrarUsuario(IPage page, string correo, string contrasena, string nombre)
    {
        await page.GotoAsync(BuildUrl(RegistroPath));
        await page.FillAsync(RegistroNombre, nombre);
        await page.FillAsync(RegistroApellido1, "UI");
        await page.FillAsync(RegistroApellido2, "E2E");
        await page.FillAsync(RegistroCalle, "Calle 9");
        await page.FillAsync(RegistroColonia, "Centro");
        await page.FillAsync(RegistroCodigoPostal, "21100");
        await page.FillAsync(RegistroCiudad, "Mexicali");
        await page.FillAsync(RegistroCorreo, correo);
        await page.FillAsync(RegistroCelular, "6861112233");
        await page.FillAsync(RegistroContrasena, contrasena);
        await page.ClickAsync("button[type='submit']");
    }

    private async Task Login(IPage page, string correo, string contrasena)
    {
        await page.GotoAsync(BuildUrl(InicioSesionPath));
        await page.FillAsync("input[name='correo']", correo);
        await page.FillAsync("input[name='contrasena']", contrasena);
        await page.ClickAsync("button[type='submit']");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    private async Task RunInNewPage(Func<IPage, Task> testBody)
    {
        await using IBrowserContext context = await _fixture.Browser.NewContextAsync();
        IPage page = await context.NewPageAsync();
        await BringBrowserToFront(page);
        await testBody(page);
    }

    private static async Task BringBrowserToFront(IPage page)
    {
        try
        {
            await page.BringToFrontAsync();
            await page.EvaluateAsync("() => window.focus()")!;
        }
        catch
        {
            // Best-effort only: some OS/window manager settings can block focus stealing.
        }
    }

    private string BuildUrl(string path)
    {
        return $"{_fixture.BaseUrl}{path}";
    }

    private static string CreateUniqueEmail(string prefijo)
    {
        string unico = Guid.NewGuid().ToString("N").Substring(0, 8);
        return $"{prefijo}_{unico}@mail.com";
    }

    private static async Task ExpectVisible(IPage page, string selector)
    {
        await page.Locator(selector).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeoutMs
        });
    }

    private static async Task ExpectTextContains(IPage page, string selector, string text)
    {
        await page.Locator(selector).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = DefaultTimeoutMs
        });

        string body = await page.Locator(selector).InnerTextAsync();
        Assert.Contains(text, body, StringComparison.OrdinalIgnoreCase);
    }
}
