using System;
using Boilerplate.DAL.Entities;
using Boilerplate.DAL.Entities.Chat;
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

        //chat
        public DbSet<ChatChannel> ChatChannels { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatMessageAttachment> ChatMessageAttachments { get; set; }
        public DbSet<ChatChannelUser> ChatChannelUsers { get; set; }

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

            //many-to-many ChatChannel <-> ApplicationUser with join table ChatChannelUser
            builder.Entity<ChatChannelUser>(charUser =>
            {
                charUser.HasKey(cu => new { cu.UserId, cu.Channeld});

                charUser.HasOne(cu => cu.User)
                    .WithMany(u => u.ChatUsers)
                    .HasForeignKey(cu => cu.UserId)
                    .IsRequired();

                charUser.HasOne(cu => cu.Channel)
                    .WithMany(c => c.ChatUsers)
                    .HasForeignKey(cu => cu.Channeld)
                    .IsRequired();
            });
        }
    }
}   
