using Microsoft.OpenApi.MicrosoftExtensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_API.Domain.Model
{
    public class MedicalRecord
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PatientFileId { get; set; }
        public PatientFile PatientFile { get; set; }
        public Guid DoctorId { get; set; }
        public Doctor  Doctor { get; set; }

        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public bool IsArchieved { get; set; } = false;
        public bool IsClosed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
