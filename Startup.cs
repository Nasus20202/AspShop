using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRazorPages();
            services.AddControllersWithViews();
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
                options.Cookie.Name = "NasusCookie";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.AccessDeniedPath = new PathString("/Account/Denied");
            });
            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ShopDatabase.ConnectionString = !env.IsDevelopment() ? "production_string" : "Server = 127.0.0.1; Database = shop; Uid = nasus; Pwd = KNu17g2003cdVLO;" ;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Populate PC Parts
            using (var context = new ShopDatabase())
            {
                var category = new Category
                {
                    Name = "Podzespo³y Komputerowe",
                    About = "PC Parts",
                    Subcategories = new List<Subcategory>()
                    {
                        new Subcategory { Name = "Procesory", About="CPUs", Tags=""},
                        new Subcategory { Name = "Karty graficzne", About="GPU", Tags=""},
                        new Subcategory { Name = "P³yty g³ówne", About="MOBOs", Tags=""}
                    }

                };
                context.Add(category);
                //context.SaveChanges();
            }
            // Populate Keyboards
            using (var context = new ShopDatabase())
            {
                var category = new Category
                {
                    Name = "Klawiatury",
                    About = "Keyboards",
                    Subcategories = new List<Subcategory>()
                    {
                        new Subcategory { Name = "Klawiatury 60%", About="60", Tags=""},
                        new Subcategory { Name = "Klawiatury TKL", About="80", Tags=""},
                        new Subcategory { Name = "Klawiatury 100%", About="100", Tags=""}
                    }

                };
                context.Add(category);
                //context.SaveChanges();
            }
            // Move Subcategories between Categories
            using (var context = new ShopDatabase())
            {
                var category = (from c in context.Categories
                                where c.CategoryId == 2
                                select c).FirstOrDefault();
                var subcategory = (from c in context.Subcategories
                                   where c.SubcategoryId == 6
                                   select c).FirstOrDefault();
                subcategory.Category = category;
                //context.SaveChanges();
            }
            // Add products
            using (var context = new ShopDatabase())
            {
                var subcategory = context.Subcategories
                                .Single(s => s.SubcategoryId == 4);
                context.Entry(subcategory)
                    .Collection(s => s.Products)
                    .Load();
                var product = new Product { Name = "GK61", Brand= "HK Gaming", Code = "gk61", Price = 25000, Tags="", About = "", Photo = "" };
                subcategory.Products.Add(product);
                //context.SaveChanges();
            }


                app.UseStaticFiles();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            var db = new ShopDatabase();
            db.Database.EnsureCreated();

            app.UseStatusCodePagesWithRedirects("/Error/{0}");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                "default", "{controller=Home}/{action=Index}/{id?}");
            });         
        }
    }
}
