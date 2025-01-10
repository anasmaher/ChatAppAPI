using Application.DTOs.ForAdminDTOs;
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
    [Authorize(Roles = "Owner,Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAdminService adminService;

        public AdminController( IMapper mapper, IAdminService adminService)
        {
            this.mapper = mapper;
            this.adminService = adminService;
        }


        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var res = await adminService.GetAllUsersAsync();

            if (!res.success)
                return BadRequest(res.Errors);

            return Ok(res.data);
        }

        [HttpPost("ChangeRole/{id}")]
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

        [HttpDelete("DeleteUser/{id}")]
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
