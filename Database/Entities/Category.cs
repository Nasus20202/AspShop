using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShopWebApp
{
    public class Category : EntityBase
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
