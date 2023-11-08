namespace Infrastructure.Entities
{
    public class Menu : BaseEntity
    {
        [Required]
        public int ParentId { get; set; } = 0;
        [Required]
        public string Name { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }
        public string IconClass { get; set; }
        public bool IsPortal { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
