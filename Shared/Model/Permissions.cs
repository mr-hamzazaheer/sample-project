using System.Text.Json.Serialization;

namespace Shared.Model
{
    public class MenuAccessLevel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }
        public string IconClass { get; set; }
        [JsonIgnore]
        public bool IsActive { get; set; }
        [JsonIgnore]
        public int Level { get; set; }
        [JsonIgnore]
        public string OrderSequence { get; set; }
        public bool IsPortal { get; set; }
    }
    public class ModuleAccessLevel
    {
        public string Path { get; set; }
        public string RoleId { get; set; }
    }
}
