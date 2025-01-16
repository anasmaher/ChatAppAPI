using Application.DTOs.AdminDTOs;
using Application.DTOs.ResultsDTOs;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IAdminService
    {
        Task<ServiceResult> GetAllUsersAsync();

        Task<ServiceResult> AssignRoleAsync(string id, ChangeRoleDTO model);

        Task<ServiceResult> RemoveUserAdminAsync(string id);
    }
}
