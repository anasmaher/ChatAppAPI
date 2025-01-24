using Application.DTOs.UserDTOs;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using ChatAppAPI.ViewModels.UserVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatAppAPI.Controllers
{
    /// <summary>
    /// Controller responsible for handling user account-related operations.
    /// </summary>
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userService">Instance of <see cref="IUserService"/> to handle user operations.</param>
        /// <param name="mapper">Instance of AutoMapper <see cref="IMapper"/>.</param>
        public AccountController(IUserService userService, IMapper mapper)
        {
            this.userService = userService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Registers a new user with the provided information.
        /// </summary>
        /// <param name="model">The registration details provided by the user.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the registration operation.
        /// Returns <see cref="OkObjectResult"/> if registration is successful,
        /// or <see cref="BadRequestObjectResult"/> with validation errors.
        /// </returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] RegisterVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modelDTO = mapper.Map<RegisterDTO>(model);
            var res = await userService.RegisterUserAsync(modelDTO);

            if (res.success)
                return Ok(res.data);


            return BadRequest(res.Errors);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="model">The login credentials provided by the user.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the login operation.
        /// Returns <see cref="OkObjectResult"/> with authentication data if successful,
        /// or <see cref="UnauthorizedResult"/> if authentication fails.
        /// </returns>
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

        /// <summary>
        /// Initiates the password reset process by sending a reset link to the user's email.
        /// </summary>
        /// <param name="model">The email address of the user requesting password reset.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// Returns <see cref="OkObjectResult"/> if the email was sent successfully,
        /// or <see cref="BadRequestObjectResult"/> with validation errors.
        /// </returns>
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

        /// <summary>
        /// Resets the user's password using the provided reset token and new password.
        /// </summary>
        /// <param name="model">The reset password details including token, email, and new password.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the password reset operation.
        /// Returns <see cref="OkObjectResult"/> if password reset is successful,
        /// or <see cref="BadRequestObjectResult"/> with validation errors.
        /// </returns>
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

        /// <summary>
        /// Changes the authenticated user's password.
        /// </summary>
        /// <param name="model">An object containing the current and new password.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the password change operation.
        /// Returns <see cref="OkObjectResult"/> if password change is successful,
        /// <see cref="UnauthorizedResult"/> if credentials are invalid,
        /// or <see cref="BadRequestObjectResult"/> with validation errors.
        /// </returns>
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
                if (res.Errors.Contains("Invalid credentials"))
                    return Unauthorized(res.Errors);

                return BadRequest(res.Errors);
            }

            return Ok(res.data);
        }

        /// <summary>
        /// Logs out the authenticated user from the current session.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the logout operation.
        /// Returns <see cref="OkObjectResult"/> if logout is successful,
        /// <see cref="UnauthorizedResult"/> if the token is invalid,
        /// or <see cref="BadRequestObjectResult"/> with errors.
        /// </returns>
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

        /// <summary>
        /// Logs out the authenticated user from all sessions.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the logout operation.
        /// Returns <see cref="OkObjectResult"/> if logout is successful,
        /// or <see cref="BadRequestObjectResult"/> with errors.
        /// </returns>
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

        /// <summary>
        /// Retrieves information about the authenticated user.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the user's information.
        /// Returns <see cref="OkObjectResult"/> with user data if successful,
        /// or <see cref="NotFoundResult"/> if the user is not found.
        /// </returns>
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

        /// <summary>
        /// Updates the authenticated user's information.
        /// </summary>
        /// <param name="model">An object containing the updated user information.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the update operation.
        /// Returns <see cref="OkObjectResult"/> with updated user data if successful,
        /// <see cref="NotFoundResult"/> if the user is not found,
        /// or <see cref="BadRequestObjectResult"/> with validation errors.
        /// </returns>
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
                if (res.Errors.Contains("Invalid credentials"))
                    return NotFound(res.Errors);

                return BadRequest(res.Errors);
            }

            return Ok(res.data);
        }

        /// <summary>
        /// Permanently removes the authenticated user's account.
        /// </summary>
        /// <param name="model">The user's login credentials for verification.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the removal operation.
        /// Returns <see cref="OkObjectResult"/> if the account was removed successfully,
        /// <see cref="UnauthorizedResult"/> if credentials are invalid,
        /// or <see cref="BadRequestObjectResult"/> with errors.
        /// </returns>
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
                if (res.Errors.Contains("Invalid credentials"))
                    return Unauthorized(res.Errors);

                return BadRequest(res.Errors);
            }

            return Ok(res.data);
        }
    }
}