using Application.DTOs.GroupDTOs;
using Application.Interfaces.ServicesInterfaces;
using Application.Services;
using AutoMapper;
using ChatAppAPI.ViewModels.GroupVMs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IGroupService groupService;

        public GroupController(IMapper mapper, IGroupService groupService)
        {
            this.mapper = mapper;
            this.groupService = groupService;
        }

        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup(CreateGroupVM createGroupVm)
        {
            var createGroupDto = mapper.Map<CreateGroupDTO>(createGroupVm);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await groupService.CreateGroupAsync(createGroupDto, userId);

            if (!result.success)
                return BadRequest(result.Errors);

            return Ok(result.data);
        }

        [HttpPost("add-member/{groupId}/{userId}")]
        public async Task<IActionResult> AddMember(Guid groupId, string userId)
        {
            var result = await groupService.AddMemberAsync(groupId, userId);

            if (!result.success)
                return BadRequest(result.Errors);

            return Ok(result.data);
        }

        [HttpDelete("remove-member/{groupId}/{userId}")]
        public async Task<IActionResult> RemoveMember(Guid groupId, string userId)
        {
            var result = await groupService.RemoveMemberAsync(groupId, userId);

            if (!result.success)
                return BadRequest(result.Errors);

            return Ok(result.data);
        }

        [HttpGet("get-group{groupId}")]
        public async Task<IActionResult> GetGroup(Guid groupId)
        {
            var result = await groupService.GetGroupAsync(groupId);

            if (!result.success)
                return NotFound(result.Errors);

            return Ok(result.data);
        }
    }
}
