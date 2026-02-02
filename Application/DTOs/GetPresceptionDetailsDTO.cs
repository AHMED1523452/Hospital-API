using HealthSync.Model;

namespace Hospital_API.Application.PrescptionsDTO
{
    public class GetPresceptionDetailsDTO
    {
        public Guid PresceptionID { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<PresceptionMedicineDTO>? medicines { get; set; }
    }
}
