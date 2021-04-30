using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp.Controllers
{
    public class ShopController : Controller
    {
        [Route("/s")]
        public IActionResult Index()
        {
            return Redirect(Url.Action("index", "home"));
        }
        [Route("s/{categoryName}/")]
        public IActionResult Category(string categoryName)
        {
            var model = new CategoryModel();
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
            model.Title = categoryName;
            return View(model);
        }
        [Route("/s/{categoryName}/{subcategoryName}/")]
        public IActionResult Subcategory(string categoryName, string subcategoryName)
        {
            var model = new SubcategoryModel();
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
            using(var db = new ShopDatabase())
            {
                var subcategory = (from c in db.Subcategories
                                   where c.Code == subcategoryName
                                   select c).FirstOrDefault();
                if (subcategory == null)
                    subcategory = (from c in db.Subcategories
                                   where c.Name == subcategoryName
                                   select c).FirstOrDefault();
                if(subcategory == null)
                {
                    int id;
                    if (int.TryParse(subcategoryName, out id))
                        subcategory = DbFunctions.FindSubcategoryById(id);
                }
                if(subcategory == null)
                    return Redirect("/Error/404");
                db.Entry(subcategory)
                    .Collection(s => s.Products)
                    .Load();
                model.Products = subcategory.Products.ToList();
                model.Subcategory = subcategory;
                model.SubcategoryId = subcategory.SubcategoryId;
            }
            model.Title = model.Subcategory.Name;
            return View(model);
        }
    }
}
