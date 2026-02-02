namespace Hospital_API.Application.MedialRecordDTO
{
    public class MedicalRecordResponseDTO
    {
        public string Id { get; set; }
        public string patientFileId { get; set; }
        public string DoctorId { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public bool IsClosed { get; set; }
        public bool IsArchieved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
