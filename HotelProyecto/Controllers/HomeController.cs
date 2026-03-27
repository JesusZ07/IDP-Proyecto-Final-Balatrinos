using System;
using Microsoft.AspNetCore.Mvc;

namespace HotelProyecto.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contacto()
        {
            return View();
        }

    }
}