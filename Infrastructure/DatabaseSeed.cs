using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    internal class DatabaseSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppRole>().HasData(new List<AppRole>
            {
                new AppRole
                {
                    Id = new Guid("F9679CC2-6D06-4859-8D25-406A6004198E"),
                    Name = "Admin",
                    IsSystem = true,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            });
        }
    }
}
