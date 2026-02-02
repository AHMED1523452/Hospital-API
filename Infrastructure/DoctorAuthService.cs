using Hospital_API.ApplicationDbContext;
using Hospital_API.DTO;
using Hospital_API.Model;
using Microsoft.AspNetCore.Identity;

namespace Hospital_API.Services
{
    public class DoctorAuthService : IDoctorAuthService
    {
        private readonly AppDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly ILogger<DoctorAuthService> logger;

        public DoctorAuthService(AppDbContext dbContext, 
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor, 
            ILogger<DoctorAuthService> logger)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.contextAccessor = contextAccessor;
            this.logger = logger;
        }

        //. this function for a doctor role only
        public async Task ChangePassword(ChangeDoctorPasswordDTO doctorDTO, string currentPassword, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(doctorDTO.Email);
            if (user == null)
                throw new Exception("User is not exist");
            await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            logger.LogInformation(
                "User {UserName} changed his password at {ChangedAt}",
                doctorDTO.Email,
                DateTime.Now);

        }
    }
}
