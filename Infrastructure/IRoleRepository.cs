using Hospital_API.DTO;
using Hospital_API.Model;
using Microsoft.AspNetCore.Identity;

namespace Hospital_API.Services
{
    public interface IRoleRepository
    {
        Task CreateRole(string roleName);
        Task Update(IdentityRole role);
        Task<IdentityRole> GetRoleByName(string name);
        Task<IdentityRole> GetRoleUsinID(string id);
        Task<List<UsersInRoleDTO>> GetUsersInRoleAsync(string RoleName);
        Task<List<GettingRolesDTO>> GetRoles();
        Task DeleteRole(IdentityRole role);
        Task<List<string>> RoleOfCurrentUser(string UserId);
    }
}
