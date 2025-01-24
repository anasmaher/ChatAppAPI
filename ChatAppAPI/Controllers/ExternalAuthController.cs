using Application.DTOs.UserDTOs;
using Application.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Domain.Entities;

namespace ChatAppAPI.Controllers
{
    /// <summary>
    /// Controller responsible for handling external authentication (e.g., Google Sign-In).
    /// </summary>
    [Route("api/ExternalAuth")]
    [ApiController]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService tokenService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalAuthController"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration settings.</param>
        /// <param name="tokenService">Service for generating tokens.</param>
        /// <param name="userService">Service for user operations.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        public ExternalAuthController(
            IConfiguration configuration,
            ITokenService tokenService,
            IUserService userService,
            IMapper mapper)
        {
            _configuration = configuration;
            this.tokenService = tokenService;
            this.userService = userService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Initiates the Google Sign-In process.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that challenges the user with the Google authentication scheme.
        /// </returns>
        [HttpGet("signin-google")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleResponse))
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Handles the response from Google after authentication.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the JWT tokens if authentication is successful,
        /// or an appropriate error response.
        /// </returns>
        [HttpGet("GoogleResponse")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return Unauthorized();

            // Extract user information
            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var userEmail = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var userId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Call to create or retrieve the external user
            var userResult = await userService.GetOrCreateExternalUserAsync(userEmail, userName, userId);
            var user = userResult.data as AppUser;

            if (user is null)
                return BadRequest("Could not create or retrieve user.");

            // Generate JWT token
            var tokens = await tokenService.GenerateTokenAsync(user);
            // Return the token to the client
            return Ok(tokens);
        }
    }
}