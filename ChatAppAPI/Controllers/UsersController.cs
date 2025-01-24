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
    /// <summary>
    /// Controller responsible for administrative operations such as managing user roles and accounts.
    /// </summary>
    [Route("api/Admin")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAdminService adminService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        /// <param name="adminService">The service for administrative operations.</param>
        public UsersController(IMapper mapper, IAdminService adminService)
        {
            this.mapper = mapper;
            this.adminService = adminService;
        }

        /// <summary>
        /// Changes the role of a specified user.
        /// </summary>
        /// <param name="id">The identifier of the user whose role is to be changed.</param>
        /// <param name="model">An object containing the new role information.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// Returns <see cref="OkObjectResult"/> with the updated user data if successful,
        /// <see cref="NotFoundResult"/> if the user is not found,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
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

        /// <summary>
        /// Retrieves all users with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination. Default is 1.</param>
        /// <param name="pageSize">The number of users per page. Default is 10.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a paginated list of users.
        /// Returns <see cref="OkObjectResult"/> with the list of users if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            var res = await adminService.GetAllUsersAsync(pageNumber, pageSize);

            if (!res.success)
                return BadRequest(res.Errors);

            return Ok(res.data);
        }

        /// <summary>
        /// Deletes a specified user.
        /// </summary>
        /// <param name="id">The identifier of the user to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the delete operation.
        /// Returns <see cref="OkObjectResult"/> if the user was successfully deleted,
        /// <see cref="NotFoundResult"/> if the user is not found,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
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