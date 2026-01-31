using HealthSync.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_API.Domain.Model
{
    public class Patient
    {
        public Guid Id { get; set; }
        // FK
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool CheckedIn { get; set; } = false;
        [Required(ErrorMessage = "blood types is required")]
        public string bloodTypes { get; set; }
        public string? ChronicDiseases { get; set; }
        public List<Appoinment> Appointments { get; set; }
        public List<EmergencyContact> EmergencyContacts { get; set; }
        public List<Presception> Prescriptions { get; set; }
        public List<PatientFile> patientFiles { get; set; }
    }
}
