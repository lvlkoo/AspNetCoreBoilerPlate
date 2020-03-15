using System;
using System.Linq;
using Boilerplate.EF;
using Boilerplate.Entities;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate.Services
{
    public static class DataSeeder
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
            var permissionsService = scope.ServiceProvider.GetService<IPermissionsService>();

            if (!context.Users.Any())
            {
                var adminRole = new ApplicationRole
                {
                    Name = "Administrator",
                    Permissions = string.Join(",", permissionsService.GetAllPermissions())
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
