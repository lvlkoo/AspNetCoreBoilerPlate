using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Boilerplate.Entities;
using Boilerplate.Entities.Chat;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Boilerplate.EF
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

            foreach (var entity in builder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entity.ClrType))
                {
                    entity.AddProperty(nameof(ISoftDeletable.IsDeleted), typeof(bool));

                    builder
                        .Entity(entity.ClrType)
                        .HasQueryFilter(BuildQueryFilter(entity.ClrType));
                }
            }


            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

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
                charUser.HasKey(cu => new {cu.UserId, cu.ChannelId});

                charUser.HasOne(cu => cu.User)
                    .WithMany(u => u.ChatUsers)
                    .HasForeignKey(cu => cu.UserId)
                    .IsRequired();

                charUser.HasOne(cu => cu.Channel)
                    .WithMany(c => c.ChatUsers)
                    .HasForeignKey(cu => cu.ChannelId)
                    .IsRequired();
            });
        }

        public void Restore(ISoftDeletable entity)
        {
            var entry = ChangeTracker.Entries().First(en => en.Entity == entity);
            if ((bool) entry.Property(nameof(ISoftDeletable.IsDeleted)).CurrentValue == true)
                entry.Property(nameof(ISoftDeletable.IsDeleted)).CurrentValue = false;
        }

        public void RestoreRange(IEnumerable<ISoftDeletable> entities)
        {
            foreach (var entity in entities)
                Restore(entity);
        }


        private static LambdaExpression BuildQueryFilter(Type type)
        {
            var propertyMethod = typeof(Microsoft.EntityFrameworkCore.EF).GetMethod(nameof(Microsoft.EntityFrameworkCore.EF.Property), BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(typeof(bool));
            var pram = Expression.Parameter(type, "it");
            var prop = Expression.Call(propertyMethod, pram, Expression.Constant(nameof(ISoftDeletable.IsDeleted)));
            var condition = Expression.MakeBinary(ExpressionType.Equal, prop, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, pram);
            return lambda;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void SetNull(EntityEntry entry, IForeignKey fk)
        {
            foreach (var property in fk.Properties)
                entry.Property(property.Name).CurrentValue = null;
        }


        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues[nameof(ISoftDeletable.IsDeleted)] = false;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues[nameof(ISoftDeletable.IsDeleted)] = true;
                        foreach (var navigationEntry in entry.Navigations.Where(n =>
                            !n.Metadata.IsDependentToPrincipal()))
                        {
                            if (navigationEntry is CollectionEntry collectionEntry)
                            {
                                collectionEntry.Load();
                                if (collectionEntry.CurrentValue == null)
                                    continue;

                                var collection = new List<EntityEntry>();

                                switch (collectionEntry.Metadata.ForeignKey.DeleteBehavior)
                                {
                                    case DeleteBehavior.SetNull:
                                        foreach (var entity in collectionEntry.CurrentValue)
                                            collection.Add(Entry(entity));

                                        foreach (var dependentEntry in collection)
                                            SetNull(dependentEntry, collectionEntry.Metadata.ForeignKey);

                                        break;
                                    case DeleteBehavior.Cascade:
                                        foreach (var entity in collectionEntry.CurrentValue)
                                            Remove(entity);

                                        break;
                                }
                            }
                            else
                            {
                                var dependentEntry = navigationEntry.CurrentValue;
                                if (dependentEntry != null)
                                    Remove(Entry(dependentEntry));
                            }
                        }

                        break;
                }
            }
        }
    }
}