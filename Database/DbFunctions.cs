using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class DbFunctions
    {
        public static Category FindCategoryById(int id)
        {
            using(var db = new ShopDatabase())
            {
                var category = (from c in db.Categories
                                where c.Id == id
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
        public static User FindUserById(int id)
        {
            using (var db = new ShopDatabase())
            {
                var user = (from c in db.Users
                                where c.Id == id
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
                            where c.Id == updatedUser.Id
                            select c).FirstOrDefault();
                if (user == null)
                    return;
                user.Name = updatedUser.Name;
                user.Surname = updatedUser.Surname;
                user.Email = updatedUser.Email;
                user.Phone = updatedUser.Phone;
                user.Password = updatedUser.Password;
                user.Address = updatedUser.Address;
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
    }
}
