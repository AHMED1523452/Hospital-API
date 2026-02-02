using Hospital_API.DTO;
using Hospital_API.Model;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Diagnostics;

namespace Hospital_API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserService(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<ApplicationUser> GetUserByID(string UserID)
        {
            var user = await userManager.FindByIdAsync(UserID);
            return user;
        }

        public async Task<ApplicationUser> GetUserByEmail(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
                throw new Exception("This user is not exist");
            return user;
        }

        public async Task Update(ApplicationUser user)
        {
            await userManager.SetEmailAsync(user, user.Email);
            await userManager.SetPhoneNumberAsync(user, user.PhoneNumber);
            await userManager.SetUserNameAsync(user, user.Email);

            user.UpdateAt = DateTime.UtcNow ;

           var result= await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var items in result.Errors)
                    throw new Exception(items.ToString());
            }

        }

        public async Task<Result> RemoveUser(ApplicationUser user)
        {
            if (user == null)
                return new Result().Failure("User not found");

            user.IsActive = false;
            user.UpdateAt = DateTime.UtcNow;

            await userManager.UpdateAsync(user);
            return new Result().Success;
        }

        public async Task CheckActivationForUser(ApplicationUser doctor)
        {
            if (doctor.IsActive == false)
                throw new Exception("Account is inactivated , please roll back  to the adminstrator");
        }
    }
}
