using HealthSync.DTOs;
using Hospital_API.DTO;
using Hospital_API.Enums;
using Hospital_API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hospital_API.Services
{
    public interface IDoctorRepository
    {
        Task<List<DoctorDTO>> Doctors();

        Task<Doctor> GetDoctorById(string id);

        Task<Doctor> GetDoctorByEmail(string Email);

        //. will take an Application User argument
        Task Update(Doctor Doctor);
        //. will take an argument of ApplicationUser
        Task AdminUpdateDoctor(Doctor doctor);

        Task Delete(Doctor doctor);

        Task<Result> CheckCurrentRoleForDoctorUpdate(string UserID);

        Task SaveAsync();
    }
}
