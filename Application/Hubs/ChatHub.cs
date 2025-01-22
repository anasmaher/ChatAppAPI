using Application.DTOs.ConversationDTOs;
using Application.Interfaces.ReposInterfaces;
using Application.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace ChatAppAPI.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConversationService messageService;
        private readonly IUnitOfWork unitOfWork;

        public ChatHub(IConversationService messageService, IUnitOfWork unitOfWork)
        {
            messageService = messageService;
            this.unitOfWork = unitOfWork;
        }

        //public async Task SendMessage(SendMessageDTO model)
        //{
        //    var senderId = Context.UserIdentifier;

        //    var result = await messageService.SendMessageAsync(senderId, model);

        //    if (result.success)
        //    {
        //        var messageDto = result.data as MessageDTO;

        //        // Broadcast the message to all clients in the conversation group
        //        await Clients.Group(model.ConversationId.ToString()).SendAsync("ReceiveMessage", messageDto);
        //    }
        //    else
        //    {
        //        await Clients.Caller.SendAsync("Error", result.Errors);
        //    }
        //}

        public async Task JoinGroup(Guid conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());

            //await Clients.Group(conversationId.ToString()).SendAsync("UserJoined", Context.UserIdentifier);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            // Join all conversation groups that the user is a part of
            var conversations = await unitOfWork.ConversationMemberRepo.GetConversationsForUserAsync(userId);

            foreach (var conversation in conversations)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;

            // Remove the user from all groups they were part of
            var conversations = await unitOfWork.ConversationMemberRepo.GetConversationsForUserAsync(userId);

            foreach (var conversation in conversations)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversation.Id.ToString());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
