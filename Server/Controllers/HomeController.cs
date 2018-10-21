using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

    public class OrderData
    {
        public IList<ProductData> Products { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
    }

    public class ProductData
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
    }
}
