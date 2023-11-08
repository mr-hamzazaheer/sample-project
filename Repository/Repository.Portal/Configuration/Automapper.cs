global using Model.Portal;

namespace Repository.Portal.Configuration
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<CreateUserDto, AppUser>().ForMember(z => z.DOB, opt => opt.MapFrom(s => s.DOB.DbDate(false)))
                .ForMember(z => z.EntryTime, opt => opt.MapFrom(s => DateTime.UtcNow))
            .ForMember(z => z.CreatedAt, opt => opt.MapFrom(s => DateTime.UtcNow));

            CreateMap<UpdateUserDto, AppUser>()
                .ForMember(z => z.DOB, opt => opt.MapFrom(s => s.DOB.DbDate(false)))
                .ForMember(z => z.UpdatedAt, opt => opt.MapFrom(s => DateTime.UtcNow));
        }
    }
}
