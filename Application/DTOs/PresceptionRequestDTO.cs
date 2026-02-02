using HealthSync.Model;
using Hospital_API.Model;

namespace Hospital_API.Application.PrescptionsDTO
{
    public class PresceptionRequest
    {
        public Guid? AppoinmentId { get; set; }
        public string Notes { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public List<PresceptionMedicineDTO> medicines { get; set; }
    }
}
