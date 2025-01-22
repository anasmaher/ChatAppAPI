using Application.DTOs.GroupDTOs;
using Application.DTOs.ResultsDTOs;
using Application.Interfaces.ReposInterfaces;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ServiceResult> AddMemberAsync(Guid groupId, string memberId)
        {
            var group = await unitOfWork.ConversationRepo.GetSingleAsync(c => c.Id == groupId && c.IsGroup);

            if (group is null)
                return new ServiceResult(false, ["Group does not exist"]);

            var member = new ConversationMember
            {
                ConversationId = groupId,
                UserId = memberId,
                IsAdmin = false
            };

            await unitOfWork.ConversationMemberRepo.AddAsync(member);
            await unitOfWork.CommitAsync();

            return new ServiceResult(true, data: member);
        }

        public async Task<ServiceResult> CreateGroupAsync(CreateGroupDTO model, string creatorId)
        {
            var convo = mapper.Map<Conversation>(model);
            convo.CreatedAt = DateTime.Now;
            convo.Id = Guid.NewGuid();

            await unitOfWork.ConversationRepo.AddAsync(convo);

            var member = new ConversationMember
            {
                IsAdmin = true,
                ConversationId = convo.Id,
                UserId = creatorId,
            };
            await unitOfWork.ConversationMemberRepo.AddAsync(member);

            foreach (string id in model.MembersIds)
            {
                var newMember = new ConversationMember
                {
                    IsAdmin = false,
                    ConversationId = convo.Id,
                    UserId = id
                };

                await unitOfWork.ConversationMemberRepo.AddAsync(newMember);
            }

            await unitOfWork.CommitAsync();

            return new ServiceResult(true, data: model);
        }

        public async Task<ServiceResult> GetGroupAsync(Guid groupId)
        {
            var conversation = await unitOfWork.ConversationRepo.GetGroupWithMembersAsync(groupId);

            if (conversation is null)
                return new ServiceResult(false, ["Group does not exist"]);

            var groupDto = mapper.Map<GroupDTO>(conversation);

            return new ServiceResult(true, data: groupDto);
        }

        public Task<ServiceResult> RemoveMemberAsync(Guid groupId, string memberId)
        {
            throw new NotImplementedException();
        }
    }
}
