using Microsoft.Identity.Client;

namespace Hospital_API.Application.MedialRecordDTO
{
    public class MedicalRecordRequestDTO
    {
        public Guid patientFileId { get; set; }
        public Guid DoctorId { get; set; }
        public string diagnosis { get; set; }
        public string Treatment { get; set; }
    }
}
