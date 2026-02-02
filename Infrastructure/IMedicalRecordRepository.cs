using HealthSync.MedialRecordDTO;
using Hospital_API.Model;

namespace Hospital_API.Services
{
    public interface IMedicalRecordRepository
    {
        Task Create(MedicalRecord record);
        Task<MedicalRecord> GetMedicalRecordById(Guid medicalRecordID);
        Task<MedicalRecordResponseDTO> GetMedicalRecord(Guid Id);
        Task<List<MedicalRecordByPatientFileDTO>> GetMedicalRecordUsingPatientFileId(Guid patientFileId);
        Task Update(MedicalRecord record);
    }
}
