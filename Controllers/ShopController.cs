using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp.Controllers
{
    public class ShopController : Controller
    {
        private static int productsPerPage = 30;
        [Route("/s")]
        public IActionResult Index()
        {
            return Redirect(Url.Action("index", "home"));
        }
        [Route("s/{categoryName}/")]
        public IActionResult Category(string categoryName, [FromQuery] int page = 1)
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
            using (var db = new ShopDatabase())
            {
                var category = (from c in db.Categories
                                   where c.Code == categoryName
                                   select c).FirstOrDefault();
                if (category == null)
                    category = (from c in db.Categories
                                where c.Name == categoryName
                                select c).FirstOrDefault();
                if (category == null)
                {
                    int id;
                    if (int.TryParse(categoryName, out id))
                        category = DbFunctions.FindCategoryById(id);
                }
                if (category == null)
                    return Redirect("/Error/404");
                db.Entry(category)
                    .Collection(s => s.Subcategories)
                    .Load();
                model.Subcategories = category.Subcategories.ToList();
                model.Category = category;
                model.CategoryId = category.CategoryId;
                db.Entry(category)
                    .Collection(s => s.Subcategories)
                    .Load();
                var productList = new List<Product>();
                foreach(Subcategory subcategory in category.Subcategories)
                {
                    db.Entry(subcategory)
                        .Collection(s => s.Products)
                        .Load();
                    foreach (Product product in subcategory.Products)
                        productList.Add(product);
                }
                var cutProductList = new List<Product>();
                if (page <= 0 || page > productList.Count / productsPerPage + (productList.Count % productsPerPage != 0 ? 1 : 0))
                    return Redirect("/Error/404");
                for (int i = (page - 1) * productsPerPage; i < Math.Min(page * productsPerPage, productList.Count); i++)
                {
                    cutProductList.Add(productList[i]);
                }
                model.LastPage = productList.Count / productsPerPage;
                if (productList.Count % productsPerPage != 0)
                    model.LastPage++;
                model.Page = page;
                model.Products = cutProductList;
                model.Title = category.Name;
            }

            return View(model);
        }
        [Route("/s/{categoryName}/{subcategoryName}/")]
        public IActionResult Subcategory(string categoryName, string subcategoryName, [FromQuery] int page = 1)
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
                var productList = subcategory.Products.ToList();
                var cutProductList = new List<Product>();
                if (page <= 0 || page > productList.Count / productsPerPage + (productList.Count % productsPerPage != 0 ? 1 : 0))
                    return Redirect("/Error/404");
                for(int i = (page-1)*productsPerPage; i < Math.Min(page*productsPerPage, productList.Count); i++)
                {
                    cutProductList.Add(productList[i]);
                }
                model.LastPage = productList.Count / productsPerPage;
                if (productList.Count % productsPerPage != 0)
                    model.LastPage++;
                model.Page = page;
                model.Products = cutProductList;
                model.Subcategory = subcategory;
                model.SubcategoryId = subcategory.SubcategoryId;
            }
            model.Title = model.Subcategory.Name;
            return View(model);
        }
    }
}
