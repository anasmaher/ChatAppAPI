using Application.DTOs.ResultsDTOs;
using Application.DTOs.UserDTOs;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private readonly IFileValidatorService fileValidatorService;
        private readonly ITokenService tokenService;
        private readonly IEmailService emailService;
        private readonly IUrlService urlService;

        public UserService(
            UserManager<AppUser> userManager,
            IMapper mapper,
            IFileStorageService fileStorageService,
            IFileValidatorService fileValidatorService, 
            ITokenService tokenService,
            IEmailService emailService,
            IUrlService urlService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
            this.fileValidatorService = fileValidatorService;
            this.tokenService = tokenService;
            this.emailService = emailService;
            this.urlService = urlService;
        }

        public async Task<ServiceResult> LoginUserAsync(LoginDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null || !await userManager.CheckPasswordAsync(user, model.Password))
                return new ServiceResult(false, ["Incorrect data"]);

            // get tokens: access and refresh
            AuthenticationResult tokens = await tokenService.GenerateTokenAsync(user);

            return new ServiceResult(true, data: tokens);
        }

        public async Task<ServiceResult> RegisterUserAsync(RegisterDTO model)
        {
            var existEmail = await userManager.FindByEmailAsync(model.Email);
            if (existEmail is not null)
                return new ServiceResult(false, ["Email is taken"]);

            var user = mapper.Map<AppUser>(model);
            user.UserName = model.Email;

            // Deal with photo and mapping
            if (model.Photo is not null)
            {
                if (!fileValidatorService.IsValidFileSignature(model.Photo))
                    return new ServiceResult(false, ["Photo is not valid"]);

                var photoPath = await fileStorageService.SaveFileAsync(model.Photo);

                user.PhotoFilePath = photoPath;
            }
            
            var res = await userManager.CreateAsync(user, model.Password);

            if (res.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");

                var userDTO = mapper.Map<UserDTO>(user);

                return new ServiceResult(true, data: userDTO);
            }
            else
                return new ServiceResult(false, res.Errors.Select(x => x.Description).ToList());
        }

        public async Task<ServiceResult> RemoveUserAsync(LoginDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null || !await userManager.CheckPasswordAsync(user, model.Password)) 
                return new ServiceResult(false, ["Incorrect data"]);

            var res = await userManager.DeleteAsync(user);

            if (res.Succeeded)
                return new ServiceResult(true, data: "Account removed");
            else
                return new ServiceResult(false, res.Errors.Select(x => x.Description).ToList());
        }

        public async Task<ServiceResult> UpdateUserAsync(string userId, UpdateUserDTO model)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (string.IsNullOrWhiteSpace(userId) || user is null)
                return new ServiceResult(false, ["User is not found"]);

            // map and update
            mapper.Map(model, user);
            var res = await userManager.UpdateAsync(user);

            if (res.Succeeded)
            {
                var userDTO = mapper.Map<UserDTO>(user);
                return new ServiceResult(true, data: userDTO);
            }
            else
                return new ServiceResult(false, res.Errors.Select(x => x.Description).ToList());
        }

        public async Task<ServiceResult> GetUserInfoAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
                return new ServiceResult(false, ["User was not found"]);
            else
            {
                var userDTO = mapper.Map<UserDTO>(user);
                return new ServiceResult(true, data: userDTO);
            }
        }

        public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return new ServiceResult(true, ["An email was sent to your email if it's registered"]);

            // get resetpassword token and url sent to user's email
            string resetPasswordToken = await userManager.GeneratePasswordResetTokenAsync(user);
            string resetUrl = urlService.GenerateResetPasswordUrl(resetPasswordToken);

            // send email
            try
            {
                EmailMetadata emailMetadata = new(
                    model.Email,
                    "Reset password",
                    $"Reset your password using this link: <a href='{resetUrl}'>Reset Password</a>"
                );

                emailService.SendEmailAsync(emailMetadata);
                return new ServiceResult(true, data: "An email was sent to your email if it's registered");
            }
            catch (Exception ex)
            {
                return new ServiceResult(false, [ex.Message]);
            }
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            var res = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (res.Succeeded)
            {
                // logout all devices
                user.TokenVersion++;

                await userManager.UpdateAsync(user);

                return new ServiceResult(true);
            }

            return new ServiceResult(false, res.Errors.Select(x => x.Description).ToList());
        }

        public async Task<ServiceResult> ChangePasswordAsync(string userId, ChangePasswordDTO model)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user is null || !await userManager.CheckPasswordAsync(user, model.OldPassword))
                return new ServiceResult(false, ["Incorrect password"]);

            var res = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);


            if (!res.Succeeded)
                return new ServiceResult(false, res.Errors.Select(x => x.Description).ToList());

            // logout all devices
            user.TokenVersion++;
            await userManager.UpdateAsync(user);

            return new ServiceResult(true, data: "Password changed");
        }

        public async Task<ServiceResult> LogOutAllAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
                return new ServiceResult(false, ["User was not found"]);

            // change token version to invalid all tokens
            user.TokenVersion++;
            await userManager.UpdateAsync(user);

            return new ServiceResult(true, data: "Logged out on all devices");
        }

        public async Task<ServiceResult> LogOutSingleAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
                return new ServiceResult(false, ["User was not found"]);

            if (token is null)
                return new ServiceResult(false, ["Token is invalid"]);

            // revoke current device token and refresh token
            await tokenService.RevokeTokenAsync(token);
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.Now;

            await userManager.UpdateAsync(user);

            return new ServiceResult(true, data: "Logged out on current device only");
        }

        public async Task<ServiceResult> GetOrCreateExternalUserAsync(string userEmail, string userName, string userId)
        {
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user is null)
            {
                user = new AppUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = userName.Split(' ')[0] ?? "First",
                    LastName = userName.Split(" ")[1] ?? "Last"
                };

                var res = await userManager.CreateAsync(user);
                if (!res.Succeeded)
                    return new ServiceResult(false, res.Errors.Select(x => x.Description).ToList());

                await userManager.AddToRoleAsync(user, "User");
            }
          
            return new ServiceResult(true, data: user);
        }
    }
}
