using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShopWebApp
{
    public class Subcategory : EntityBase
    {
        [Key]
        public int SubcategoryId { get; set;  }
        public string Name { get; set; }
        public string About { get; set; }
        public string Tags { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
