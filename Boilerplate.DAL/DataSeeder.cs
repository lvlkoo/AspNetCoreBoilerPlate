using System;
using System.Linq;
using Boilerplate.DAL.Entities;
using Boilerplate.Models.Auth;
using Microsoft.AspNetCore.Identity;

namespace Boilerplate.DAL
{
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            if (!context.Users.Any())
            {
                var adminRole = new ApplicationRole
                {
                    Name = "Administrator",
                    Permissions = string.Join(",", Permission.GetAllPermissions())
                };

                var result = roleManager.CreateAsync(adminRole).Result;
                if (!result.Succeeded)
                    throw new Exception($"Error user creation: {string.Join(",", result.Errors)}");

                var adminUser = new ApplicationUser
                {
                    UserName = "admin@project.local",
                    Email = "admin@project.local",
                };

                result = userManager.CreateAsync(adminUser, "AdminPassword2018").Result;
                if (!result.Succeeded)
                    throw new Exception($"Error user creation: {string.Join(",", result.Errors)}");

                result = userManager.AddToRoleAsync(adminUser, adminRole.Name).Result;
                if (!result.Succeeded)
                    throw new Exception($"Error user creation: {string.Join(",", result.Errors)}");
            }
        }
    }
}
