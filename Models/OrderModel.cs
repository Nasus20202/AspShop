using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class OrderModel : BaseViewModel
    {
        public OrderModel() : base() { }
        public List<Product> Products { get; set; }
        public Order Order { get; set; }
    }
}
