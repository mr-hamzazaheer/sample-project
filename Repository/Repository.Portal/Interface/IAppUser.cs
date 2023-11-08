namespace Repository.Portal
{
    public interface IAppUser
    {
        Task<Response> AddUser(CreateUserDto createUserDto);
        Task<Response> UpdateUser(UpdateUserDto updateUserDto, UserProfile profile);
        Task<Response> GetLogin(AuthDto authDto);
    }
}
