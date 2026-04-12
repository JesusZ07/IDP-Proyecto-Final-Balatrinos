using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CapaEntidad;
using CapaNegocios;

namespace HotelProyecto.Controllers
{
    public class CuentasController : Controller
    {
        HuespedBLL huespedBLL = new HuespedBLL();

        // Vista de inicio de sesión (GET)
        public IActionResult InicioSesion()
        {
            return View();
        }

        // Procesar inicio de sesión (POST)
        [HttpPost]
        public IActionResult InicioSesion(string correo, string contrasena)
        {
            Huesped huesped = huespedBLL.ValidarCredenciales(correo, contrasena);

            if (huesped != null) // Si el huésped existe
            {
                CookieOptions opciones = new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMonths(1),
                    IsEssential = true
                };
                Response.Cookies.Append("UsuarioLogueado", "1", opciones);
                Response.Cookies.Append("UsuarioNombre", huesped.nombre ?? string.Empty, opciones);
                Response.Cookies.Append("UsuarioCorreo", huesped.correo ?? string.Empty, opciones);

                return RedirectToAction("Index", "Home"); // Acción "Index" del controlador "Home"

            }
            else
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }
        }

        [HttpGet] 
        public IActionResult Registro()
        { 
            return View();
        }


        [HttpPost]
        public IActionResult Registro(Huesped huesped, string contrasena)
        {
            if (!ModelState.IsValid)
            {
                return View(huesped);
            }
            bool exito = huespedBLL.Agregar(huesped);
            if (exito)
            {
                CookieOptions opciones = new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMonths(1),
                    IsEssential = true
                };
                Response.Cookies.Append("UsuarioLogueado", "1", opciones);
                Response.Cookies.Append("UsuarioNombre", huesped.nombre ?? string.Empty, opciones);
                Response.Cookies.Append("UsuarioCorreo", huesped.correo ?? string.Empty, opciones);
                ViewBag.MensajeExito = "¡Registro exitoso!";
                ModelState.Clear();
                // Limpia el formulario después del registro exitoso
                return View();
            }
            else
            {
                ViewBag.Error = "Ocurrió un problema al registrar el huésped. Intenta de nuevo."; return View(huesped);
            }
        }

        public IActionResult CerrarSesion()
        {
            Response.Cookies.Delete("UsuarioLogueado");
            Response.Cookies.Delete("UsuarioNombre");
            Response.Cookies.Delete("UsuarioCorreo");

            return RedirectToAction("InicioSesion");
        }


        [HttpGet]
        public IActionResult EditarPerfil()
        {
            if (Request.Cookies.ContainsKey("UsuarioCorreo"))
            {
                string correo = Request.Cookies["UsuarioCorreo"];
                Huesped huesped = huespedBLL.ObtenerPorCorreo(correo); // Usa el correo
                return View(huesped);
            }

            return RedirectToAction("InicioSesion");
        }

        [HttpPost]
        public IActionResult EditarPerfil(Huesped huesped)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Hubo un problema al actualizar el perfil.";
                return View(huesped);
            }

            Huesped huespedExistente = huespedBLL.Obtener(huesped.huesped_id);

            if (huespedExistente == null)
            {
                ViewBag.Error = "El huésped no existe.";
                return View(huesped);
            }

            huespedExistente.nombre = huesped.nombre;
            huespedExistente.apellido_1 = huesped.apellido_1;
            huespedExistente.apellido_2 = huesped.apellido_2;
            huespedExistente.calle = huesped.calle;
            huespedExistente.colonia = huesped.colonia;
            huespedExistente.codigo_postal = huesped.codigo_postal;
            huespedExistente.ciudad = huesped.ciudad;
            huespedExistente.correo = huesped.correo;
            huespedExistente.numero_celular = huesped.numero_celular;
            huespedExistente.contrasena = huesped.contrasena;

            bool exito = huespedBLL.Actualizar(huespedExistente);

            if (exito)
            {
                CookieOptions opciones = new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMonths(1),
                    IsEssential = true
                };
                Response.Cookies.Append("UsuarioLogueado", "1", opciones);
                Response.Cookies.Append("UsuarioNombre", huespedExistente.nombre ?? string.Empty, opciones);
                Response.Cookies.Append("UsuarioCorreo", huespedExistente.correo ?? string.Empty, opciones);

                ViewBag.MensajeExito = "¡Perfil actualizado correctamente!";
            }
            else
            {
                ViewBag.Error = "No se pudo actualizar el perfil.";
            }

            return View(huespedExistente);
        }




    }
}
