using Hospital_API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital_API.Services
{
    public class SeedingSuperAdminRoleService : ISeedingSuperAdminRoleService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public SeedingSuperAdminRoleService(IConfiguration configuration,
             UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.roleManager = roleManager;
        } 
        public async Task SeedingSuperAdmin()
        {
            var user = await userManager.FindByEmailAsync(configuration["SuperAdmin:Email"]);
            if(user == null)
            {
                user = new ApplicationUser
                {
                    FullName = configuration["SuperAdmin:FullName"],
                    UserName = configuration["SuperAdmin:Email"],
                    Email = configuration["SuperAdmin:Email"],
                    Gender = configuration["SuperAdmin:Gender"],
                    NationalId = configuration["SuperAdmin:NationalId"]
                };

                var result = await userManager.CreateAsync(user, configuration["SuperAdmin:Password"]);
                if (! await roleManager.RoleExistsAsync("SuperAdmin"))
                {
                    var role = new IdentityRole
                    {
                        Name = "SuperAdmin"
                    };
                    await roleManager.CreateAsync(role);
                }
                await userManager.AddToRoleAsync(user, "SuperAdmin");
            }
            await userManager.AddToRoleAsync(user, "SuperAdmin");
        }
    }
}
