using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class AccountModel : BaseViewModel
    {
        public AccountModel(string title) : base(title) { Message = ""; }
        public AccountModel() : base("") { Message = ""; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public string Message { get; set; }
        public bool RememberMe { get; set; }
    }
}
