global using Model.Mobile;

namespace Repository.Mobile.Configuration
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<AppUser, GetUserDto>()
                .ForMember(z => z.DOB, opt => opt.MapFrom(s => s.DOB.ViewDate(false)));
        }
    }
}
