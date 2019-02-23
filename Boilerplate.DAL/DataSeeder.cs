using System;
using System.Linq;
using Boilerplate.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace Boilerplate.DAL
{
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.Users.Any())
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@project.local",
                    Email = "admin@project.local",
                };

                var result = userManager.CreateAsync(adminUser, "AdminPassword2018").Result;
                if (!result.Succeeded)
                    throw new Exception($"Error user creation: {string.Join(",", result.Errors)}");
            }
        }
    }
}
