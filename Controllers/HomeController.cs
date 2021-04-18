using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new BaseViewModel("Sklep");
            return View(model);
        }
    }
}
