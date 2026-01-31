using HealthSync.DTOs;
using HealthSync.Model;

namespace HealthSync.Services
{
    public interface IEmergencyContactsRepository
    {
        Task Create(EmergencyContact emergencyContact);
        Task<List<GetEmergencyContactDTO>> GetListOfEmergencyContactsWithPatientId(Guid patientId);
        Task<EmergencyContact> GetEmergencyContact(Guid emergencyContactId);
        Task Update(EmergencyContact emergencyContact);
        Task Delete(EmergencyContact emergencyContact);
    }
}
