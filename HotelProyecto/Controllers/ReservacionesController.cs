using System;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CapaEntidad;
using CapaNegocios;

namespace HotelProyecto.Controllers
{
    public class ReservacionesController : Controller
    {
        private const string CorreoReceptorFijo = "jesus.zapata@uabc.edu.mx";

        private HabitacionBLL habitacionBLL = new HabitacionBLL();
        private ReservacionBLL reservacionBLL = new ReservacionBLL();

        // GET: /Reservaciones/
        public IActionResult Reservaciones()
        {
            var habitaciones = habitacionBLL.ObtenerTodos();

            var disponibles = habitaciones.AsEnumerable()
                .Where(row => row.Field<string>("estatus") == "Disponible");

            if (disponibles.Any())
            {
                DataTable dtDisponibles = disponibles.CopyToDataTable();
                return View(dtDisponibles);
            }
            else
            {
                return View(new DataTable());
            }
        }

        // GET: Reservaciones/ReservarHabitacion/5
        public IActionResult ReservarHabitacion(int numero_habitacion)
        {
            if (!Request.Cookies.ContainsKey("UsuarioCorreo"))
            {
                TempData["Error"] = "Debes iniciar sesión para reservar una habitación.";
                return RedirectToAction("Reservaciones");
            }

            var habitacion = habitacionBLL.Obtener(numero_habitacion);

            if (habitacion == null || habitacion.estatus != "Disponible")
                return RedirectToAction("Reservaciones");

            ViewBag.NumeroHabitacion = habitacion.numero_habitacion;
            ViewBag.TipoHabitacion = habitacion.tipo_habitacion;
            ViewBag.PrecioPorNoche = ObtenerPrecioPorTipo(habitacion.tipo_habitacion);

            return View();
        }

        [HttpPost]
        public IActionResult ConfirmarReserva(int numero_habitacion, string nombre_huesped, string correo_huesped, int num_personas, DateTime fecha_entrada, DateTime fecha_salida)
        {
            if (!Request.Cookies.ContainsKey("UsuarioCorreo"))
            {
                TempData["Error"] = "Debes iniciar sesión para confirmar la reservación.";
                return RedirectToAction("Reservaciones");
            }

            var habitacion = habitacionBLL.Obtener(numero_habitacion);

            if (habitacion == null || habitacion.estatus != "Disponible")
            {
                TempData["Error"] = "La habitación ya no está disponible.";
                return RedirectToAction("Reservaciones");
            }

            if (fecha_salida <= fecha_entrada)
            {
                ModelState.AddModelError("", "La fecha de salida debe ser posterior a la fecha de entrada.");
                ViewBag.NumeroHabitacion = habitacion.numero_habitacion;
                ViewBag.TipoHabitacion = habitacion.tipo_habitacion;
                ViewBag.PrecioPorNoche = ObtenerPrecioPorTipo(habitacion.tipo_habitacion);
                return View("ReservarHabitacion");
            }

            var reservacion = new Reservacion
            {
                estatus = "Confirmada",
                fecha_entrada = fecha_entrada,
                fecha_salida = fecha_salida,
                nombre_huesped = nombre_huesped,
                numero_personas = num_personas
            };

            bool resultado = reservacionBLL.Agregar(reservacion);

            if (resultado)
            {
                habitacion.estatus = "Ocupado";
                bool actualizoHabitacion = habitacionBLL.Actualizar(habitacion);

                if (!actualizoHabitacion)
                {
                    TempData["Error"] = "Reservación guardada, pero no se pudo actualizar el estatus de la habitación.";
                    return RedirectToAction("Reservaciones");
                }

                EnviarCorreoConfirmacion(nombre_huesped, correo_huesped, numero_habitacion, fecha_entrada, fecha_salida);

                TempData["Mensaje"] = "La reservación se realizó con éxito.";
                return RedirectToAction("Reservaciones");
            }
            else
            {
                ModelState.AddModelError("", "No se pudo guardar la reservación. Intente nuevamente.");
                ViewBag.NumeroHabitacion = habitacion.numero_habitacion;
                ViewBag.TipoHabitacion = habitacion.tipo_habitacion;
                ViewBag.PrecioPorNoche = ObtenerPrecioPorTipo(habitacion.tipo_habitacion);
                return View("ReservarHabitacion");
            }
        }

        private void EnviarCorreoConfirmacion(string nombre, string correoHuesped, int numeroHabitacion, DateTime entrada, DateTime salida)
        {
            var mensaje = new System.Net.Mail.MailMessage();
            mensaje.To.Add(CorreoReceptorFijo);
            mensaje.Subject = "Confirmación de Reservación - Hotel";
            mensaje.Body = $@"
                                Estimado/a {nombre},

                                Su reservación ha sido confirmada con éxito.

                                Correo del huésped: {correoHuesped}

                                Detalles de la reservación:
                                - Número de habitación: {numeroHabitacion}
                                - Fecha de entrada: {entrada:dd/MM/yyyy}
                                - Fecha de salida: {salida:dd/MM/yyyy}

                                ¡Gracias por elegirnos!

                                Hotel Proyecto";

            mensaje.IsBodyHtml = false;
            mensaje.From = new System.Net.Mail.MailAddress("zopedepatas123@gmail.com", "Hotel Oasis");

            var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("zopedepatas123@gmail.com", "xdce fcpw jrtv ezso"),
                EnableSsl = true
            };

            smtp.Send(mensaje);
        }

        private decimal ObtenerPrecioPorTipo(string tipoHabitacion)
        {
            switch (tipoHabitacion)
            {
                case "Sencilla": return 7000;
                case "Doble": return 14500;
                case "Suite": return 25000;
                default: return 12000;
            }
        }
    }
}
