using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ShopWebApp
{
    public class Order : EntityBase
    {
        [Key]
        public int OrderId { get; set; }
        public string Code { get; set; }
        public int Amount { get; set; }
        public string Address { get; set; }
        public int PaymentMethod { get; set; }
        public bool Paid { get; set; }
        public int Status { get; set; }
        public DateTime DateOfOrder { get; set; }
        public string ClientName { get; set; }
        public string ClientSurname { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public string Comments { get; set; }

        public int ShippingType { get; set; }
        public string ShippingInfo { get; set; }



        public int UserId { get; set; }
        public User User { get; set; }
        public IList<ProductOrder> ProductOrders { get; set; }
    }
}
