using Application.DTOs.ConversationDTOs;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using ChatAppAPI.ViewModels.ChatVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Security.Claims;

namespace ChatAppAPI.Controllers
{
    [Route("api/Chat")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IConversationService messageService;
        private readonly IMapper mapper;
        private readonly IConversationService conversationService;

        public ChatController(IConversationService messageService, IMapper mapper, IConversationService conversationService)
        {
            this.messageService = messageService;
            this.mapper = mapper;
            this.conversationService = conversationService;
        }

        [HttpPost("send/{ConversationId}")]
        public async Task<IActionResult> SendMessage(Guid ConversationId, SendMesaageVM model)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var modelDTO = mapper.Map<SendMessageDTO>(model);
            modelDTO.ConversationId = ConversationId;

            var result = await messageService.SendMessageAsync(senderId, modelDTO);

            if (result.success)
                return Ok(result.data);
            

            return BadRequest(result.Errors);
        }

        [HttpGet("messages/{conversationId}")]
        public async Task<IActionResult> GetMessages(Guid conversationId)
        {
            var messages = await messageService.GetMessagesForConversationAsync(conversationId);
            return Ok(messages.data);
        }

        [HttpPost("get-or-create/{recpId}")]
        public async Task<IActionResult> GetOrCreateConversation(string recpId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await conversationService.GetOrCreateConversationAsync(userId, recpId);

            if (result.success)
                return Ok(result.data);
            
            return BadRequest(result.Errors);
        }
    }
}
