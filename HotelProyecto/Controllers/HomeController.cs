using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CapaNegocios;

namespace HotelProyecto.Controllers
{
    public class HomeController : Controller
    {
        private const string CorreoReceptorFijo = "jesus.zapata@uabc.edu.mx";

        private readonly HuespedBLL huespedBLL = new HuespedBLL();

        // GET: Home
        public IActionResult Index()
        {
            CargarDatosContactoUsuario();
            return View();
        }

        public IActionResult Contacto()
        {
            CargarDatosContactoUsuario();
            return View();
        }

        [HttpPost]
        public IActionResult EnviarContacto(string nombre, string correo, string telefono, string mensaje, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(correo))
            {
                TempData["ContactoError"] = "Completa al menos nombre y correo para poder contactarte.";
            }
            else
            {
                try
                {
                    EnviarCorreoContacto(nombre, correo, telefono, mensaje);
                    TempData["ContactoMensaje"] = "Gracias por contactarnos. Tu mensaje fue enviado correctamente.";
                }
                catch
                {
                    TempData["ContactoError"] = "No fue posible enviar tu mensaje en este momento. Intenta de nuevo.";
                }
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Contacto");
        }

        private void EnviarCorreoContacto(string nombre, string correo, string telefono, string mensaje)
        {
            var mail = new System.Net.Mail.MailMessage();
            mail.To.Add(CorreoReceptorFijo);
            mail.Subject = "Nuevo mensaje de contacto - Hotel Oasis";
            mail.Body = $@"
Nombre: {nombre}
Correo: {correo}
Teléfono: {telefono}

Mensaje:
{mensaje}";
            mail.IsBodyHtml = false;
            mail.From = new System.Net.Mail.MailAddress("zopedepatas123@gmail.com", "Hotel Oasis");

            var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("zopedepatas123@gmail.com", "xdce fcpw jrtv ezso"),
                EnableSsl = true
            };

            smtp.Send(mail);
        }

        private void CargarDatosContactoUsuario()
        {
            ViewBag.ContactoNombreCompleto = string.Empty;
            ViewBag.ContactoCorreo = string.Empty;
            ViewBag.ContactoTelefono = string.Empty;

            if (!Request.Cookies.ContainsKey("UsuarioCorreo"))
            {
                return;
            }

            string correo = Request.Cookies["UsuarioCorreo"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(correo))
            {
                return;
            }

            var huesped = huespedBLL.ObtenerPorCorreo(correo);
            if (huesped == null)
            {
                ViewBag.ContactoCorreo = correo;
                return;
            }

            string nombreCompleto = string.Join(" ", new[] { huesped.nombre, huesped.apellido_1, huesped.apellido_2 }
                .Where(parte => !string.IsNullOrWhiteSpace(parte))
                .Select(parte => parte.Trim()));

            ViewBag.ContactoNombreCompleto = nombreCompleto;
            ViewBag.ContactoCorreo = huesped.correo ?? correo;
            ViewBag.ContactoTelefono = huesped.numero_celular ?? string.Empty;
        }

    }
}