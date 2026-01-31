using Hospital_API.Infrastructure.patientFileService;
using Hospital_API.Infrastructure.PresceptionService;
using Hospital_API.Infrastructure.PresceptionService.MedicineService;
using Hospital_API.Infrastructure.Services;
using Hospital_API.Infrastructure.VitalSignsServices;

namespace Hospital_API.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAppoinmentRepository appoinmentRepository { get; }
        INurseRepository nurseRepository { get; }
        IPatientRepository patientRepository { get; }
        IAdminRepository adminRepository { get; }
        IEmergencyContactsRepository emergencyContacts { get; }
        IPresceptionRepository presceptionRepository { get; }
        IMedicineService medicineService { get; }
        IMedicalRecordRepository medicalRecord { get; }
        IPatientFileRepository patientFileRepository { get; }
        IVitaSignsRepository vitaSignsRepository { get; }

        Task<int> SaveAsync();
    }
}
