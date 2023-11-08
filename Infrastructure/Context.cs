
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class PracticeContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public PracticeContext(DbContextOptions items) : base(items)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            DatabaseSeed.Seed(builder);
            base.OnModelCreating(builder);
            builder.Entity<AppUser>(b =>
            {
                b.Property(u => u.Id).HasDefaultValueSql("newsequentialid()");
            });
            builder.Entity<AppRole>(b =>
            {
                b.Property(u => u.Id).HasDefaultValueSql("newsequentialid()");
            });

        }
        public virtual DbSet<AppUser> AppUsers { get; set; }

        public virtual DbSet<AppRole> AppRoles { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Module> Modules { get; set; }

    }
}
