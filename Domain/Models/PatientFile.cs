using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_API.Domain.Model
{
    public class PatientFile
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required(ErrorMessage = "NewRoleName of file is required")]
        public string FileName { get; set; }
        [Required(ErrorMessage = "File Path is required")]
        public string FilePath { get; set; }

        public DateTime UploadDate { get; set; }

        public List<MedicalRecord> medicalRecords { get; set; }
        public List<Appoinment> Appoinments { get; set; }
    }
}
