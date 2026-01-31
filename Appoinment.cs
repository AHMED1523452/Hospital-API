using Hospital_API.Domain.Enums;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_API.Domain.Model
{
    public class Appoinment
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "DoctorId is required")]
        public Guid DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required(ErrorMessage = "Patient Id is required")]
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }

        public DateOnly? AppointmentDate { get; set; }
        [Required(ErrorMessage= "Start At time is required")]
        public TimeOnly StartAt { get; set; }
        [Required(ErrorMessage = "End at time is required")]
        public TimeOnly EndAt { get; set; }
        [Required(ErrorMessage = "Send the reason of this appoinment")]
        public string reason { get; set; }
        //. here the enum must be in the DTO not in the entity 
        [Required]
        public AppoinmentStatus Status { get; set; } // Pending - Completed - Cancelled

        public bool IsCancelled { get; set; } = false; //. in the Delete API 
        public bool IsPrepared { get; set; } = false;
        public List<Presception>?  Prescription { get; set; }
    }
}
