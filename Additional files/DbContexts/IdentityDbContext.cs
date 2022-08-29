using Marvin.IDP.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marvin.IDP.DbContexts
{
    public class IdentityDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<UserClaim> UserClaims { get; set; }         

        public IdentityDbContext(
          DbContextOptions<IdentityDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Subject)
            .IsUnique();

            modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                    Password = "password",
                    Subject = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    UserName = "David",
                    Active = true
                },
                new User()
                {
                    Id = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                    Password = "password",
                    Subject = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    UserName = "Emma",
                    Active = true
                });

            modelBuilder.Entity<UserClaim>().HasData(
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                 Type = "given_name",
                 Value = "David"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                 Type = "family_name",
                 Value = "Flagg"
             }, 
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                 Type = "country",
                 Value = "nl"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                 Type = "role",
                 Value = "FreeUser"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                 Type = "given_name",
                 Value = "Emma"
             },
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                 Type = "family_name",
                 Value = "Flagg"
             }, 
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                 Type = "country",
                 Value = "be"
             }, 
             new UserClaim()
             {
                 Id = Guid.NewGuid(),
                 UserId = new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"),
                 Type = "role",
                 Value = "PayingUser"
             });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // get updated entries
            var updatedConcurrencyAwareEntries = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified)
                    .OfType<IConcurrencyAware>();

            foreach (var entry in updatedConcurrencyAwareEntries)
            {
                entry.ConcurrencyStamp = Guid.NewGuid().ToString();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
