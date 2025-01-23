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
        private readonly IMapper mapper;
        private readonly IConversationService conversationService;

        public ChatController(IMapper mapper, IConversationService conversationService)
        {
            this.mapper = mapper;
            this.conversationService = conversationService;
        }

        [HttpPost("send/{ConversationId}")]
        public async Task<IActionResult> SendMessage(Guid ConversationId, SendMesaageVM model)
        {
            string senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var modelDTO = mapper.Map<SendMessageDTO>(model);
            modelDTO.ConversationId = ConversationId;

            var result = await conversationService.SendMessageAsync(senderId, modelDTO);

            if (result.success)
                return Ok(result.data);
            

            return BadRequest(result.Errors);
        }

        [HttpGet("messages/{conversationId}")]
        public async Task<IActionResult> GetMessages(Guid conversationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var messages = await conversationService.GetMessagesForConversationAsync(conversationId, userId);
            return Ok(messages.data);
        }

        [HttpPost("get-or-create/{recpId}")]
        public async Task<IActionResult> GetOrCreateConversation(string recpId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await conversationService.GetOrCreateConversationAsync(userId, recpId);

            if (result.success)
                return Ok(result.data);
            
            return BadRequest(result.Errors);
        }

        [HttpDelete("delete-message")]
        public async Task<IActionResult> DeleteMessage(DeleteMessageDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await conversationService.DeleteMessageAsync(model.convoId, model.messageId, userId);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }

        [HttpPost("mark-as-read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await conversationService.MarkMessageAsReadAsync(messageId, userId);
            
            if (res.success)
                return Ok(res.data);    

            return BadRequest(res.Errors);
        }

        [HttpPut("edit-message/{messageId}/{convoId}")]
        public async Task<IActionResult> EditMessage(int messageId, Guid convoId, EditMessageVM model)
        {
            var modelDTO = mapper.Map<EditMessageDTO>(model);
            modelDTO.messageId = messageId;
            modelDTO.convoId = convoId;

            var res = await conversationService.EditMessageAsync(modelDTO);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllConversations()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await conversationService.GetAllConversationsAsync(userId);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }
    }
}
