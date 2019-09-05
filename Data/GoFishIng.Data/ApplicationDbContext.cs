namespace GoFishIng.Data
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


            builder.Entity<TripUser>()
                .HasKey(x => new { x.TripId, x.UserId });

            builder.Entity<TripUser>()
            .HasOne(t => t.Trip)
            .WithMany(tu => tu.TripUsers)
            .HasForeignKey(t => t.TripId);

            builder.Entity<TripUser>()
                .HasOne(u => u.User)
                .WithMany(t => t.UserTrips)
                .HasForeignKey(u => u.UserId);

            builder.Entity<Cart>().HasMany(x => x.Trips)
                .WithOne(x => x.Cart).HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Cart>().HasMany(x => x.Products)
                .WithOne(x => x.Cart).HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>().HasMany(c=>c.Carts)
                .WithOne(c => c.User).HasForeignKey(ca => ca.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>().HasMany(o => o.Orders)
                .WithOne(c => c.User).HasForeignKey(or => or.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>().HasMany(p => p.Permits)
                .WithOne(c => c.User).HasForeignKey(pe => pe.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cart>().HasMany(p=>p.Permits)
                .WithOne(p => p.Cart).HasForeignKey(pe => pe.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>().HasMany(p => p.Permits)
                .WithOne(p => p.Order).HasForeignKey(pe => pe.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cart>().HasOne(o => o.Order)
                .WithOne(c => c.Cart).HasForeignKey<Order>(or => or.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>().HasOne(c => c.Cart)
                .WithOne(o => o.Order).HasForeignKey<Cart>(ca => ca.OrderId)
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
