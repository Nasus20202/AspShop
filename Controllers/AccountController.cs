using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            var model = new LoginModel("Moje konto");
            return View(model);
        }

        public IActionResult Login()
        {
            var input = new LoginModel();
            input.Title = "Zaloguj się";
            return View(input);
        }

        public IActionResult Denied()
        {
            var model = new BaseViewModel("Odmowa dostępu");
            return View(model);
        }

        public IActionResult Register()
        {
            var model = new LoginModel("Zarejestruj się");
            return View(model);
        }
    }
}
