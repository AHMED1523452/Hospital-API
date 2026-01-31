using HealthSync.patientFileService;
using HealthSync.PresceptionService;
using HealthSync.PresceptionService.MedicineService;
using HealthSync.VitalSignsServices;
using Hospital_API.Infrastructure.ApplicationDbContext;
using Hospital_API.Infrastructure.patientFileService;
using Hospital_API.Infrastructure.PresceptionService;
using Hospital_API.Infrastructure.PresceptionService.MedicineService;
using Hospital_API.Infrastructure.Services;
using Hospital_API.Infrastructure.VitalSignsServices;
using Hospital_API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Hospital_API.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public AppDbContext dbContext;
        public IPatientRepository patientRepository { get; }
        public IAppoinmentRepository appoinmentRepository { get; }
        public INurseRepository nurseRepository { get; }
        public IAdminRepository adminRepository { get; }
        public IEmergencyContactsRepository emergencyContacts { get; }
        public IPresceptionRepository presceptionRepository { get; }
        public IMedicineService medicineService { get; }
        public IMedicalRecordRepository medicalRecord { get; }
        public IPatientFileRepository patientFileRepository { get; }
        public IVitaSignsRepository vitaSignsRepository { get; }
        public UnitOfWork(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            appoinmentRepository = new AppoinmentRepository(dbContext);
            nurseRepository = new NurseRepository(dbContext);
            patientRepository = new PatientRepository(dbContext);
            emergencyContacts = new EmergencyContactsRepository(dbContext);
            presceptionRepository = new prescriptionRepository(dbContext);
            medicineService = new MedicineService(dbContext);
            medicalRecord = new MedicalRecordRepository(dbContext);
            patientFileRepository = new patientFileRepository(dbContext);
            vitaSignsRepository = new vitalSingsRepository(dbContext);
        }


        public async Task<int> SaveAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
