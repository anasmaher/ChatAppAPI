using Application.DTOs.AdminDTOs;
using Application.DTOs.UserDTOs;
using AutoMapper;
using ChatAppAPI.ViewModels.ForAdminVMs;
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

            CreateMap<UserDTO, UserVM>();
        }
    }
}
