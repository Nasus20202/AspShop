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
                category.About = updatedCategory.About;


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
                subcategory.About = updatedSubcategory.About;
                subcategory.Tags = updatedSubcategory.Tags;


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
