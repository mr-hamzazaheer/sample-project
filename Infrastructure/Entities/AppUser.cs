using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
