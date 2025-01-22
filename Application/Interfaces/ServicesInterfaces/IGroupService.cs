using Application.DTOs.GroupDTOs;
using Application.DTOs.ResultsDTOs;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IGroupService
    {
        Task<ServiceResult> CreateGroupAsync(CreateGroupDTO createGroupDto, string creatorId);

        Task<ServiceResult> AddMemberAsync(Guid groupId, string memberId);

        Task<ServiceResult> RemoveMemberAsync(Guid groupId, string memberId);

        Task<ServiceResult> GetGroupAsync(Guid groupId);
    }
}
