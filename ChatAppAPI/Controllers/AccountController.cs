using Application.DTOs.ResultsDTOs;
using Application.DTOs.UserDTOs;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using ChatAppAPI.ViewModels.UserVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm]RegisterVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userDTO = mapper.Map<RegisterDTO>(model);
            var res = await userService.RegisterUserAsync(userDTO);

            if (res.success)
            {
                var userVM = mapper.Map<UserVM>(res.data);
                return Ok(userVM);
            }
            else
                return BadRequest(res.Error);
        }
    }
}
