﻿namespace GoFishIng.Data
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using GoFishIng.Data.Common.Models;
    using GoFishIng.Data.Models;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Permit> Permits { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Trip> Trips { get; set; }

        public override int SaveChanges() => this.SaveChanges(true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            this.SaveChangesAsync(true, cancellationToken);

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            ConfigureUserIdentityRelations(builder);

            EntityIndexesConfiguration.Configure(builder);

            var entityTypes = builder.Model.GetEntityTypes().ToList();

            // Set global query filter for not deleted entities only
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { builder });
            }

            // Disable cascade delete
            var foreignKeys = entityTypes
                .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private static void ConfigureUserIdentityRelations(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>().HasOne(u => u.Cart)
                .WithOne(c => c.User).HasForeignKey<Cart>(ca => ca.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cart>().HasOne(c => c.User)
                .WithOne(u => u.Cart).HasForeignKey<ApplicationUser>(u => u.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Permit>().HasOne(p => p.Cart)
                .WithOne(c => c.Permit).HasForeignKey<Cart>(ca => ca.PermitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cart>().HasOne(c => c.Permit)
                .WithOne(p => p.Cart).HasForeignKey<Permit>(p => p.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>().HasOne(u => u.Order)
                .WithOne(o => o.User).HasForeignKey<Order>(or => or.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>().HasOne(o => o.User)
                .WithOne(u => u.Order).HasForeignKey<ApplicationUser>(us => us.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>().HasOne(u => u.Permit)
                .WithOne(p => p.User).HasForeignKey<Permit>(pe => pe.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Permit>().HasOne(p => p.User)
                .WithOne(u => u.Permit).HasForeignKey<ApplicationUser>(us => us.PermitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Permit>().HasOne(p => p.Order)
                .WithOne(o => o.Permit).HasForeignKey<Order>(or => or.PermitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>().HasOne(o => o.Permit)
                .WithOne(p => p.Order).HasForeignKey<Permit>(pe => pe.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IDeletableEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        private void ApplyAuditInfoRules()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}