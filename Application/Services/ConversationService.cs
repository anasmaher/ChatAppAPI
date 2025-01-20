using Application.DTOs.ConversationDTOs;
using Application.DTOs.ResultsDTOs;
using Application.Interfaces.ReposInterfaces;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using ChatAppAPI.Hubs;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly UserManager<AppUser> userManager;

        public ConversationService(IMapper mapper, IUnitOfWork unitOfWork, IHubContext<ChatHub> hubContext, UserManager<AppUser> userManager)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.hubContext = hubContext;
            this.userManager = userManager;
        }

        public async Task<ServiceResult> GetMessagesForConversationAsync(Guid conversationId)
        {
            var convo = await unitOfWork.ConversationRepo
                .GetSingleAsync(c => c.Id == conversationId);

            var msgsDTO = mapper.Map<List<MessageDTO>>(convo.Messages);

            return new ServiceResult(true, data: msgsDTO);
        }

        public async Task<ServiceResult> GetOrCreateConversationAsync(string userId1, string userId2)
        {
            if (userId1 == null || userId2 == null)
                return new ServiceResult(false, ["one or both users does not exist"]);

            var existConvo = await unitOfWork.ConversationRepo.GetPrivateConversationAsync(userId1, userId2);

            if(existConvo != null)
            {
                var convoDTO = mapper.Map<ConversationDTO>(existConvo);
                
                return new ServiceResult(true, data: convoDTO);
            }

            var newConversation = new Conversation
            {
                Id = Guid.NewGuid(),
                Name = "New",
                IsGroup = false,
                CreatedAt = DateTime.UtcNow,
                Members = new List<ConversationMember>()
            };

            await unitOfWork.ConversationRepo.AddAsync(newConversation);
            await unitOfWork.CommitAsync();

            var member1 = new ConversationMember
            {
                UserId = userId1,
                ConversationId = newConversation.Id
            };

            var member2 = new ConversationMember
            {
                UserId = userId2,
                ConversationId = newConversation.Id
            };

            await unitOfWork.ConversationMemberRepo.AddAsync(member1);
            await unitOfWork.ConversationMemberRepo.AddAsync(member2);

            newConversation.Members.Add(member1);
            newConversation.Members.Add(member2);

            await unitOfWork.CommitAsync();

            var newConversationDto = mapper.Map<ConversationDTO>(newConversation);

            return new ServiceResult(true, data: newConversationDto);
        }
    

        public async Task<ServiceResult> SendMessageAsync(string senderId, SendMessageDTO model)
        {
            var convo = await unitOfWork.ConversationMemberRepo
                .GetSingleAsync(c => c.ConversationId == model.ConversationId && c.UserId == senderId);

            if (convo is null)
                return new ServiceResult(false, ["User is not part of this conversation"]);

            var msg = new Message
            {
                content = model.Content,
                SenderId = senderId,
                ConversationId = model.ConversationId,
                SentAt = DateTime.Now,
                Sender = await userManager.FindByIdAsync(senderId)
            };

            await unitOfWork.MessageRepo.AddAsync(msg);
            await unitOfWork.CommitAsync();

            var msgDTO = mapper.Map<MessageDTO>(msg);
  
            await hubContext.Clients.Group(model.ConversationId.ToString()).SendAsync("ReceiveMessage", msgDTO);

            return new ServiceResult(true, data: msgDTO);
        }
    }
}
