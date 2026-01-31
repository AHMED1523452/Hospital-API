using Hospital_API.ApplicationDbContext;
using Hospital_API.DTO;
using Hospital_API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Claims;

namespace Hospital_API.Services
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RoleRepository(RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateRole(string roleName)
        {
            var role = new IdentityRole
            {
                Name =roleName
            };

            await roleManager.CreateAsync(role);
        }

        public async Task Update(IdentityRole role)
        {
            var result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                foreach(var items in result.Errors)
                {
                    throw new Exception(items.ToString());
                }
            }
        }

        public async Task<IdentityRole> GetRoleUsinID(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            return role;
        }

        public async Task<List<UsersInRoleDTO>> GetUsersInRoleAsync(string RoleName)
        {
            var users = (await userManager.GetUsersInRoleAsync(RoleName)).ToList();
            var usersInRole = new List<UsersInRoleDTO>();
            foreach(var items in users)
            {
                usersInRole.Add(new UsersInRoleDTO
                {
                    UserName = items.FullName,
                    Email = items.Email,
                    NationalID = items.NationalId
                });
            }
            return usersInRole;
        }

        public async Task<List<string>> RoleOfCurrentUser(string UserId)
        {
            var user = await userManager.FindByIdAsync(UserId);
            var roleOfCurrentUser = (await userManager.GetRolesAsync(user)).ToList();
            return roleOfCurrentUser;
        }

        public async Task<List<GettingRolesDTO>> GetRoles()
        {
            var roles = await roleManager.Roles.ToListAsync();
            var GettingRoles = new List<GettingRolesDTO>();
            foreach(var items in roles)
            {
                GettingRoles.Add(new GettingRolesDTO
                {
                    RoleName = items.Name
                });
            }
            return GettingRoles;
        }

        public async Task<IdentityRole> GetRoleByName(string name)
        {
            var role = await roleManager.FindByNameAsync(name);
            return role;
        }

        public async Task DeleteRole(IdentityRole role)
        {
            await roleManager.DeleteAsync(role);
        }
    }
}
