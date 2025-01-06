using Application.DTOs.ResultsDTOs;
using Application.DTOs.UserDTOs;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly IFileValidatorService fileValidatorService;

        public UserService(
            UserManager<AppUser> userManager,
            IMapper mapper,
            IFileStorageService fileStorageService,
            IFileValidatorService fileValidatorService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
            this.fileValidatorService = fileValidatorService;
        }

        public async Task<ServiceResult> RegisterUserAsync(RegisterDTO model)
        {
            var existEmail = await userManager.FindByEmailAsync(model.Email);
            if (existEmail is not null)
                return new ServiceResult(false, "Email is taken");

            var user = mapper.Map<AppUser>(model);
            user.UserName = model.Email;

            // Deal with photo and mapping
            if (model.Photo is not null)
            {
                if (!fileValidatorService.IsValidFileSignature(model.Photo))
                    return new ServiceResult(false, "Photo is not valid");

                var photoPath = await fileStorageService.SaveFileAsync(model.Photo);

                user.PhotoFilePath = photoPath;
            }
            
            // Create user
            var res = await userManager.CreateAsync(user, model.Password);

            if (res.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");

                return new ServiceResult(true, data: user);
            }
            else
                return new ServiceResult(false, string.Join(", ", res.Errors));
        }
    }
}
