﻿using Application.DTOs.UserDTOs;
using AutoMapper;
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

            CreateMap<UserDTO, UserVM>();
        }
    }
}
