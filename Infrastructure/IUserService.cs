using Hospital_API.DTO;
using Hospital_API.Model;

namespace Hospital_API.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByID(string UserID);
        Task<ApplicationUser> GetUserByEmail(string Email);
        Task Update(ApplicationUser User);
        Task<Result> RemoveUser(ApplicationUser user);
        Task CheckActivationForUser(ApplicationUser doctor);
    }
}
