using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class OrderModel : BaseViewModel
    {
        public OrderModel() : base() { }
        public Dictionary<String, int> Cart { get; set; }
        public Order Order { get; set; }
        public string Message { get; set; }
    }
}
