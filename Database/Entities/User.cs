using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ShopWebApp
{
    public class User : EntityBase
    {
        public User() { }

        [Key]
        public int UserId { get; set; }

        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }


        public IList<Order> Orders { get; set; }
    }
}
