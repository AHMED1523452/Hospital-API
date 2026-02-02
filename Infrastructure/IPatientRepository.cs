using HealthSync.DTOs;
using Hospital_API.DTO;
using Hospital_API.Model;

namespace Hospital_API.Services
{
    public interface IPatientRepository
    {
        Task<ResponseDTOForGettingAPIs<List<PatientsList>>> GetAllPatients();
        Task<List<PatientsList>> GetListForDoctorRole(Guid DoctorId);
        Task<GetEmergencyContactDTO> GetEmergencyContact(Guid UserId);
        Task<ResponseDTOForGettingAPIs<GettingPatientDTO>> GetPatientById(Guid patientId);
        Task<List<PatientsList>> GetListOfPatientsForNurseRole(Guid UserId);
        Task<Patient> GetPatient(Guid patientId);
        Task<bool> CheckIfTheUserForTheSameDepartmentOfTheDoctor(Guid patientId, Guid DoctorId);
        Task Update(Patient patient);
    }
}
