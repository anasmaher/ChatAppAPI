using Application.DTOs.ResultsDTOs;
using Application.DTOs.UserDTOs;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IUserService
    {
        public Task<ServiceResult> RegisterUserAsync(RegisterDTO model);

        public Task<ServiceResult> LoginUserAsync(LoginDTO model);

        public Task<ServiceResult> RemoveUserAsync(LoginDTO model);

        public Task<ServiceResult> UpdateUserAsync(string userId, UpdateUserDTO model);

        public Task<ServiceResult> GetUserInfoAsync(string userId);

        public Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDTO model);

        public Task<ServiceResult> ResetPasswordAsync(ResetPasswordDTO model);

        public Task<ServiceResult> ChangePasswordAsync(string userId, ChangePasswordDTO model);

        public Task<ServiceResult> LogOutSingleAsync(string userId, string token);

        public Task<ServiceResult> LogOutAllAsync(string userId);

        public Task<ServiceResult> GetOrCreateExternalUserAsync(string userEmail, string userName, string userId);
    }
}
