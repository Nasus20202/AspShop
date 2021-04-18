using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var model = new AdminModel("Panel administratora");
            return View(model);
        }
    }
}
