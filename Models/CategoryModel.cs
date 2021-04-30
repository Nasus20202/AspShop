using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class CategoryModel : BaseViewModel
    {
        public CategoryModel() : base() {}
        public IList<Product> Products { get; set; }
        public IList<Subcategory> Subcategories { get; set; }
        public int Page { get; set; }
        public int LastPage { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
    }
}
