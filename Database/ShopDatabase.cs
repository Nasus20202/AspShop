using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShopWebApp
{
    public class ShopDatabase : DbContext
    {
        //public static string ConnectionString = "";
        public static string ConnectionString = "Server = 127.0.0.1; Database = shop; Uid = nasus; Pwd = KNu17g2003cdVLO;";
       

        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(ConnectionString);
        }
    }
}
