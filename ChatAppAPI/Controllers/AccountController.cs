using Application.DTOs.UserDTOs;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using ChatAppAPI.ViewModels.UserVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm]RegisterVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<RegisterDTO>(model);
            var res = await userService.RegisterUserAsync(modelDTO);

            if (res.success)
                return Ok(res.data);
            
            
            return BadRequest(res.Errors);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<LoginDTO>(model);
            var res = await userService.LoginUserAsync(modelDTO);

            if (!res.success)
                return Unauthorized(res.Errors);

            return Ok(res.data);
        }

        [HttpPost("forgot-password")]
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

        [HttpPost("reset-password")]
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

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<ChangePasswordDTO>(model);
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await userService.ChangePasswordAsync(userId, modelDTO);

            if (!res.success)
            {
                if (res.Errors.Contains("Incorrect password"))
                    return Unauthorized(res.Errors);

                return BadRequest(res.Errors);
            }

            return Ok(res.data);
        }

        [HttpPost("logout-single")]
        [Authorize]
        public async Task<IActionResult> LogOutSingle()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? token = User.FindFirstValue(JwtRegisteredClaimNames.Jti);

            var res = await userService.LogOutSingleAsync(userId, token);

            if (!res.success)
            {
                if (res.Errors.Contains("Token is invalid"))
                    return Unauthorized(res.Errors);

                return BadRequest(res.Errors);
            }

            return Ok(res.data);
        }

        [HttpPost("logout-all")]
        [Authorize]
        public async Task<IActionResult> LogOutAll()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await userService.LogOutAllAsync(userId);

            if (!res.success)
                return BadRequest(res.Errors);

            return Ok(res.data);
        }

        [HttpGet("get-info")]
        [Authorize]
        public async Task<IActionResult> GetInfo()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await userService.GetUserInfoAsync(userId);

            if (!res.success)
                return NotFound(res.Errors);

            return Ok(res.data);
        }

        [HttpPatch("update-info")]
        [Authorize]
        public async Task<IActionResult> UpdateInfo([FromForm] UpdateUserVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var modelDTO = mapper.Map<UpdateUserDTO>(model);

            var res = await userService.UpdateUserAsync(userId, modelDTO);

            if (!res.success)
            {
                if (res.Errors.Contains("User is not found"))
                    return NotFound(res.Errors);

                return BadRequest(res.Errors);
            }

            return Ok(res.data);
        }

        [HttpDelete("remove")]
        [Authorize]
        public async Task<IActionResult> RemoveAccount(LoginVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<LoginDTO>(model);
            var res = await userService.RemoveUserAsync(modelDTO);

            if (!res.success)
            {
                if (res.Errors.Contains("Incorrect data"))
                    return Unauthorized(res.Errors);

                return BadRequest(res.Errors);
            }
            
            return Ok(res.data);
        }
    }
}
