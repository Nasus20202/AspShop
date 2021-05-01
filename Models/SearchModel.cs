using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class SearchModel : BaseViewModel
    {
        public SearchModel() : base() { }
        public string SearchFor { get; set; }
        public IList<Product> Products { get; set; }
        public int Page { get; set; }
        public int LastPage { get; set; }
        public int Count { get; set; }
    }
}
