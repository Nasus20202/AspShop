using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class SubcategoryModel : BaseViewModel
    {
        public SubcategoryModel() : base() {}
        public IList<Product> Products { get; set; }

        public int Page { get; set; }
        public int LastPage { get; set; }
        public Subcategory Subcategory { get; set; }
        public int SubcategoryId { get; set; }

    }
}
