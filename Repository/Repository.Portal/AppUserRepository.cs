global using AutoMapper;
global using Infrastructure;
global using Infrastructure.Entities;
global using Microsoft.EntityFrameworkCore;
global using Shared;

namespace Repository.Portal
{
    public class AppUserRepository : IAppUser
    {
        private PracticeContext _db { get; }
        private readonly IMapper _mapper;
        private readonly Response _response;

        public AppUserRepository(PracticeContext db, IMapper mapper, Response response)
        {
            _db = db;
            _mapper = mapper;
            _response = response;
        }
        public async Task<Response> AddUser(CreateUserDto createUserDto)
        {
            var user = _db.AppUsers.FirstOrDefault(z => z.Name == createUserDto.Name);
            if (user != null) throw new ApplicationException(Message.Exists);
            var dbUser = _mapper.Map<AppUser>(createUserDto);
            await _db.AppUsers.AddAsync(dbUser);
            await _db.SaveChangesAsync();
            return _response;
        }

        public async Task<Response> UpdateUser(UpdateUserDto updateUserDto, UserProfile profile)
        {
            var userId = new Guid(profile.Id);
            var dbUser = await _db.AppUsers.FirstOrDefaultAsync(z => z.Id == userId);
            if (dbUser == null)
                throw new KeyNotFoundException(Message.KeyNotFound("User"));
            _mapper.Map(updateUserDto, dbUser);  //Note: Only those fields will be update that exists in UpdateUserDto
            _db.AppUsers.Update(dbUser);
            await _db.SaveChangesAsync();
            return _response;

        }
        public async Task<Response> GetLogin(AuthDto authDto)
        {
            var dbUser = await _db.AppUsers.FirstOrDefaultAsync(z => z.Name == authDto.Name && z.PhoneNumber == authDto.PhoneNumber);
            if (dbUser == null)
                throw new KeyNotFoundException(Message.KeyNotFound("User"));
            _response.Data = dbUser.Id.ToString().GetToken();
            return _response;
        }
    }
}
