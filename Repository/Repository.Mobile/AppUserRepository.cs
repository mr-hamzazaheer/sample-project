global using AutoMapper;
global using Infrastructure;
global using Infrastructure.Entities;
global using Shared;

namespace Repository.Mobile
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


        public Task<Response> GetUsers()
        {
            #region ByAutomapper
            //var dbUsers = _db.AppUsers.Where(z => z.IsActive).AsQueryable();
            //var userDto = _mapper.Map<List<GetUserDto>>(dbUsers);
            #endregion

            var userDto = _db.AppUsers.Where(z => z.IsActive)
                .Select(s => new GetUserDto()
                {
                    Id = s.Id,
                    IsActive = s.IsActive,
                    Name = s.Name,
                    DOB = s.DOB.ViewDate(false),
                    PhoneNumber = s.PhoneNumber,
                }).AsQueryable();
            _response.Data = userDto;
            return Task.FromResult(_response);
        }

    }
}
