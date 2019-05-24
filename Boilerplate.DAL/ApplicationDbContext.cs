using System;
using Boilerplate.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
    {
        public DbSet<FileUpload> FileUploads { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
        }
    }
}   
