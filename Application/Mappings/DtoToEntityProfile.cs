using Application.DTOs.UserDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class DtoToEntityProfile : Profile
    {
        public DtoToEntityProfile()
        {
            CreateMap<RegisterDTO, AppUser>();
            CreateMap<UpdateUserDTO, AppUser>()
                .ForAllMembers(opts => opts.Condition((src, dst, srcMember) => srcMember is not null));

            CreateMap<AppUser, UserDTO>().ReverseMap();
        }
    }
}
