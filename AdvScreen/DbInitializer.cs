using Dal.Data;
using Dal.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvScreen
{
    public static class DbInitializer
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                var _userManager =
                         serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var _roleManager =
                         serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                if (!context.Users.Any(usr => usr.UserName == "admin@ad.me"))
                {
                    var user = new ApplicationUser()
                    {
                        UserName = "admin@ad.me",
                        Email = "admin@ad.me",
                        EmailConfirmed = true,
                    };

                    var userResult = _userManager.CreateAsync(user, "P@ssw0rd").Result;
                }

                if (!_roleManager.RoleExistsAsync("Admin").Result)
                {
                    var role = _roleManager.CreateAsync
                               (new IdentityRole { Name = "Admin" }).Result;
                }

                var adminUser = _userManager.FindByNameAsync("admin@ad.me").Result;
                var userRole = _userManager.AddToRolesAsync
                               (adminUser, new string[] { "Admin" }).Result;


                if (!context.AdvertisementStatuses.Any(s => s.Name == AdvertisementStatusEnum.Active.ToString() ))
                {
                    context.AdvertisementStatuses.Add(new AdvertisementStatus(){Name = "Created",NameRu = "Создан"});
                    context.AdvertisementStatuses.Add(new AdvertisementStatus() { Name = "InModeration", NameRu = "Модерация"});
                    context.AdvertisementStatuses.Add(new AdvertisementStatus() { Name = "ForPayment", NameRu = "К оплате" });
                    context.AdvertisementStatuses.Add(new AdvertisementStatus() { Name = "Wating", NameRu = "В ожидании" });
                    context.AdvertisementStatuses.Add(new AdvertisementStatus() { Name = "Active", NameRu = "Активный" });
                    context.AdvertisementStatuses.Add(new AdvertisementStatus() { Name = "Finished", NameRu = "Завершен" });
                }

                context.SaveChanges();
            }
        }
    }
}
