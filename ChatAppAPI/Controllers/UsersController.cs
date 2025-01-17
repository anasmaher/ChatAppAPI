using Application.DTOs.AdminDTOs;
using Application.Interfaces.ServicesInterfaces;
using Application.Services;
using AutoMapper;
using ChatAppAPI.ViewModels.ForAdminVMs;
using ChatAppAPI.ViewModels.UserVMs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppAPI.Controllers
{
    [Route("api/Admin")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAdminService adminService;

        public UsersController( IMapper mapper, IAdminService adminService)
        {
            this.mapper = mapper;
            this.adminService = adminService;
        }

        [HttpPost("change-role/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ChangeRole(string id, ChangeRoleVM model)
        {
            var modelDTO = mapper.Map<ChangeRoleDTO>(model);
            var res = await adminService.AssignRoleAsync(id, modelDTO);

            if (!res.success)
            {
                if (res.Errors.Contains("User not found"))
                    return NotFound(res.Errors);

                return BadRequest(res.Errors);
            }

            return Ok(res.data);
        }

        [HttpGet("get-all-users")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUsers(int pageNubmer = 1, int pageSize = 10)
        {
            var res = await adminService.GetAllUsersAsync(pageNubmer, pageSize);

            if (!res.success)
                return BadRequest(res.Errors);

            return Ok(res.data);
        }

        [HttpDelete("delete-user/{id}")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var res = await adminService.RemoveUserAdminAsync(id);

            if (!res.success)
            {
                if (res.Errors.Contains("User not found"))
                    return NotFound(res.Errors);

                return BadRequest(res.Errors);
            }

            return Ok(res.data);
        }
    }
}
