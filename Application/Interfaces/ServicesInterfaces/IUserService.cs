﻿using Application.DTOs.ResultsDTOs;
using Application.DTOs.UserDTOs;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IUserService
    {
        public Task<ServiceResult> RegisterUserAsync(RegisterDTO model);

        public Task<ServiceResult> LoginUserAsync(LoginDTO model);
    }
}