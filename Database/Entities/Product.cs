using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShopWebApp
{
    public class Product : EntityBase
    {
        public Product() : base()
        {
            RatingSum = 0;
            RatingVotes = 0;
            About = "";
            LongAbout = "";
            Photo = "";
            OtherPhotos = "";
        }
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Code { get; set; }
        public int Price { get; set; }
        public string Tags { get; set; }
        public string About { get; set; }
        public string LongAbout { get; set; }
        public int RatingSum { get; set; }
        public int RatingVotes { get; set; }
        public string Photo { get; set; }
        public string OtherPhotos { get; set; }
        public int Stock { get; set; }



        public int SubcategoryId { get; set; }
        public Subcategory Subcategory { get; set; }
        public IList<ProductOrder> ProductOrders { get; set; }
    }
}
