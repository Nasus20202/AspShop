using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
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
            table = table.ToLower();
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
                if (model.User.Role == null || Functions.PermissionLevel(model.User.Role) < 3)
                {
                    return Forbid();
                }
                
                var user = DbFunctions.FindUserByEmail(name);
                if (user == null) {
                    int id;
                    if(int.TryParse(name, out id))
                        user = DbFunctions.FindUserById(id);
                }
                model.Title = "Użytkownik : " + user.Email;
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
                if(Functions.PermissionLevel(model.User.Role)>=5)
                    model.Dict.Add("Hash hasła (SHA256)", user.Password);
                ViewData["Name"] = "Użytkownik";
                ViewData["ObjectName"] = user.Email;
                ViewBag.Id = user.UserId;
                ViewBag.PermissionLevelToEdit = 5;
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
                ViewBag.Id = category.CategoryId;
                ViewBag.PermissionLevelToEdit = 4;

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
                ViewBag.Id = subcategory.SubcategoryId;
                ViewBag.PermissionLevelToEdit = 4;
                ViewData["Name"] = "Podkategoria";
                ViewData["ObjectName"] = subcategory.Name;
            }
            else if(table == "products")
            {
                Product product = new Product();
                using(var db = new ShopDatabase())
                {
                    product = (from p in db.Products
                               where p.Code == name
                               select p).FirstOrDefault();
                }
                if(product == null)
                {
                    int id;
                    if (int.TryParse(name, out id))
                        product = DbFunctions.FindProductById(id);
                }
                if (product == null)
                    return Redirect("/Erorr/404");
                model.Dict.Add("ID", product.ProductId.ToString());
                model.Dict.Add("Nazwa", product.Name);
                model.Dict.Add("Producent", product.Brand);
                model.Dict.Add("Kod", product.Code);
                model.Dict.Add("Cena", (product.Price / 100.0).ToString() + " zł");
                model.Dict.Add("Cena przed promocją", (product.OldPrice / 100.0).ToString() + " zł");
                model.Dict.Add("Ilość dostępnych", product.Stock.ToString());
                model.Dict.Add("Zdjęcie główne", product.Photo);
                model.Dict.Add("Dodatkowe zdjęcia", product.OtherPhotos);
                model.Dict.Add("Ocena produktu", product.RatingVotes > 0 ? Math.Round(product.RatingSum / (double)product.RatingVotes, 2).ToString() + " (" + product.RatingVotes + ")" : "Brak ocen");
                model.Dict.Add("Tagi", product.Tags);
                model.Dict.Add("Krótki opis", product.About);
                model.Dict.Add("Długi opis", product.LongAbout);
                model.Title = "Produkt: " + product.Brand + " " + product.Name;
                ViewBag.Id = product.ProductId;
                ViewBag.PermissionLevelToEdit = 4;
                ViewData["Name"] = "Produkt";
                ViewData["ObjectName"] = product.Brand + " " + product.Name;
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

            // Number of objects displayed on one page
            const int objectsPerPage = 50;


            if (page < 1)
                return Redirect("/Error/404");
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

            tablename = tablename.ToLower();
            if (tablename == "users")
            {
                if (model.User.Role == null || Functions.PermissionLevel(model.User.Role) < 3)
                {
                    return Forbid();
                }
                using (var db = new ShopDatabase())
                {
                    List<User> userList = db.Users.OrderBy(u => u.Email).ToList();
                    
                    AdminModel.AdminList table = new AdminModel.AdminList();
                    int start = (page - 1) * objectsPerPage, end = Math.Min(page * objectsPerPage, userList.Count), lastPage = userList.Count/ objectsPerPage;
                    if (userList.Count % objectsPerPage != 0) { lastPage++; }
                    if(start < userList.Count)
                    {
                        for (int i = start; i < end; i++) {
                            table.Names.Add(userList[i].Email);
                            table.Codes.Add(userList[i].UserId.ToString());
                        }
                    }
                    if (table.Names.Count == 0)
                    {
                        return Redirect("/Error/404");
                    }
                    table.Path = "/admin/users/";
                    model.Table = table;
                    model.Page = page; model.LastPage = lastPage;
                    ViewData["Name"] = "użytkownicy";
                }
            }
            else if(tablename == "categories")
            {
                using (var db = new ShopDatabase())
                {
                    List<Category> categoryList = db.Categories.OrderBy(c => c.Name).ToList();
                    AdminModel.AdminList table = new AdminModel.AdminList();
                    int start = (page - 1) * objectsPerPage, end = Math.Min(page * objectsPerPage, categoryList.Count()), lastPage = categoryList.Count() / objectsPerPage + (categoryList.Count() % objectsPerPage == 0 ? 0 : 1);
                    if (start < categoryList.Count)
                    {
                        for (int i = start; i < end; i++)
                        {
                            table.Names.Add(categoryList[i].Name);
                            table.Codes.Add(categoryList[i].Code);
                        }
                    }
                    if (table.Names.Count == 0)
                    {
                        return Redirect("/Error/404");
                    }
                    table.Path = "/admin/categories/";
                    model.Table = table;
                    model.Page = page; model.LastPage = lastPage;
                    ViewData["Name"] = "Kategorie";
                }
            }
            else if(tablename == "subcategories")
            {
                using (var db = new ShopDatabase())
                {
                    List<Subcategory> subcategoryList = db.Subcategories.OrderBy(s => s.Name).ToList();
                    AdminModel.AdminList table = new AdminModel.AdminList();
                    int start = (page - 1) * objectsPerPage, end = Math.Min(page * objectsPerPage, subcategoryList.Count()), lastPage = subcategoryList.Count() / objectsPerPage + (subcategoryList.Count() % objectsPerPage == 0 ? 0 : 1);
                    if (start < subcategoryList.Count)
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
            else if(tablename == "products")
            {
                using (var db = new ShopDatabase())
                {
                    List<Product> productList = db.Products.OrderBy(p => p.Brand).ToList();
                    AdminModel.AdminList table = new AdminModel.AdminList();
                    int start = (page - 1) * objectsPerPage, end = Math.Min(page * objectsPerPage, productList.Count()), lastPage = productList.Count() / objectsPerPage + (productList.Count()%objectsPerPage == 0 ? 0 : 1);
                    if (start < productList.Count())
                    {
                        for (int i = start; i < end; i++)
                        {
                            table.Names.Add(productList[i].Brand + " " + productList[i].Name);
                            table.Codes.Add(productList[i].Code);
                        }
                    }
                    if (table.Names.Count() == 0)
                    {
                        return Redirect("/Error/404");
                    }
                    table.Path = "/admin/products/";
                    model.Table = table;
                    model.Page = page; model.LastPage = lastPage;
                    ViewData["Name"] = "Produkty";
                }
            }
            else
            {
                return Redirect("/Error/404");
            }
            model.Tablename = tablename;
            return View(model);
        }

        // Edit object
        [HttpGet]
        [Route("/admin/{table}/{name}/edit")]
        public IActionResult Edit(string table, string name)
        {
            var model = new BaseViewModel();
            model.Title = "Edytuj " + name + " w " + table;
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
            if (table == "categories")
            {
                if (Functions.PermissionLevel(model.User.Role) < 4)
                {
                    return Forbid();
                }
                using(var db = new ShopDatabase())
                {
                    Category category = (from c in db.Categories
                                         where c.CategoryId.ToString() == name || c.Name == name || c.Code == name
                                         select c).FirstOrDefault();
                    if (category == null)
                        return Redirect("/Error/404");
                    var values = new Dictionary<string, string>();
                    values.Add("Code", category.Code);
                    values.Add("Name", category.Name);
                    values.Add("About", category.About);
                    ViewBag.values = values;
                    ViewData["Name"] = category.Name;
                    ViewData["Id"] = category.CategoryId.ToString();
                    ViewData["table"] = table;
                }
            }
            else if (table == "subcategories")
            {
                if (Functions.PermissionLevel(model.User.Role) < 4)
                {
                    return Forbid();
                }
                using (var db = new ShopDatabase())
                {
                    Subcategory subcategory = (from c in db.Subcategories
                                         where c.SubcategoryId.ToString() == name || c.Name == name || c.Code == name
                                         select c).FirstOrDefault();
                    if (subcategory == null)
                        return Redirect("/Error/404");
                    var values = new Dictionary<string, string>();
                    values.Add("Code", subcategory.Code);
                    values.Add("Name", subcategory.Name);
                    values.Add("About", subcategory.About);
                    values.Add("Tags", subcategory.Tags);
                    values.Add("CategoryId", subcategory.CategoryId.ToString());
                    ViewBag.values = values;
                    ViewData["Name"] = subcategory.Name;
                    ViewData["Id"] = subcategory.SubcategoryId.ToString();
                    ViewData["table"] = table;
                }
            }
            else if (table == "products")
            {
                if (Functions.PermissionLevel(model.User.Role) < 4)
                {
                    return Forbid();
                }
                using (var db = new ShopDatabase())
                {
                    Product product = (from c in db.Products
                                         where c.ProductId.ToString() == name || c.Name == name || c.Code == name
                                         select c).FirstOrDefault();
                    if (product == null)
                        return Redirect("/Error/404");
                    var values = new Dictionary<string, string>();
                    values.Add("Name", product.Name);
                    values.Add("Brand", product.Brand);
                    values.Add("Code", product.Code);
                    values.Add("Price", product.Price.ToString());
                    values.Add("OldPrice", product.OldPrice.ToString());
                    values.Add("Tags", product.Tags);
                    values.Add("About", product.About);
                    values.Add("LongAbout", product.LongAbout);
                    if (Functions.PermissionLevel(model.User.Role) >= 5)
                    {
                        values.Add("RatingSum", product.RatingSum.ToString());
                        values.Add("RatingVotes", product.RatingVotes.ToString());
                    }
                    values.Add("Stock", product.Stock.ToString());
                    values.Add("Photo", product.Photo);
                    values.Add("OtherPhotos", product.OtherPhotos);

                    values.Add("SubcategoryId", product.SubcategoryId.ToString());
                    ViewBag.values = values;
                    ViewData["Name"] = product.Name;
                    ViewData["Id"] = product.ProductId.ToString();
                    ViewData["table"] = table;
                }
            }
            else if(table == "users")
            {
                if (Functions.PermissionLevel(model.User.Role) < 5)
                {
                    return Forbid();
                }
                using (var db = new ShopDatabase())
                {
                    User user = (from c in db.Users
                                       where c.UserId.ToString() == name || c.Email == name
                                       select c).FirstOrDefault();
                    if (user == null)
                        return Redirect("/Error/404");
                    var values = new Dictionary<string, string>();
                    values.Add("Email", user.Email);
                    values.Add("Name", user.Name);
                    values.Add("Surname", user.Surname);
                    values.Add("Role", user.Role);
                    values.Add("Password", user.Password);
                    values.Add("Address", user.Address);
                    values.Add("Phone", user.Phone);
                    ViewBag.values = values;
                    ViewData["Name"] = user.Email;
                    ViewData["Id"] = user.UserId.ToString();
                    ViewData["table"] = table;
                }
            }
            else
                return Redirect("/Error/404");
            return View(model);
        }

        [HttpPost]
        [Route("/admin/{table}/{objectName}/edit")]
        public IActionResult EditPost(string table, string objectName, Dictionary<string, string> values)
        {
            var user = new User();
            if (User.Identity.IsAuthenticated)
            {
                using (var db = new ShopDatabase())
                {
                    user = (from c in db.Users
                                where c.Email == User.Identity.Name
                                select c).FirstOrDefault();
                }
            }
            if (table == "categories")
            {
                using (var db = new ShopDatabase())
                {
                    if (Functions.PermissionLevel(user.Role) < 4)
                    {
                        return Forbid();
                    }
                    string s = objectName;
                    Category category = (from c in db.Categories
                                         where c.CategoryId.ToString() == objectName || c.Code == objectName
                                         select c).FirstOrDefault();
                    if (category == null)
                        return Redirect("/Error/404");
                    if (values["Name"] != null) { category.Name = values["Name"]; }
                    if (values["Code"] != null) { category.Code = values["Code"]; }
                    if (values["About"] != null) { category.About = values["About"]; }
                    DbFunctions.UpdateCategory(category);
                    objectName = category.Code;
                }
            }
            else if (table == "subcategories")
            {
                if (Functions.PermissionLevel(user.Role) < 4)
                {
                    return Forbid();
                }
                using (var db = new ShopDatabase())
                {
                    Subcategory subcategory = (from c in db.Subcategories
                                               where c.SubcategoryId.ToString() == objectName || c.Code == objectName
                                               select c).FirstOrDefault();
                    if (subcategory == null)
                        return Redirect("/Error/404");
                    if (values["Name"] != null) { subcategory.Name = values["Name"]; }
                    if (values["Code"] != null) { subcategory.Code = values["Code"]; }
                    if (values["About"] != null) { subcategory.About = values["About"]; }
                    if (values["Tags"] != null) { subcategory.Tags = values["Tags"]; }
                    int categoryId;
                    if (values["CategoryId"] != null)
                        if (int.TryParse(values["CategoryId"], out categoryId))
                        {
                            if(db.Categories.Where(c => c.CategoryId == categoryId).FirstOrDefault() != null)
                                subcategory.CategoryId = categoryId;
                        }
                    DbFunctions.UpdateSubategory(subcategory);
                    objectName = subcategory.Code;
                }
            }
            else if (table == "products")
            {
                if (Functions.PermissionLevel(user.Role) < 4)
                {
                    return Forbid();
                }
                using (var db = new ShopDatabase())
                {
                    Product product = (from c in db.Products
                                       where c.ProductId.ToString() == objectName || c.Code == objectName
                                       select c).FirstOrDefault();
                    if (product == null)
                        return Redirect("/Error/404");
                    int number;
                    if (values["Name"] != null) { product.Name = values["Name"]; }
                    if (values["Brand"] != null) { product.Brand = values["Brand"]; }
                    if (values["Code"] != null) { product.Code = values["Code"]; }
                    if (Functions.PermissionLevel(user.Role) >= 5)
                    {
                        if (values["Price"] != null)
                            if (int.TryParse(values["Price"], out number))
                                product.Price = number;
                        if (values["OldPrice"] != null)
                            if (int.TryParse(values["OldPrice"], out number))
                                product.OldPrice = number;
                    }
                    if (values["Tags"] != null) { product.Tags = values["Tags"]; }
                    if (values["About"] != null) { product.About = values["About"]; }
                    if (values["LongAbout"] != null) { product.LongAbout = values["LongAbout"]; }
                    if (Functions.PermissionLevel(user.Role)>=5)
                    {
                        if (values["RatingSum"] != null)
                            if (int.TryParse(values["RatingSum"], out number))
                                product.RatingSum = number;
                        if (values["RatingVotes"] != null)
                            if (int.TryParse(values["RatingVotes"], out number))
                                product.RatingVotes = number;
                    }
                    if (values["Photo"] != null) { product.Photo = values["Photo"]; }
                    if (values["OtherPhotos"] != null) { product.OtherPhotos = values["OtherPhotos"]; }
                    if (values["Stock"] != null)
                        if (int.TryParse(values["Stock"], out number))
                            product.Stock = number;

                    if (values["SubcategoryId"] != null)
                        if (int.TryParse(values["SubcategoryId"], out number))
                        {
                            if (db.Subcategories.Where(c => c.SubcategoryId == number).FirstOrDefault() != null)
                                product.SubcategoryId = number;
                        }
                    DbFunctions.UpdateProduct(product);
                    objectName = product.Code;
                }
            }
            else if(table == "users")
            {
                if (Functions.PermissionLevel(user.Role) < 5)
                {
                    return Forbid();
                }
                using (var db = new ShopDatabase())
                {
                    User shopUser = (from c in db.Users
                                       where c.UserId.ToString() == objectName || c.Email == objectName
                                       select c).FirstOrDefault();
                    if (shopUser == null)
                        return Redirect("/Error/404");
                    bool isCurrentUserBeingChanged = false;
                    if (shopUser.Email == user.Email)
                        isCurrentUserBeingChanged = true;
                    if (values["Email"] != null) { shopUser.Email = values["Email"]; }
                    if (values["Name"] != null) { shopUser.Name = values["Name"]; }
                    if (values["Surname"] != null) { shopUser.Surname = values["Surname"]; }
                    if (values["Role"] != null) { shopUser.Role = values["Role"]; }
                    if (values["Password"] != null) { shopUser.Password = values["Password"]; }
                    if (values["Address"] != null) { shopUser.Address = values["Address"]; }
                    if (values["Phone"] != null) { shopUser.Surname = values["Phone"]; }
                    DbFunctions.UpdateUser(shopUser);
                    objectName = shopUser.UserId.ToString();
                    if (user.Email != shopUser.Email && isCurrentUserBeingChanged)
                        return Redirect(Url.Action("logout", "account"));
                }
            }
            else
                return Redirect("/Error/404");
            return Redirect("/admin/" + table + "/" + objectName);
        }

        // Users and products search
        public IActionResult Search([FromQuery] string name, [FromQuery] int page = 1)
        {

            if (name == null)
                return Redirect("/Error/404");
            const int objectsPerPage = 30;
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
                    model.User.Role = user.Role;
                }
            }
            model.Title = "Szukaj: " + name;
            using (var db = new ShopDatabase()) {
                Dictionary<string, string> products = new Dictionary<string, string>();
                List<Product> productList = db.Products.Where(p => p.Name.Contains(name) || p.Brand.Contains(name) || p.Code == name || p.ProductId.ToString() == name).OrderBy(p => p.Brand).ToList();
                if (page > 0 && page <= productList.Count / objectsPerPage + (productList.Count % objectsPerPage != 0 ? 1 : 0))
                {
                    for (int i = (page - 1) * objectsPerPage; i < Math.Min(page * objectsPerPage, productList.Count); i++)
                    {
                        Product prod = productList[i];
                        products.Add(prod.Brand + " " + prod.Name, "/admin/products/" + prod.Code);
                    }
                }
                ViewBag.products = products;
                Dictionary<string, string> users = new Dictionary<string, string>();
                List<User> usersList = db.Users.Where(u => u.Email.Contains(name) || u.Name.Contains(name) || u.Surname.Contains(name) || u.UserId.ToString() == name).OrderBy(u => u.Email).ToList();
                if (page > 0 && page <= usersList.Count / objectsPerPage + (usersList.Count % objectsPerPage != 0 ? 1 : 0))
                {
                    for (int i = (page - 1) * objectsPerPage; i < Math.Min(page * objectsPerPage, usersList.Count); i++)
                    {
                        User user = usersList[i];
                        users.Add(user.Email + " (" + user.Name + " " + user.Surname + ")", "/admin/users/" + user.UserId);
                    }
                }
                if (page < 0 || page > Math.Max(usersList.Count / objectsPerPage + (usersList.Count % objectsPerPage != 0 ? 1 : 0), productList.Count / objectsPerPage + (productList.Count % objectsPerPage != 0 ? 1 : 0)))
                    return Redirect("/Error/404");
                ViewData["name"] = name;
                if (User.Identity.IsAuthenticated && Functions.PermissionLevel(model.User.Role) > 2)
                    ViewBag.users = users;
                else
                    ViewBag.users = new Dictionary<string, string>();
                ViewBag.page = page;
                ViewBag.lastPage = Math.Max(productList.Count / objectsPerPage + (productList.Count % objectsPerPage != 0 ? 1 : 0), usersList.Count / objectsPerPage + (usersList.Count % objectsPerPage != 0 ? 1 : 0));
            }
            return View(model);
        }
        
        // wwwroot/images browser
        public IActionResult Photos()
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
                    model.User.Role = user.Role;
                }
            }
            model.Title = "Zdjęcia";
            return View(model);
        }

        // Upload a new photo(s) to wwwroot/images
        // Only for authenticated users with rank higher than employee
        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            if (User.Identity.IsAuthenticated)
            {
                using (var db = new ShopDatabase())
                {
                    var user = (from c in db.Users
                                where c.Email == User.Identity.Name
                                select c).FirstOrDefault();
                    var role = user.Role;
                    if (Functions.PermissionLevel(role) < 3)
                        return Forbid();
                }
            }
            long size = files.Sum(f => f.Length);
            foreach(var formFile in files)
            {
                if(formFile.Length > 0)
                {
                    var fileName = Path.GetFileName(formFile.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var filePath = @"wwwroot/images/" + fileName;
                    if (fileExtension != ".webp" && fileExtension != ".png" && fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".gif" && fileExtension != ".bmp")
                        continue;

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return Redirect(Url.Action("photos", "admin"));
        }

        // Remove photo from wwwroot/images
        [Route("/admin/photos/remove/{filename}")]
        public IActionResult RemovePhoto(string filename)
        {
            if (filename == null)
                return Redirect("/Error/404");
            if (User.Identity.IsAuthenticated)
            {
                using (var db = new ShopDatabase())
                {
                    var user = (from c in db.Users
                                where c.Email == User.Identity.Name
                                select c).FirstOrDefault();
                    var role = user.Role;
                    if (Functions.PermissionLevel(role) < 3)
                        return Forbid();
                }
            }

            string filepath = @"wwwroot/images/" + filename;
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }

            return Redirect(Url.Action("photos", "admin"));
        }

    }
}
