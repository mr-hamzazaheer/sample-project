namespace Infrastructure.Entities
{
    public class Module : BaseEntity
    {
        public int ParentId { get; set; } = 0;
        public string Name { get; set; }
        public string Detail { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int MenuId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
