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
    [Route("api/ExternalAuth")]
    [ApiController]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService tokenService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

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
            
            // call log in google
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
