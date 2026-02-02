using HealthSync.DTOs;
using HealthSync.Model;
using Hospital_API.ApplicationDbContext;
using Hospital_API.DTO;
using Hospital_API.Model;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Validations;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace Hospital_API.Services
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AppDbContext dbContext;
        public PatientRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ResponseDTOForGettingAPIs<List<PatientsList>>> GetAllPatients()
        {
            var patients = await dbContext.patients.AsNoTracking().Select(options => new PatientsList
            {
                PatientId = options.Id.ToString(),
                FullName = options.User.FullName,
                PhoneNumber = options.User.PhoneNumber,
                ChronicDisease = options.ChronicDiseases,
                BloodType = options.bloodTypes.ToString(),
                IsActive = options.User.IsActive

            }).ToListAsync();

            return new ResponseDTOForGettingAPIs<List<PatientsList>>
            {
                Data = patients
            };
        }

        public async Task<List<PatientsList>> GetListOfPatientsForNurseRole(Guid UserId)
        {
            string Department = await dbContext.nurses.Where(op => op.Id == UserId).Select(op => op.Department).FirstOrDefaultAsync();
            if (Department == null)
                throw new Exception("Invalid Department"); //. will be handeled using the Middleware 

            var DepartmentForDoctor = await dbContext.appoinments.Where
                (op => op.Doctor.Department == Department && op.IsCancelled == false 
                 && op.Patient.User.IsActive == true && op.Patient.User != null).Select(op => new PatientsList
            {
                PatientId = op.PatientId.ToString(),
                FullName = op.Patient.User.FullName,
                PhoneNumber = op.Patient.User.PhoneNumber,
                BloodType = op.Patient.bloodTypes,
                ChronicDisease = op.Patient.ChronicDiseases,
                IsActive = op.Patient.User.IsActive
            }).ToListAsync();

            return DepartmentForDoctor;
        }


        public async Task<GetEmergencyContactDTO> GetEmergencyContact(Guid UserId)
        {
            var emergencyContact = await dbContext.emergencyContacts.AsNoTracking().Where(op => op.PatientId == UserId)
                .Select(op => new GetEmergencyContactDTO
            {
                FullName = op.Patient.User.FullName,
                PhoneNumber = op.PhoneNumber,
                AlternatePhone = op.AlternatePhone,
                Address = op.Address,
                Relation = op.Relation,
                IsPrimary = op.IsPrimary
            }).FirstOrDefaultAsync();

            return emergencyContact;
        }
        public async Task<List<PatientsList>> GetListForDoctorRole(Guid DoctorId)
        {
            //. not specific patient these are the list of patients
            var ListOfPatients = await dbContext.appoinments.AsNoTracking().Where(op => op.DoctorId == DoctorId).Select(op => new PatientsList
            {
                PatientId = op.PatientId.ToString(),
                PhoneNumber = op.Patient.User.PhoneNumber ,
                BloodType = op.Patient.bloodTypes,
                ChronicDisease = op.Patient.ChronicDiseases,
                FullName = op.Patient.User.FullName,
                IsActive = op.Patient.User.IsActive
            }).ToListAsync();

            return ListOfPatients;
        }

        public async Task<ResponseDTOForGettingAPIs<GettingPatientDTO>> GetPatientById(Guid patientId)
        {

            //. must use a pagination for better performance 
            var patient = await dbContext.patients.AsNoTracking().Select(op => new GettingPatientDTO
            {
                PatientId = op.Id.ToString(),
                FullName = op.User.FullName,
                Gender = op.User.Gender,
                PhoneNumber = op.User.PhoneNumber

            }).FirstOrDefaultAsync();
            if (patient == null)
                throw new Exception("patient are not exist"); //. will be handeled using the global middleware 

            return new ResponseDTOForGettingAPIs<GettingPatientDTO>
            {
                Data = patient
            };
        }

        public async Task<Patient> GetPatient(Guid patientId)
        {
            return await dbContext.patients.FirstOrDefaultAsync(op => op.Id == patientId);
        }

        public async Task<bool> CheckIfTheUserForTheSameDepartmentOfTheDoctor(Guid patientId, Guid DoctorId)
        {
            //. here will check if the patient has an appoinment with the Doctor or not
            var checking = await dbContext.appoinments.AnyAsync(op => op.PatientId == patientId && op.DoctorId
                                   == DoctorId);
            return checking;
        }

        //. i will send a mapping patient not from the DB
        public async Task Update(Patient patient)
        {
            dbContext.patients.Update(patient);
        }
    }
}
