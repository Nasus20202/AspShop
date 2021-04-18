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
        }
        public string Title { get; set; }
    }
}
