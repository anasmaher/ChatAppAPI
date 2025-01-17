using Application.DTOs.AdminDTOs;
using Application.DTOs.RelationshipDTOs;
using Application.DTOs.SignalrDTOs;
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

            CreateMap<AppUser, UserForAdminDTO>();

            CreateMap<UserRelationship, FriendRequestDTO>()
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.ActionUserId))
                .ForMember(dest => dest.SenderFirstName, opt => opt.MapFrom(src => src.ActionUser.FirstName))
                .ForMember(dest => dest.SenderLastName, opt => opt.MapFrom(src => src.ActionUser.LastName));

            CreateMap<UserRelationship, RelationMemberDTO>()
                .ForMember(dest => dest.FriendId, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var currentUserId = context.Items["CurrentUserId"] as string;
                    return src.User1Id != currentUserId ? src.User1Id : src.User2Id;
                }))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var currentUserId = context.Items["CurrentUserId"] as string;
                    return src.User1Id != currentUserId ? src.User1.FirstName : src.User2.FirstName;
                }))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    var currentUserId = context.Items["CurrentUserId"] as string;
                    return src.User1Id != currentUserId ? src.User1.LastName : src.User2.LastName;
                }));

            CreateMap<Notification, NotificationDTO>()
                .ForMember(dest => dest.SenderUsername, opt => opt.MapFrom(src => src.SenderUser.UserName));
        }
    }
}
