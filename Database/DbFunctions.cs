using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class DbFunctions
    {

        //Categories

        public static Category FindCategoryById(int id)
        {
            using(var db = new ShopDatabase())
            {
                var category = (from c in db.Categories
                                where c.CategoryId == id
                                select c).FirstOrDefault();
                return category;
            }
        }
        public static void AddCategory(Category category)
        {
            if (category == null)
                return;
            using (var db = new ShopDatabase())
            {
                db.Categories.Add(category);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return;
                }
            }
        }
        public static void UpdateCategory(Category updatedCategory)
        {
            using (var db = new ShopDatabase())
            {
                var category = (from c in db.Categories
                            where c.CategoryId == updatedCategory.CategoryId
                            select c).FirstOrDefault();
                if (category == null)
                    return;
                category.Name = updatedCategory.Name;
                category.Code = updatedCategory.Code;
                category.About = updatedCategory.About;
                category.Enabled = updatedCategory.Enabled;


                category.Modified = DateTime.UtcNow;
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }


        //Subcategories


        public static Subcategory FindSubcategoryById(int id)
        {
            using (var db = new ShopDatabase())
            {
                var subcategory = (from c in db.Subcategories
                                where c.SubcategoryId == id
                                select c).FirstOrDefault();
                return subcategory;
            }
        }
        public static void AddSubcategory(Subcategory subcategory)
        {
            if (subcategory == null)
                return;
            using (var db = new ShopDatabase())
            {
                db.Subcategories.Add(subcategory);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return;
                }
            }
        }
        public static void UpdateSubategory(Subcategory updatedSubcategory)
        {
            using (var db = new ShopDatabase())
            {
                var subcategory = (from c in db.Subcategories
                                where c.SubcategoryId == updatedSubcategory.SubcategoryId
                                select c).FirstOrDefault();
                if (subcategory == null)
                    return;
                subcategory.Name = updatedSubcategory.Name;
                subcategory.Code = updatedSubcategory.Code;
                subcategory.About = updatedSubcategory.About;
                subcategory.Tags = updatedSubcategory.Tags;
                subcategory.CategoryId = updatedSubcategory.CategoryId;
                subcategory.Enabled = updatedSubcategory.Enabled;


                subcategory.Modified = DateTime.UtcNow;
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }


        //Products


        public static Product FindProductById(int id)
        {
            using (var db = new ShopDatabase())
            {
                var product = (from c in db.Products
                                where c.ProductId == id
                                select c).FirstOrDefault();
                return product;
            }
        }
        public static void AddProduct(Product product)
        {
            if (product == null)
                return;
            using (var db = new ShopDatabase())
            {
                db.Products.Add(product);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return;
                }
            }
        }
        public static void UpdateProduct(Product updatedProduct)
        {
            using (var db = new ShopDatabase())
            {
                var product = (from c in db.Products
                                where c.ProductId == updatedProduct.ProductId
                                select c).FirstOrDefault();
                if (product == null)
                    return;
                product.Name = updatedProduct.Name;
                product.Brand = updatedProduct.Brand;
                product.Code = updatedProduct.Code;
                product.About = updatedProduct.About;
                product.LongAbout = updatedProduct.LongAbout;
                product.Price = updatedProduct.Price;
                product.OldPrice = updatedProduct.OldPrice;
                product.RatingSum = updatedProduct.RatingSum;
                product.RatingVotes = updatedProduct.RatingVotes;
                product.Photo = updatedProduct.Photo;
                product.Types = updatedProduct.Types;
                product.Tags = updatedProduct.Tags;
                product.OtherPhotos = updatedProduct.OtherPhotos;
                product.Stock = updatedProduct.Stock;
                product.Enabled = updatedProduct.Enabled;

                product.SubcategoryId = updatedProduct.SubcategoryId;

                product.Modified = DateTime.UtcNow;
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }


        //Users

        public static User FindUserById(int id)
        {
            using (var db = new ShopDatabase())
            {
                var user = (from c in db.Users
                                where c.UserId == id
                                select c).FirstOrDefault();
                return user;
            }
        }
        public static User FindUserByEmail(string email)
        {
            using (var db = new ShopDatabase())
            {
                var user = (from c in db.Users
                            where c.Email == email
                            select c).FirstOrDefault();
                return user;
            }
        }
        public static void AddUser(User user)
        {
            if (user == null)
                return;
            using (var db = new ShopDatabase())
            {
                db.Users.Add(user);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return;
                }
            }
        }
        public static void UpdateUser(User updatedUser)
        {
            using (var db = new ShopDatabase())
            {
                var user = (from c in db.Users
                            where c.UserId == updatedUser.UserId
                            select c).FirstOrDefault();
                if (user == null)
                    return;
                user.Name = updatedUser.Name;
                user.Surname = updatedUser.Surname;
                user.Email = updatedUser.Email;
                user.Role = updatedUser.Role;
                user.Phone = updatedUser.Phone;
                user.Password = updatedUser.Password;
                user.Address = updatedUser.Address;
                user.Enabled = updatedUser.Enabled;

                user.Modified = DateTime.UtcNow;
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }


        // Orders

        public static Order FindOrderById(int id)
        {
            using (var db = new ShopDatabase())
            {
                var order = (from o in db.Orders
                            where o.OrderId == id
                            select o).FirstOrDefault();
                return order;
            }
        }
        public static Order FindOrderByCode(string code)
        {
            using (var db = new ShopDatabase())
            {
                var order = (from o in db.Orders
                            where o.Code == code
                            select o).FirstOrDefault();
                return order;
            }
        }
        public static void AddOrder(Order order)
        {
            if (order == null)
                return;
            order.DateOfOrder = DateTime.UtcNow;
            using (var db = new ShopDatabase())
            {
                db.Orders.Add(order);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return;
                }
            }
        }
        public static void UpdateOrder(Order updatedOrder)
        {
            using (var db = new ShopDatabase())
            {
                var order = (from o in db.Orders
                            where o.OrderId == updatedOrder.OrderId
                            select o).FirstOrDefault();
                if (order == null)
                    return;
                order.Code = updatedOrder.Code;
                order.Amount = updatedOrder.Amount;
                order.Address = updatedOrder.Address;
                order.PaymentMethod = updatedOrder.PaymentMethod;
                order.Status = updatedOrder.Status;
                order.ClientName = updatedOrder.ClientName;
                order.ClientSurname = updatedOrder.ClientSurname;
                order.ClientEmail = updatedOrder.ClientEmail;
                order.ClientPhone = updatedOrder.ClientPhone;
                order.ShippingType = updatedOrder.ShippingType;
                order.ShippingInfo = updatedOrder.ShippingInfo;
                order.Comments = updatedOrder.Comments;

                order.UserId = updatedOrder.UserId;
                order.ProductOrders = updatedOrder.ProductOrders;

                order.Modified = DateTime.UtcNow;
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
            }
        }
    }
}
