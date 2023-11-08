namespace Model.Portal
{
    public class BaseUserDto
    {
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
    }
    public class CreateUserDto : BaseUserDto
    {
        public string Name { get; set; }

    }
    public class UpdateUserDto : BaseUserDto
    {

    }
}
