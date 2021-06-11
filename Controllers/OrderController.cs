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
            if (input.Order != null)
                model.Order = input.Order;
            if(User.Identity.IsAuthenticated)
            {
                model.Order.ClientName = model.Order.ClientName == null || model.Order.ClientName == string.Empty ? model.User.Name : model.Order.ClientName;
                model.Order.ClientSurname = model.Order.ClientSurname == null || model.Order.ClientSurname == string.Empty ? model.User.Surname : model.Order.ClientSurname;
                model.Order.ClientPhone = model.Order.ClientPhone == null || model.Order.ClientPhone == string.Empty ? model.User.Phone : model.Order.ClientPhone;
                model.Order.ClientEmail = model.Order.ClientEmail == null || model.Order.ClientEmail == string.Empty ? model.User.Email : model.Order.ClientEmail;
                model.Order.Address = model.Order.Address == null || model.Order.Address == string.Empty ? model.User.Address : model.Order.Address;
            }

            if (input.Message != null && input.Message != string.Empty)
                model.Message = input.Message;
            model.Title = "Twoje zamówienie";
            return View(model);
        }
        [HttpPost]
        [ActionName("Index")]
        public IActionResult SubmitOrder(OrderModel input)
        {
            input.Message = string.Empty; bool isValid = true;
            if (input.Order.ShippingType < 1 || input.Order.ShippingType > 3) { 
                input.Message += "Niepoprawna metoda dostawy\n"; isValid = false; }
            if (input.Order.PaymentMethod < 1 || input.Order.PaymentMethod > 3) {
                input.Message += "Niepoprawna metoda płatności\n"; isValid = false; }
            return Index(input);
        }
    }
}
