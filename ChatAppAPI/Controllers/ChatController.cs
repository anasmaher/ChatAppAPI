using Application.DTOs.ConversationDTOs;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using ChatAppAPI.ViewModels.ChatVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using System.Security.Claims;

namespace ChatAppAPI.Controllers
{
    /// <summary>
    /// Controller responsible for handling chat-related operations.
    /// </summary>
    [Route("api/Chat")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IConversationService conversationService;
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatController"/> class.
        /// </summary>
        /// <param name="mapper">AutoMapper instance.</param>
        /// <param name="conversationService">Conversation service instance.</param>
        /// <param name="cache">For caching</param>
        public ChatController(IMapper mapper, IConversationService conversationService, IMemoryCache cache)
        {
            this.mapper = mapper;
            this.conversationService = conversationService;
            this.cache = cache;
        }

        /// <summary>
        /// Sends a message in a specified conversation.
        /// </summary>
        /// <param name="ConversationId">The unique identifier of the conversation to send the message to.</param>
        /// <param name="model">The message content and any additional data required.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// Returns <see cref="OkObjectResult"/> with the sent message if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
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

        /// <summary>
        /// Retrieves all messages for a specified conversation.
        /// </summary>
        /// <param name="conversationId">The unique identifier of the conversation to retrieve messages from.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a list of messages in the conversation.
        /// Returns <see cref="OkObjectResult"/> with the list of messages,
        /// or <see cref="NotFoundObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpGet("messages/{conversationId}")]
        public async Task<IActionResult> GetMessages(Guid conversationId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await conversationService.GetMessagesForConversationAsync(conversationId, userId);
            
            if (res.success)
                return Ok(res.data);

            return NotFound(res.Errors);
        }

        /// <summary>
        /// Retrieves an existing conversation with a specified recipient or creates a new one if it doesn't exist.
        /// </summary>
        /// <param name="recpId">The user identifier of the recipient.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the conversation details.
        /// Returns <see cref="OkObjectResult"/> with the conversation data if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpPost("get-or-create/{recpId}")]
        public async Task<IActionResult> GetOrCreateConversation(string recpId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await conversationService.GetOrCreateConversationAsync(userId, recpId);

            if (result.success)
                return Ok(result.data);
            

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Deletes a message from a conversation.
        /// </summary>
        /// <param name="model">An object containing the conversation ID and message ID to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// Returns <see cref="OkObjectResult"/> if the message was successfully deleted,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpDelete("delete-message")]
        public async Task<IActionResult> DeleteMessage(DeleteMessageDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await conversationService.DeleteMessageAsync(model.convoId, model.messageId, userId);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }

        /// <summary>
        /// Marks a specified message as read.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message to mark as read.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// Returns <see cref="OkObjectResult"/> if the message was successfully marked as read,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpPost("mark-as-read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await conversationService.MarkMessageAsReadAsync(messageId, userId);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }

        /// <summary>
        /// Edits a message in a conversation.
        /// </summary>
        /// <param name="messageId">The unique identifier of the message to be edited.</param>
        /// <param name="convoId">The unique identifier of the conversation that contains the message.</param>
        /// <param name="model">An object containing the updated message content.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// Returns <see cref="OkObjectResult"/> with the updated message data if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
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

        /// <summary>
        /// Retrieves all conversations for the authenticated user.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a list of the user's conversations.
        /// Returns <see cref="OkObjectResult"/> with the conversations list if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllConversations()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cacheKey = $"Conversations_{userId}";

            // Try to get the data from cache
            if (cache.TryGetValue(cacheKey, out var cachedConversations))
            {
                return Ok(cachedConversations);
            }

            // Data not in cache, so retrieve it from the service
            var res = await conversationService.GetAllConversationsAsync(userId);

            if (res.success)
            {
                // Set the data in cache with an expiration time
                cache.Set(cacheKey, res.data, TimeSpan.FromSeconds(10));
                return Ok(res.data);
            }

            return BadRequest(res.Errors);
        }
    }
}