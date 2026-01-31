using Azure;
using Hospital_API.ApplicationDbContext;
using Hospital_API.DTO;
using Hospital_API.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Win32.SafeHandles;

namespace Hospital_API.Services
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext dbContext;

        public AdminRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ResponseDTOForGettingAPIs<NurseDTO>> GetNurse(Guid nurseId)
        {
            var nurse = await dbContext.nurses.Where(op => op.Id == nurseId).Select(op => new NurseDTO
            {
                NurseId = op.Id,
                FullName = op.User.FullName, //. will bring the name from the db for the nurse or user Name
                Email = op.User.Email,
                Department = op.Department,
                PhoneNumber = op.User.PhoneNumber
            }).FirstOrDefaultAsync();

            if (nurse == null) throw new Exception("Nurse is not exist");

            return new ResponseDTOForGettingAPIs<NurseDTO>
            {
                Data = nurse
            };
        }
    }
}
