using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ShopWebApp
{
    public class ShopDatabase : DbContext
    {

        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

        // Many  to many  relationships
        public DbSet<ProductOrder> ProductOrders { get; set; }

        public string DbPath { get; set; }

        public ShopDatabase()
        {
            var localPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data");
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            DbPath = Path.Join(localPath, "shop.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductOrder>()
                .HasKey(po => new { po.OrderId, po.ProductId });
            modelBuilder.Entity<ProductOrder>()
                .HasOne(o => o.Order)
                .WithMany(po => po.ProductOrders)
                .HasForeignKey(po => po.OrderId);
            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Product)
                .WithMany(p => p.ProductOrders)
                .HasForeignKey(po => po.ProductId);
        }
    }
}
