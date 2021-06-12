using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShopWebApp.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index(OrderModel input)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Cart")))
            {
                Dictionary<string, int> dict = new Dictionary<string, int>();
                HttpContext.Session.SetString("_Cart", JsonSerializer.Serialize(dict));
            }
            Dictionary<string, int> cartDict = JsonSerializer.Deserialize<Dictionary<string, int>>(HttpContext.Session.GetString("_Cart"));
            var model = new OrderModel();
            model.Cart = cartDict;
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
            if(input.Order.ClientName.Length < 3 || input.Order.ClientName.Length > 64){
                input.Message += "Niepoprawne imię\n"; isValid = false; }
            if(input.Order.ClientSurname.Length < 3 || input.Order.ClientSurname.Length > 64){
                input.Message += "Niepoprawne nazwisko\n"; isValid = false; }
            if(input.Order.ClientEmail.Length < 5 || input.Order.ClientEmail.Length > 128 || !input.Order.ClientEmail.Contains('@')){
                input.Message += "Niepoprawny adres email\n"; isValid = false; }
            if(input.Order.ClientPhone.Length < 9 || input.Order.ClientPhone.Length > 16){
                input.Message += "Niepoprawny numer telefonu\n"; isValid = false; }
            if(input.Order.Address.Length < 5 || input.Order.Address.Length > 256){
                input.Message += "Niepoprawny adres\n"; isValid = false; }
            if (isValid)
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Cart")))
                    return Redirect("/cart");
                Dictionary<string, int> cartDict = JsonSerializer.Deserialize<Dictionary<string, int>>(HttpContext.Session.GetString("_Cart"));
                if (cartDict.Count < 1)
                    return Redirect("/cart");
                using (var db = new ShopDatabase())
                {
                    input.Order.Code = Functions.GenerateOrderCode();
                    DbFunctions.AddOrder(input.Order);
                    int amount = 0;
                    if (User.Identity.IsAuthenticated)
                    {
                        var user = (from c in db.Users
                                    where c.Email == User.Identity.Name
                                    select c).FirstOrDefault();
                         input.Order.UserId = user.UserId;
                    }
                    foreach (KeyValuePair<string, int> kvp in cartDict)
                    {
                        Product product = db.Products.Where(p => p.Code == kvp.Key).FirstOrDefault();
                        if (product == null)
                            continue;
                        amount += product.Price;
                        ProductOrder po = new ProductOrder();
                        po.OrderId = input.Order.OrderId;
                        po.ProductId = product.ProductId;
                        po.Count = kvp.Value;
                        db.ProductOrders.Add(po);
                    }
                    input.Order.Amount = amount;
                    DbFunctions.UpdateOrder(input.Order);
                    db.SaveChanges();
                }
                HttpContext.Session.SetString("_Cart", JsonSerializer.Serialize(new Dictionary<string, int>()));
                return Redirect("/order/" + input.Order.Code);

            }
            else
                return Index(input);
        }
        [Route("/order/{code}")]
        public IActionResult Status(string code)
        {
            OrderModel model = new OrderModel();
            Order order;
            Dictionary<Product, int> products = new Dictionary<Product, int>();
            using (var db = new ShopDatabase()) {
                int userId = 0;
                if (User.Identity.IsAuthenticated)
                {
                    var user = (from c in db.Users
                                where c.Email == User.Identity.Name
                                select c).FirstOrDefault();
                    model.User.Name = user.Name;
                    model.User.Surname = user.Surname;
                    model.User.Email = user.Email;
                    userId = user.UserId;
                }
                order = db.Orders.Where(o => o.Code == code || o.OrderId.ToString() == code).FirstOrDefault();
                if (order == null)
                    return Redirect("/Error/404");
                if (order.UserId != null && order.UserId != userId) {
                    return Forbid();
                }
                db.Entry(order).Collection(o => o.ProductOrders).Load();
                foreach (ProductOrder productOrder in order.ProductOrders)
                {
                    db.Entry(productOrder).Reference(po => po.Product).Load();
                    products.Add(productOrder.Product, productOrder.Count);
                }
            }

            ViewBag.products = products;
            model.Title = "Zamówienie " + code;
            model.Order = order;
            return View(model);
        }
    }
}
