using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class BaseViewModel
    {
        public BaseViewModel(string title = "")
        {
            Title = title;
            User = new UserData();
        }
        public string Title { get; set; }
        public UserData User { get; set; }
    }
    public class UserData
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }
    }
}
