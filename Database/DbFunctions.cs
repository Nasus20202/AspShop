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
    }
}
