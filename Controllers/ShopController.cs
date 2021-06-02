using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public IActionResult Category(string categoryName, [FromQuery] int page = 1, [FromQuery] string sort = "popularity")
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
                foreach (Subcategory subcategory in category.Subcategories)
                {
                    db.Entry(subcategory)
                        .Collection(s => s.Products)
                        .Load();
                    foreach (Product product in subcategory.Products.Where(p => p.Enabled))
                        productList.Add(product);
                }
                switch (sort) {
                    case "name":
                        productList = productList.OrderBy(p => (p.Brand + " " + p.Name)).ToList(); break;
                    case "-name":
                        productList = productList.OrderByDescending(p => (p.Brand + " " + p.Name)).ToList(); break;
                    case "price":
                        productList = productList.OrderBy(p => p.Price).ToList(); break;
                    case "-price":
                        productList = productList.OrderByDescending(p => p.Price).ToList(); break;
                    case "rating":
                        productList = productList.OrderByDescending(p => (p.RatingVotes == 0 ? 0 : (double)p.RatingSum / (double)p.RatingVotes)).ToList(); break;
                    default: productList = productList.OrderByDescending(p => p.RatingVotes).ToList(); break;
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
                ViewData["sort"] = sort;
                model.Products = cutProductList;
                model.Title = category.Name;
            }

            return View(model);
        }
        [Route("/s/{categoryName}/{subcategoryName}/")]
        public IActionResult Subcategory(string categoryName, string subcategoryName, [FromQuery] Dictionary<string, string> filters, [FromQuery] int page = 1, [FromQuery] string sort = "popularity")
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

                var productList = subcategory.Products.Where(p => p.Enabled).ToList();

                Dictionary<string, string> tags = new Dictionary<string, string>();

                Dictionary<string, Dictionary<string, string>> productTags = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, Dictionary<string, int>> tagTypes = new Dictionary<string, Dictionary<string, int>>();
                foreach (Product product in productList)
                {
                    string[] productsTagStrings = product.Tags.Split(';');
                    Dictionary<string, string> oneProductTags = new Dictionary<string, string>();
                    foreach (string tagString in productsTagStrings)
                    {
                        string[] tag = tagString.Split(':');
                        if (tag.Count() >= 2)
                        {
                            oneProductTags.Add(tag[0], tag[1]);
                            if (tagTypes.ContainsKey(tag[0]))
                            {
                                if (tagTypes[tag[0]].ContainsKey(tag[1]))
                                    tagTypes[tag[0]][tag[1]]++;
                                else
                                    tagTypes[tag[0]].Add(tag[1], 1);
                            }
                            else
                            {
                                tagTypes.Add(tag[0], new Dictionary<string, int>() { { tag[1], 1 } });
                            }
                        }
                    }
                    productTags.Add(product.Code, oneProductTags);
                }

                string[] tagsTab = subcategory.Tags.Split(';');
                foreach (string tagString in tagsTab)
                {
                    string[] tag = tagString.Split(':');
                    if (tag.Length >= 2)
                    {
                        tags.Add(tag[0], tag[1]);
                        filters[tag[0]] = filters.ContainsKey(tag[0]) ? filters[tag[0]] : "";
                        if(tag[1] == "int")
                        {
                            filters[tag[0] + "from"] = filters.ContainsKey(tag[0]+ "from") && filters[tag[0]] + "from" != null ? filters[tag[0]+"from"] : "";
                            filters[tag[0] + "to"] = filters.ContainsKey(tag[0] + "to") && filters[tag[0]] + "to" != null ? filters[tag[0] + "to"] : "";
                        }
                        else if(tagTypes.ContainsKey(tag[0]))
                        {
                            foreach(KeyValuePair<string, int> kvp in tagTypes[tag[0]])
                            {
                                filters[tag[0] + ":" + kvp.Key] = filters.ContainsKey(tag[0] + ":" + kvp.Key) && filters[tag[0] + ":" + kvp.Key] != null ? filters[tag[0] + ":" + kvp.Key] : "";
                            }
                        }
                    }
                }
                double priceFrom = -1, priceTo = -1;

                filters["pricefrom"] = filters.ContainsKey("pricefrom") && filters["pricefrom"]!=null ? filters["pricefrom"] : "";
                filters["priceto"] = filters.ContainsKey("priceto") && filters["priceto"] != null ? filters["priceto"] : "";

                // Change ',' to '.' because double format with dot bugs the parse function
                filters["pricefrom"] = filters["pricefrom"].Replace('.', ',');
                filters["priceto"] = filters["priceto"].Replace('.', ',');

                if (filters.ContainsKey("pricefrom")) { double.TryParse(filters["pricefrom"], out priceFrom); };
                if (filters.ContainsKey("priceto")) { double.TryParse(filters["priceto"], out priceTo); };


                if(priceFrom > 0)
                {
                    productList = productList.Where(p => p.Price >= priceFrom*100).ToList();
                }
                if(priceTo > 0)
                {
                    productList = productList.Where(p => p.Price <= priceTo*100).ToList();
                }

                foreach(KeyValuePair<string, string> tag in tags)
                {
                    if(tag.Value == "int")
                    {
                        int from = -1, to = -1;
                        if (filters.ContainsKey(tag.Key + "from")){ int.TryParse(filters[tag.Key + "from"], out from); };
                        if (filters.ContainsKey(tag.Key + "to")) { int.TryParse(filters[tag.Key + "to"], out to); };
                    }
                }


                switch (sort)
                {
                    case "name":
                        productList = productList.OrderBy(p => (p.Brand + " " + p.Name)).ToList(); break;
                    case "-name":
                        productList = productList.OrderByDescending(p => (p.Brand + " " + p.Name)).ToList(); break;
                    case "price":
                        productList = productList.OrderBy(p => p.Price).ToList(); break;
                    case "-price":
                        productList = productList.OrderByDescending(p => p.Price).ToList(); break;
                    case "rating":
                        productList = productList.OrderByDescending(p => (p.RatingVotes == 0 ? 0 : (double)p.RatingSum / (double)p.RatingVotes)).ToList(); break;
                    default: productList = productList.OrderByDescending(p => p.RatingVotes).ToList(); break;
                }

                var cutProductList = new List<Product>();
                if (page <= 0 || page > productList.Count / productsPerPage + (productList.Count % productsPerPage != 0 ? 1 : 0) && productList.Count != 0)
                    return Redirect("/Error/404");
                for(int i = (page-1)*productsPerPage; i < Math.Min(page*productsPerPage, productList.Count); i++)
                {
                    cutProductList.Add(productList[i]);
                }

                model.LastPage = productList.Count / productsPerPage;
                if (productList.Count % productsPerPage != 0)
                    model.LastPage++;

                model.Page = page;
                ViewData["sort"] = sort;
                ViewBag.filters = filters;
                ViewBag.tags = tags;
                ViewBag.tagTypes = tagTypes;
                model.Products = cutProductList;
                model.Subcategory = subcategory;
                model.SubcategoryId = subcategory.SubcategoryId;
            }
            model.Title = model.Subcategory.Name;
            return View(model);
        }
        [Route("/p/{productName}")]
        public IActionResult Product(string productName)
        {
            var model = new BaseViewModel();
            var product = new Product();
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
                product = (from c in db.Products
                           where c.Code == productName
                           select c).FirstOrDefault();
                if (product == null)
                    product = (from c in db.Products
                               where c.Name == productName
                               select c).FirstOrDefault();
                if (product == null)
                {
                    int id;
                    if (int.TryParse(productName, out id))
                        product = DbFunctions.FindProductById(id);
                }
                if (product == null)
                    return Redirect("/Error/404");
            }
            ViewBag.Product = product;
            ViewData["subcategory"] = DbFunctions.FindSubcategoryById(product.SubcategoryId).Name;
            model.Title = product.Brand + " " + product.Name;
            return View(model);
        }

        [Route("/cart")]
        public IActionResult Cart()
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

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Cart")))
            {
                Dictionary<string, int> cartDict = new Dictionary<string, int>();
                HttpContext.Session.SetString("_Cart", JsonSerializer.Serialize(cartDict));
            }
            ViewBag.Cart = JsonSerializer.Deserialize<Dictionary<string, int>>(HttpContext.Session.GetString("_Cart"));
            
            model.Title = "Koszyk";
            return View(model);
        }

        [HttpPost]
        [Route("/cart/add/{code}/{count:int?}")]
        public IActionResult AddToCart(string code, int count = 1)
        {
            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                return Redirect("/Error/404");
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Cart")))
            {
                Dictionary<string, int> dict = new Dictionary<string, int>();
                HttpContext.Session.SetString("_Cart", JsonSerializer.Serialize(dict));
            }
            Dictionary<string, int> cartDict = JsonSerializer.Deserialize<Dictionary<string, int>>(HttpContext.Session.GetString("_Cart"));

            using(var db = new ShopDatabase())
            {
                var product = db.Products.Where(p => p.Code == code).FirstOrDefault();
                if (product == null)
                    throw new KeyNotFoundException("Code " + code + " was not found in products table.");
            }
            if (cartDict.ContainsKey(code))
            {
                cartDict[code] += count;
            }
            else
            {
                cartDict.Add(code, count);
            }
            if (cartDict[code] <= 0)
                cartDict.Remove(code);
            HttpContext.Session.SetString("_Cart", JsonSerializer.Serialize(cartDict));

            return Ok();

        }
        [HttpPost]
        [Route("/cart/remove/{code?}")]
        public IActionResult RemoveFromCart(string code)
        {
            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                return Redirect("/Error/404");
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Cart")))
            {
                Dictionary<string, int> dict = new Dictionary<string, int>();
                HttpContext.Session.SetString("_Cart", JsonSerializer.Serialize(dict));
            }
            Dictionary<string, int> cartDict = JsonSerializer.Deserialize<Dictionary<string, int>>(HttpContext.Session.GetString("_Cart"));

            if (code == null)
                cartDict = new Dictionary<string, int>();
            else if (cartDict.ContainsKey(code))
                cartDict.Remove(code);

            HttpContext.Session.SetString("_Cart", JsonSerializer.Serialize(cartDict));

            return Ok();
        }
        [HttpGet]
        [Route("/cart/list")]
        public IActionResult GetCartList()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("_Cart")))
            {
                Dictionary<string, int> dict = new Dictionary<string, int>();
                HttpContext.Session.SetString("_Cart", JsonSerializer.Serialize(dict));
            }
            Dictionary<string, int> cartDict = JsonSerializer.Deserialize<Dictionary<string, int>>(HttpContext.Session.GetString("_Cart"));
            ViewBag.Cart = cartDict;
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("/Views/Shop/Partial/pv_cartlist.cshtml");
            else
                return Redirect("/Error/404");
        }

        [Route("/search")]
        public IActionResult Search([FromQuery] string name = " ", [FromQuery] int page = 1, [FromQuery] string sort = "popularity")
        {
            if (name == null)
                return Redirect("/Error/404");
            var model = new SearchModel();
            model.SearchFor = name;
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
            using (var db = new ShopDatabase()) {
                var products = db.Products
                    .Where(p => (p.Name.Contains(name) || p.Brand.Contains(name)) && p.Enabled)
                    .ToList();
                products = products.Where(p => p.Enabled).ToList();
                model.Count = products.Count;
                var cutProductList = new List<Product>();
                if(products.Count == 0)
                {
                    ViewData["message"] = "Brak wyników! Polecamy inne produkty dostępne w naszym sklepie";
                    products = db.Products.Where(p => p.Enabled).ToList();
                }
                else if (page <= 0 || page > products.Count / productsPerPage + (products.Count % productsPerPage != 0 ? 1 : 0))
                    return Redirect("/Error/404");
                switch (sort)
                {
                    case "name":
                        products = products.OrderBy(p => (p.Brand + " " + p.Name)).ToList(); break;
                    case "-name":
                        products = products.OrderByDescending(p => (p.Brand + " " + p.Name)).ToList(); break;
                    case "price":
                        products = products.OrderBy(p => p.Price).ToList(); break;
                    case "-price":
                        products = products.OrderByDescending(p => p.Price).ToList(); break;
                    case "rating":
                        products = products.OrderByDescending(p => (p.RatingVotes == 0 ? 0 : (double)p.RatingSum / (double)p.RatingVotes)).ToList(); break;
                    default: products = products.OrderByDescending(p => p.RatingVotes).ToList(); break;
                }
                for (int i = (page - 1) * productsPerPage; i < Math.Min(page * productsPerPage, products.Count); i++)
                {
                    cutProductList.Add(products[i]);
                }
                model.LastPage = products.Count / productsPerPage;
                if (products.Count % productsPerPage != 0)
                    model.LastPage++;
                model.Page = page;
                ViewData["sort"] = sort;
                model.Products = cutProductList;
            }
            model.Title = "Szukaj: " + name;
            return View(model);
        }

        [Route("/review/{code}/{vote:int}")]
        public IActionResult Review(string  code, int vote)
        {
            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                return Redirect("/Error/404");
            int? isReviewed = HttpContext.Session.GetInt32(code);
            if (isReviewed == null)
                isReviewed = 0;
            Product product;
            using(var db = new ShopDatabase())
            {
                product = db.Products.Where(p => p.Code == code || p.ProductId.ToString() == code).FirstOrDefault();
                if (product == null)
                    return Redirect("/Error/404");
                if (vote <= 5 && vote >= 1)
                {
                    if (isReviewed == 0)
                    {
                        product.RatingVotes++;
                        product.RatingSum += vote;
                        HttpContext.Session.SetInt32(code, vote);
                    }
                    else
                    {
                        product.RatingSum -= (int)isReviewed;
                        product.RatingSum += vote;
                        HttpContext.Session.SetInt32(code, vote);
                    }
                }
                db.SaveChanges();
            }

            return Ok();
        }
    }
}
