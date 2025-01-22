using Application.DTOs.AdminDTOs;
using Application.DTOs.ConversationDTOs;
using Application.DTOs.GroupDTOs;
using Application.DTOs.UserDTOs;
using AutoMapper;
using ChatAppAPI.ViewModels.ChatVMs;
using ChatAppAPI.ViewModels.ForAdminVMs;
using ChatAppAPI.ViewModels.GroupVMs;
using ChatAppAPI.ViewModels.UserVMs;

namespace ChatAppAPI.Mappings
{
    public class ViewModelToDtoProfile : Profile
    {
        public ViewModelToDtoProfile()
        {
            CreateMap<RegisterVM, RegisterDTO>();
            CreateMap<LoginVM, LoginDTO>();
            CreateMap<UpdateUserVM, UpdateUserDTO>();
            CreateMap<ForgotPasswordVM, ForgotPasswordDTO>();
            CreateMap<ResetPasswordVM, ResetPasswordDTO>();
            CreateMap<ChangePasswordVM, ChangePasswordDTO>();
            CreateMap<ChangeRoleVM, ChangeRoleDTO>();
            CreateMap<SendMesaageVM, SendMessageDTO>();
            CreateMap<EditMessageVM, EditMessageDTO>();
            CreateMap<CreateGroupVM, CreateGroupDTO>();

            CreateMap<UserDTO, UserVM>();
        }
    }
}
