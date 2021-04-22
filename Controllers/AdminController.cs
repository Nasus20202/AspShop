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
                }
            }
            return View(model);
        }

        [Route("/admin/table/{tablename}")]
        public IActionResult DatabaseTable(string tablename, [FromQuery] int page = 1)
        {
            var model = new AdminModel("Tabela " + tablename);
            if(tablename.ToLower() == "users")
            {
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
                    table.Path = "/admin/user/";
                    model.Table = table;
                    model.Page = page; model.LastPage = lastPage;
                }
            }
            else if(tablename.ToLower() == "categories")
            {

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
