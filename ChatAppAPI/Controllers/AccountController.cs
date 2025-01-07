using Application.DTOs.ResultsDTOs;
using Application.DTOs.UserDTOs;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using ChatAppAPI.ViewModels.UserVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Claims;

namespace ChatAppAPI.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public AccountController(IUserService userService, IMapper mapper)
        {
            this.userService = userService;
            this.mapper = mapper;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm]RegisterVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<RegisterDTO>(model);
            var res = await userService.RegisterUserAsync(modelDTO);

            if (res.success)
            {
                var userVM = mapper.Map<UserVM>(res.data);

                return Ok(userVM);
            }
            
            return BadRequest(res.Errors);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<LoginDTO>(model);
            var res = await userService.LoginUserAsync(modelDTO);

            if (res.success)
                return Ok(res.data);
            
            return BadRequest(res.Errors);
        }

        [HttpPost("Remove")]
        [Authorize]
        public async Task<IActionResult> RemoveAccount(LoginVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<LoginDTO>(model);
            var res = await userService.RemoveUserAsync(modelDTO);

            if (!res.success)
                return BadRequest(res.Errors);
            
            return Ok(res.data);
        }

        [HttpPatch("UpdateInfo")]
        [Authorize]
        public async Task<IActionResult> UpdateInfo([FromForm]UpdateUserVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var modelDTO = mapper.Map<UpdateUserDTO>(model);

            var res = await userService.UpdateUserAsync(userId, modelDTO);

            if (!res.success)
                return BadRequest(res.Errors);
            
            return Ok(res.data);
        }

        [HttpGet("GetInfo")]
        [Authorize]
        public async Task<IActionResult> GetInfo()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await userService.GetUserInfoAsync(userId);

            if (!res.success)
                return BadRequest(res.Errors);

            return Ok(res.data);
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<ForgotPasswordDTO>(model);

            var res = await userService.ForgotPasswordAsync(modelDTO);

            if (!res.success)
                return BadRequest(res.Errors);
            
            return Ok(res.data);
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<ResetPasswordDTO>(model);

            var res = await userService.ResetPasswordAsync(modelDTO);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<ChangePasswordDTO>(model);
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await userService.ChangePasswordAsync(userId, modelDTO);

            if (!res.success)
                return BadRequest(res.Errors);

            return Ok(res.data);
        }
    }
}
