using HealthSync.DTOs;
using Hospital_API.ApplicationDbContext;
using Hospital_API.DTO;
using Hospital_API.Enums;
using Hospital_API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Xml.Linq;

namespace Hospital_API.Services
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ITokenIPService tokenIP;

        public DoctorRepository(AppDbContext dbContext, 
            UserManager<ApplicationUser> userManager, 
            IHttpContextAccessor httpContextAccessor, ITokenIPService tokenIP)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.tokenIP = tokenIP;
        }


        //. bringing list of doctors without any add or updates process 
        public async Task<List<DoctorDTO>> Doctors()
        {
            //. the time complexity is very large 
            //. here will use the global filter for the doctor list 
            var users = await dbContext.doctors.AsNoTracking().Select(op => new DoctorDTO
            {
                DoctorID = op.Id.ToString(),
                FullName = op.User.FullName,
                Department = op.Department,
                Email = op.User.Email,
                Phone = op.User.PhoneNumber,
                NationalID = op.User.NationalId
            }).ToListAsync();

            return users;
        }

        //. bringing specific doctor with track for update process
        public async Task<Doctor> GetDoctorById(string id)
        {
            //. will bring the Doctor that has this ID 
            //. if there is a case like that the doctor id for a doctor that not active on the server 
            var doctor = await dbContext.doctors.IgnoreQueryFilters().FirstOrDefaultAsync(op => op.Id == Guid.Parse(id)); 
            return doctor;
        }

        public async Task<Doctor> GetDoctorByEmail(string Email)
        {
            var doctor = await dbContext.doctors.Include(op => op.User).
                Where(op => op.User.Email == Email).FirstOrDefaultAsync();

            return doctor;
        }

        //. will take an Application User argument
        public async Task Update(Doctor Doctor)
        {
           var result = dbContext.doctors.Update(Doctor);
        }

        //. will take an argument of ApplicationUser  
        public async Task AdminUpdateDoctor(Doctor doctor) //. the user will enter the doctor ID not the UserID
        {
            dbContext.doctors.Update(doctor);
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(Doctor doctor)
        {
            dbContext.doctors.Remove(doctor);
        }

        //. this will be removed when the [Authorize(Roles = "Doctor")] is running well
        public async Task<Result> CheckCurrentRoleForDoctorUpdate(string UserID)
        {
            //. if the user is not existing in the table of doctor or Users table will throw an exception 
            //. if the usre is existing in the users that's meaning that the doctor is exiting in the doctors table 

            var user = await userManager.FindByIdAsync(UserID);
            var doctor = await dbContext.doctors.FirstOrDefaultAsync(op => op.UserId == UserID);
            if (user == null || doctor == null)
                return new Result().Failure("This user is not exist");

            if (user.IsActive != true)
                return new Result().Failure("this is user inactive");

            var roles = await userManager.GetRolesAsync(user);
            bool IsInRole = true;
            foreach(var items in roles)
            {
                if(items != "doctor")
                {
                    IsInRole = false;
                    return new Result().Failure("this is not forbidden");
                }
            }
            return new Result().Success;
        }

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
