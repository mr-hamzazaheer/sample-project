using Repository.Mobile;

namespace API.Controllers.Mobile
{
    [ApiExplorerSettings(GroupName = "mobile")]
    [Route("api/m/v1/user")]
    public class UserController : BaseController
    {
        private readonly IAppUser _appUserRepo;
        public UserController(IAppUser appUserRepo)
        {
            _appUserRepo = appUserRepo;
        }
        [HttpGet("get-users")]
        [AllowAnonymous]
        public async Task<Response> GetUsers() => await _appUserRepo.GetUsers();
    }
}
