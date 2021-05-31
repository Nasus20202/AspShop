using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index([FromQuery] Dictionary<string, string> filters)
        {
            var model = new BaseViewModel();
            if (User.Identity.IsAuthenticated)
            {
                using (var db = new ShopDatabase())
                {
                    var user = (from c in db.Users
                                where c.Email == User.Identity.Name
                                select c).FirstOrDefault();
                    model.User.Name = user.Name;
                    model.User.Surname = user.Surname;
                    model.User.Email = user.Email;
                }
            }
            if (filters == null)
                filters = new Dictionary<string, string>();
            model.Title = "Twoje zamówienie";
            ViewData["switchType"] = filters.ContainsKey("switchType") ? filters["switchType"] : "";
            ViewData["color"] = filters.ContainsKey("color") ? filters["color"] : "";
            filters["switchType"] = "";
            filters["color"] = "";
            ViewBag.filters = filters;
            return View(model);
        }
    }
}
