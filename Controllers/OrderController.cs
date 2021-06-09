using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index(OrderModel input)
        {
            var model = new OrderModel();
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
                    model.User.Address = user.Address;
                    model.User.Phone = user.Phone;
                }
            }
            if (model.Order == null)
                model.Order = new Order();
            if (input.Message != null && input.Message != string.Empty)
                model.Message = input.Message;
            model.Title = "Twoje zamówienie";
            return View(model);
        }
        [HttpPost]
        [ActionName("Index")]
        public IActionResult SubmitOrder(OrderModel input)
        {
            input.Message = input.Order.ClientName;
            return Index(input);
        }
    }
}
