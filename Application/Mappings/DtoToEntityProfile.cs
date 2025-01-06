using Application.DTOs.UserDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class DtoToEntityProfile : Profile
    {
        public DtoToEntityProfile()
        {
            CreateMap<RegisterDTO, AppUser>()
                .ForMember(dest => dest.PhotoFilePath, opt => opt.Ignore());

            CreateMap<AppUser, RegisterDTO>();
        }
    }
}
