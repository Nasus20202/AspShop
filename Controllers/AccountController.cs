using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopWebApp.Controllers
{
    public class AccountController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var model = new AccountModel("Moje konto");
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
            
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(AccountModel input)
        {
            if (input == null)
                input = new AccountModel();
            input.Title = "Edytuj konto";
            if (User.Identity.IsAuthenticated)
            {
                using (var db = new ShopDatabase())
                {
                    var user = (from c in db.Users
                                where c.Email == User.Identity.Name
                                select c).FirstOrDefault();
                    input.User.Name = user.Name;
                    input.User.Surname = user.Surname;
                    input.User.Email = user.Email;
                    input.User.Address = user.Address;
                    input.User.Phone = user.Phone;
                }
            }

            return View(input);
        }
        [HttpPost]
        [Authorize]
        [ActionName("Edit")]
        public async Task<IActionResult> TryEdit(AccountModel input)
        {
            string currentEmail = User.Identity.Name;
            var currentUser = DbFunctions.FindUserByEmail(currentEmail);
            bool needRelog = false, changedPass = false, changedEmail = false;
            if (input.User.Name != null && input.User.Name.Length >= 3 && input.User.Name.Length <= 128)
                currentUser.Name = input.User.Name;
            if (input.User.Surname != null && input.User.Surname.Length >= 3 && input.User.Surname.Length <= 128)
                currentUser.Surname = input.User.Surname;
            if (input.User.Phone != null && input.User.Phone.Length >= 9 && input.User.Phone.Length <= 16)
                currentUser.Phone = input.User.Phone;
            if (input.User.Phone == null || (input.User.Phone.Length >= 9 && input.User.Phone.Length <= 16))
                currentUser.Phone = input.User.Phone == null ? "" :input.User.Phone;
            if (input.User.Address == null || input.User.Address.Length <= 256)
                currentUser.Address = input.User.Address == null ? "" : input.User.Address;
            if (input.User.Email != null && input.User.Email.Contains('@') && input.User.Email.Length < 128 && IsEmailFree(input.User.Email))
            {
                currentUser.Email = input.User.Email; needRelog = true;
                changedEmail = true;
            }
            if (input.Password!=null && input.Password2 != null && input.OldPassword != null && input.Password == input.Password2 && input.Password.Length >= 8 && input.Password.Length <= 128 && Sha256Hash(input.OldPassword) == currentUser.Password)
            {
                currentUser.Password = Sha256Hash(input.Password);  needRelog = true;
                changedPass = true;
            }
            DbFunctions.UpdateUser(currentUser);
            if (needRelog)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var claims = new[]
                {
                 new Claim(ClaimTypes.Email, currentUser.Email),
                 new Claim(ClaimTypes.Role, currentUser.Role),
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Email, ClaimTypes.Role);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                input.Title = "Edytuj konto";
                if(changedPass && changedEmail)
                    input.Message = "Pomyślnie zmieniono adres email i hasło";
                else if (changedEmail)
                    input.Message = "Pomyślnie zmieniono adres email";
                else
                    input.Message = "Pomyślnie zmieniono hasło";
                return View(input);
            }
            return Redirect(Url.Action("index", "account"));
        }

        [HttpGet]
        public IActionResult Login(AccountModel input)
        {
            if (input == null)
                input = new AccountModel();
            input.Title = "Zaloguj się";
            return View(input);
        }
        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> TryLogin(AccountModel input, [FromQuery] string ReturnUrl)
        {
            input.Title = "Zaloguj się";
            bool status = AreCredentialsValid(input.Login, input.Password);
            if (!status)
            {
                input.Message = "Niepoprawny email lub hasło";
            }
            else
            {
                using (var db = new ShopDatabase())
                {
                    var user = (from c in db.Users
                                where c.Email == input.Login
                                select c).FirstOrDefault();
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                    };
                    bool rememberMe = input.RememberMe;
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Email, ClaimTypes.Role);
                    if (rememberMe) {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddMonths(1)
                        }) ;
                    }
                    else { await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)); }
                    if(ReturnUrl == null)
                        return Redirect(Url.Action("index", "home"));
                    return Redirect(ReturnUrl);
                }
            }
            return View(input);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(Url.Action("index", "home"));
        } 

        [HttpGet]
        public IActionResult Register(AccountModel input)
        {
            if(User.Identity.IsAuthenticated)
                return Redirect(Url.Action("index", "home"));
            var model = new AccountModel("Zarejestruj się");
            return View(model);
        }
        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> TryRegister(AccountModel input)
        {
            if (IsEmailFree(input.User.Email))
            {
                if(input.Password != input.Password2)
                {
                    input.Message = "Hasła nie zgadzają się";
                }
                else
                {
                    if(input.User.Email == null || !input.User.Email.Contains('@') || input.User.Email.Length > 128 || input.User.Name == null || input.User.Name.Length > 64 || input.User.Surname == null || input.User.Surname.Length > 64 || input.Password == null || input.Password.Length > 128)
                    {
                        input.Message = "Niepoprawne dane";
                        input.Title = "Zarejestruj się";
                        return View(input);
                    }
                    var user = new User();
                    user.Email = input.User.Email;
                    user.Name = input.User.Name;
                    user.Surname = input.User.Surname;
                    user.Password = Sha256Hash(input.Password);
                    user.Role = "user";
                    if (input.User.Address == null || input.User.Address.Length > 256)
                        user.Address = "";
                    else
                        user.Address = input.User.Address;
                    if (input.User.Phone == null || input.User.Phone.Length > 16 || input.User.Phone.Length < 9)
                        user.Phone = "";
                    else
                        user.Phone = input.User.Phone;
                    DbFunctions.AddUser(user);
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Email, ClaimTypes.Role);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    return Redirect(Url.Action("index", "home"));
                }
            }
            else
            {
                input.Message = "Email " + input.User.Email + " jest już zajęty";
            }
            input.Title = "Zarejestruj się";
            return View(input);
        }

        public IActionResult Denied()
        {
            var model = new BaseViewModel("Odmowa dostępu");
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

        private bool AreCredentialsValid(string login, string password)
        {
            using (var db = new ShopDatabase())
            {
                var user = (from c in db.Users
                            where c.Email == login
                            select c).FirstOrDefault();
                if (user == null || user.Password != Sha256Hash(password))
                {
                    return false;
                }
                return true;
            }
        }

        private bool IsEmailFree(string email)
        {
            using (var db = new ShopDatabase())
            {
                var user = (from c in db.Users
                            where c.Email == email
                            select c).FirstOrDefault();
                if (user == null)
                    return true;
                else
                    return false;
            }
        }
        private string Sha256Hash(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hashed;
            string hashedString = String.Empty;
            using (SHA256 hasher = SHA256.Create())
            {
                hashed = hasher.ComputeHash(bytes);
            }
            foreach(byte b in hashed)
            {
                hashedString += b.ToString("X");
            }
            return hashedString;
        }
    }
}
