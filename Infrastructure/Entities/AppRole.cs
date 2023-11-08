using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public bool IsSystem { set; get; }
    }
}
