global using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities
{
    public abstract class BaseEntity
    {
        [Key, Required]
        public int Id { get; set; }
    }
}
