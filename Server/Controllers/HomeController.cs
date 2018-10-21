using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Models;
using Server.Models;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly FirstStep _firstStep = new FirstStep();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Order()
        {
            ViewData["Message"] = "Tu możesz wysłać do nas zamówienie na swoje nowe kaczuszki";

            return View();
        }

        public IActionResult History()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult OrderProducts([FromBody] OrderData order)
        {
            _firstStep.Send(order);
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
