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
                model.Dict.Add("ID", user.Id.ToString());
                model.Dict.Add("Imię", user.Name);
                model.Dict.Add("Nazwisko", user.Surname);
                model.Dict.Add("Email", user.Email);
                model.Dict.Add("Adres", user.Address);
                model.Dict.Add("Telefon", user.Phone);
                model.Dict.Add("Rola", user.Role);
                model.Dict.Add("Ostatnia edycja", user.Modified.ToString());
                model.Dict.Add("Hash hasła (SHA256)", user.Password);

            } else if(table == "categories")
            {
                model.Title = "Kategoria : " + name; 
            } else { return Redirect("/Error/404"); }

            return View(model);
        }

        [Route("/admin/table/{tablename}")]
        public IActionResult DatabaseTable(string tablename, [FromQuery] int page = 1)
        {
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
                    int start = (page - 1) * 10, end = Math.Min(page * 10, userList.Count()), lastPage = userList.Count()/10;
                    if (userList.Count() % 10 != 0) { lastPage++; }
                    if(start < userList.Count())
                    {
                        for (int i = start; i < end; i++) {
                            table.Names.Add(userList[i].Email);
                        }
                    }
                    if (table.Names.Count() == 0)
                    {
                        return Redirect("/Error/404");
                    }
                    table.Path = "/admin/users/";
                    model.Table = table;
                    model.Page = page; model.LastPage = lastPage;
                }
            }
            else if(tablename.ToLower() == "categories")
            {
                using (var db = new ShopDatabase())
                {
                    List<Category> categoryList = (from c in db.Categories
                                           select c).ToList();

                    AdminModel.AdminList table = new AdminModel.AdminList();
                    int start = (page - 1) * 10, end = Math.Min(page * 10, categoryList.Count()), lastPage = categoryList.Count() / 10;
                    if (categoryList.Count() % 10 != 0) { lastPage++; }
                    if (start < categoryList.Count())
                    {
                        for (int i = start; i < end; i++)
                        {
                            table.Names.Add(categoryList[i].Name);
                        }
                    }
                    if (table.Names.Count() == 0)
                    {
                        return Redirect("/Error/404");
                    }
                    table.Path = "/admin/categories/";
                    model.Table = table;
                    model.Page = page; model.LastPage = lastPage;
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
