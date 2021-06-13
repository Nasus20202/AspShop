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
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".Nasus.Session";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ShopDatabase.ConnectionString = !env.IsDevelopment() ? "production_string" : "Server = 127.0.0.1; Database = shop; Uid = nasus; Pwd = KNu17g2003cdVLO;" ;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            var db = new ShopDatabase();
            db.Database.EnsureCreated();

            // populate db

            //Order order = db.Orders.Where(o => o.OrderId == 1).FirstOrDefault();
            /*Product product1 = db.Products.Where(p => p.ProductId == 1).FirstOrDefault();
            Product product2 = db.Products.Where(p => p.ProductId == 2).FirstOrDefault();
            ProductOrder po1 = new ProductOrder();
            ProductOrder po2 = new ProductOrder();


            db.Entry(product1)
                .Collection(o => o.ProductOrders)
                .Load();
            db.Entry(product2)
                .Collection(o => o.ProductOrders)
                .Load();

            po1.OrderId = order.OrderId;
            po1.ProductId = product1.ProductId;
            po2.OrderId = order.OrderId;
            po2.ProductId = product2.ProductId;
            order.ProductOrders.Add(po1);
            order.ProductOrders.Add(po2);
            product1.ProductOrders.Add(po1);
            product2.ProductOrders.Add(po2);*/
            //db.SaveChanges();
            //db.Entry(order)
            //    .Collection(o => o.ProductOrders)
            //    .Load();

            /*foreach (ProductOrder po in order.ProductOrders)
            {
                Product p = db.Products.Where(p => p.ProductId == po.ProductId).FirstOrDefault();
                Console.WriteLine(p.Name);
            }*/

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
