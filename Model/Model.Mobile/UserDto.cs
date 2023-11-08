global using Shared;
using System.Text.Json.Serialization;

namespace Model.Mobile
{
    public class BaseUserDto
    {
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
    }
    public class GetUserDto : BaseUserDto
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Code { get { return Id.ToString().Encode(); } }
        [JsonIgnore]
        public bool IsActive { get; set; }
        public string Name { get; set; }
    }
}
