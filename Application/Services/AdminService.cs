using Application.DTOs.AdminDTOs;
using Application.DTOs.ResultsDTOs;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMapper mapper;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminService(
            UserManager<AppUser> userManager,
            IMapper mapper,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<ServiceResult> AssignRoleAsync(string id, ChangeRoleDTO model)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user is null)
                return new ServiceResult(false, ["User not found"]);

            var isRoleExist = roleManager.Roles.FirstOrDefault(x => x.Name == model.RoleName);

            if (isRoleExist is null)
            {
                var roleCreated = await roleManager.CreateAsync(new IdentityRole(model.RoleName));

                if (!roleCreated.Succeeded)
                    return new ServiceResult(false, ["Role does not exist and could not be created"]);
            }

            var roleForUser = (await userManager.GetRolesAsync(user))[0];

            if (roleForUser == model.RoleName)
                return new ServiceResult(false, ["User already has this role"]);

            await userManager.RemoveFromRoleAsync(user, roleForUser);
            var res = await userManager.AddToRoleAsync(user, model.RoleName);

            if (res.Succeeded)
            {
                await signInManager.RefreshSignInAsync(user);

                return new ServiceResult(true, data: "Role was assigned");
            }

            return new ServiceResult(false, res.Errors.Select(e => e.Description).ToList());
        }

        public async Task<ServiceResult> GetAllUsersAsync()
        {
            var users = userManager.Users.ToList();
            
            var usersForAdminDTO = mapper.Map<IEnumerable<UserForAdminDTO>>(users);
            foreach (var item in usersForAdminDTO)
            {
                var user = await userManager.FindByIdAsync(item.Id);
                var roles = await userManager.GetRolesAsync(user);
                item.Role = roles[0];
            }
            return new ServiceResult(true, data: usersForAdminDTO);
        }

        public async Task<ServiceResult> RemoveUserAdminAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user is null)
                return new ServiceResult(false, ["User not found"]);

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
                return new ServiceResult(true, data: "User deleted");

            return new ServiceResult(false, result.Errors.Select(e => e.Description).ToList());
        }
    }
}
