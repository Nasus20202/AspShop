using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    [Authorize(Roles = "admin, manager, editor, employee")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var model = new AdminModel("Panel administratora");
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
                    model.User.Role = user.Role;
                }
            }
            return View(model);
        }


        // Detailed view

        [Route("/admin/{table}/{name}")]
        public IActionResult Info(string table, string name)
        {
            var model = new AdminModel();

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
                    model.User.Role = user.Role;
                }
            }
            
            model.Tablename = table; model.Name = name;
            if(table == "users")
            {
                if (model.User.Role == null || Functions.permissionLevel(model.User.Role) < 3)
                {
                    return Forbid();
                }
                model.Title = "Użytkownik : " + name;
                var user = DbFunctions.FindUserByEmail(name);
                if (user == null) {
                    int id;
                    if(int.TryParse(name, out id))
                        user = DbFunctions.FindUserById(id);
                }
                if (user == null)
                    return Redirect("/Error/404");
                model.Dict.Add("ID", user.UserId.ToString());
                model.Dict.Add("Imię", user.Name);
                model.Dict.Add("Nazwisko", user.Surname);
                model.Dict.Add("Email", user.Email);
                model.Dict.Add("Adres", user.Address);
                model.Dict.Add("Telefon", user.Phone);
                model.Dict.Add("Rola", user.Role);
                model.Dict.Add("Ostatnia edycja", user.Modified.ToString());
                model.Dict.Add("Hash hasła (SHA256)", user.Password);
                ViewData["Name"] = "Użytkownik";
                ViewData["ObjectName"] = user.Email;
            } 
            else if(table == "categories")
            {
                
                var category = new Category();
                using (var db = new ShopDatabase())
                {
                    category = (from c in db.Categories
                                where c.Name == name
                                select c).FirstOrDefault();
                    if (category == null)
                    {
                        int id;
                        if (int.TryParse(name, out id))
                            category = DbFunctions.FindCategoryById(id);
                    }
                    if(category == null)
                    {
                        category = (from c in db.Categories
                                    where c.Code == name
                                    select c).FirstOrDefault();
                    }
                }
                model.Title = "Kategoria : " + category.Name;
                if (category == null)
                    return Redirect("/Error/404");
                model.Dict.Add("ID", category.CategoryId.ToString());
                model.Dict.Add("Nazwa", category.Name);
                model.Dict.Add("Kod", category.Code);
                model.Dict.Add("Opis", category.About);

                using (var db = new ShopDatabase())
                {
                    var loadedCategory = db.Categories.Single(c => c.CategoryId == category.CategoryId);
                    var subcategories = db.Entry(loadedCategory)
                               .Collection(c => c.Subcategories)
                                .Query()
                                .ToList();
                    ViewData["type"] = "subcategories";
                    if (subcategories == null)
                        ViewBag.ChildObjects = new List<Subcategory>();
                    ViewBag.ChildObjects = subcategories;
                }

                ViewData["Name"] = "Kategoria";
                ViewData["ObjectName"] = category.Name;
            }
            else if(table == "subcategories")
            {
                var subcategory = new Subcategory();
                using (var db = new ShopDatabase())
                {
                    subcategory = (from c in db.Subcategories
                                where c.Name == name
                                select c).FirstOrDefault();
                    if (subcategory == null)
                    {
                        int id;
                        if (int.TryParse(name, out id))
                            subcategory = DbFunctions.FindSubcategoryById(id);
                    }
                    if (subcategory == null)
                    {
                        subcategory = (from c in db.Subcategories
                                       where c.Code == name
                                    select c).FirstOrDefault();
                    }
                }
                model.Title = "Podkategoria : " + subcategory.Name;
                if (subcategory == null)
                    return Redirect("/Error/404");
                model.Dict.Add("ID", subcategory.SubcategoryId.ToString());
                model.Dict.Add("Nazwa", subcategory.Name);
                model.Dict.Add("Kod", subcategory.Code);
                model.Dict.Add("Tagi", subcategory.Tags);
                model.Dict.Add("Opis", subcategory.About);
                using (var db = new ShopDatabase())
                {
                    var loadedSubcategory = db.Subcategories.Single(s => s.SubcategoryId == subcategory.SubcategoryId);
                    var products = db.Entry(loadedSubcategory)
                               .Collection(c => c.Products)
                                .Query()
                                .ToList();
                    ViewData["type"] = "products";
                    if (products == null)
                        ViewBag.ChildObjects = new List<Product>();
                    ViewBag.ChildObjects = products;
                }
                ViewData["Name"] = "Podkategoria";
                ViewData["ObjectName"] = subcategory.Name;
            }
            else
            { 
                return Redirect("/Error/404"); 
            }

            return View(model);
        }

        // Category objects list

        [Route("/admin/table/{tablename}")]
        public IActionResult DatabaseTable(string tablename, [FromQuery] int page = 1)
        {
            const int objectsPerPage = 10;
            var model = new AdminModel("Tabela " + tablename);
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
                    model.User.Role = user.Role;
                }
            }

            if (tablename.ToLower() == "users")
            {
                if (model.User.Role == null || Functions.permissionLevel(model.User.Role) < 3)
                {
                    return Forbid();
                }
                using (var db = new ShopDatabase())
                {
                    List<User> userList = (from c in db.Users
                                      select c).ToList();
                    
                    AdminModel.AdminList table = new AdminModel.AdminList();
                    int start = (page - 1) * objectsPerPage, end = Math.Min(page * objectsPerPage, userList.Count()), lastPage = userList.Count()/ objectsPerPage;
                    if (userList.Count() % objectsPerPage != 0) { lastPage++; }
                    if(start < userList.Count())
                    {
                        for (int i = start; i < end; i++) {
                            table.Names.Add(userList[i].Email);
                            table.Codes.Add(userList[i].UserId.ToString());
                        }
                    }
                    if (table.Names.Count() == 0)
                    {
                        return Redirect("/Error/404");
                    }
                    table.Path = "/admin/users/";
                    model.Table = table;
                    model.Page = page; model.LastPage = lastPage;
                    ViewData["Name"] = "użytkownicy";
                }
            }
            else if(tablename.ToLower() == "categories")
            {
                using (var db = new ShopDatabase())
                {
                    List<Category> categoryList = (from c in db.Categories
                                           select c).ToList();

                    AdminModel.AdminList table = new AdminModel.AdminList();
                    int start = (page - 1) * objectsPerPage, end = Math.Min(page * objectsPerPage, categoryList.Count()), lastPage = categoryList.Count() / objectsPerPage;
                    if (categoryList.Count() % objectsPerPage != 0) { lastPage++; }
                    if (start < categoryList.Count())
                    {
                        for (int i = start; i < end; i++)
                        {
                            table.Names.Add(categoryList[i].Name);
                            table.Codes.Add(categoryList[i].Code);
                        }
                    }
                    if (table.Names.Count() == 0)
                    {
                        return Redirect("/Error/404");
                    }
                    table.Path = "/admin/categories/";
                    model.Table = table;
                    model.Page = page; model.LastPage = lastPage;
                    ViewData["Name"] = "Kategorie";
                }
            }
            else if(tablename.ToLower() == "subcategories")
            {
                using (var db = new ShopDatabase())
                {
                    List<Subcategory> subcategoryList = (from c in db.Subcategories
                                                         select c).ToList();
                    AdminModel.AdminList table = new AdminModel.AdminList();
                    int start = (page - 1) * objectsPerPage, end = Math.Min(page * objectsPerPage, subcategoryList.Count()), lastPage = subcategoryList.Count() / objectsPerPage;
                    if (start < subcategoryList.Count())
                    {
                        for(int i = start; i < end; i++)
                        {
                            table.Names.Add(subcategoryList[i].Name);
                            table.Codes.Add(subcategoryList[i].Code);
                        }
                    }
                    if(table.Names.Count() == 0)
                    {
                        return Redirect("/Error/404");
                    }
                    table.Path = "/admin/subcategories/";
                    model.Table = table;
                    model.Page = page; model.LastPage = lastPage;
                    ViewData["Name"] = "Podkategorie";
                }
            }
            else
            {
                return Redirect("/Error/404");
            }
            model.Tablename = tablename;
            return View(model);
        }
    }
}
