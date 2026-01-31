using Hospital_API.Enums;

namespace Hospital_API.Domain.Model
{
    public class Doctor
    {
        public Guid Id { get; set; }

        // FK → AspNetUsers
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Specialty { get; set; }
        public string LicenseNumber { get; set; }
        public string? Department { get; set; }

        public List<Appoinment> Appointments { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
        public List<Presception> Prescriptions { get; set; }
    }
}
