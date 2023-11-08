global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Middleware;
global using Shared;
using Model.Portal;
using Repository.Portal;

namespace API.Controllers.Portal
{
    [ApiExplorerSettings(GroupName = "portal")]
    [Route("api/v1/user")]
    public class UserController : BaseController
    {
        private readonly IAppUser _appUserRepo;
        public UserController(IAppUser appUserRepo)
        {
            _appUserRepo = appUserRepo;
        }

        [HttpPost("add-user")]
        [AllowAnonymous]
        public async Task<Response> AddUser(CreateUserDto createUserDto) => await _appUserRepo.AddUser(createUserDto);


        [HttpPost("update-user")]
        public async Task<Response> UpdateUser(UpdateUserDto updateUserDto) => await _appUserRepo.UpdateUser(updateUserDto, HttpContext.UserProfile());

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<Response> Login(AuthDto authDto) => await _appUserRepo.GetLogin(authDto);

    }
}