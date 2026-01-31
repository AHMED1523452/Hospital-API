using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Domain.Model
{
    public class EmergencyContact
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name field is required"), MaxLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Relation field is requied"), MaxLength(50)]
        public string Relation { get; set; }
        [Required(ErrorMessage = "Phone Number field is required"), Phone(ErrorMessage = "Send Valid phone number")]
        public string  PhoneNumber { get; set; }

        public string? AlternatePhone { get; set; }
        public string? Notes { get; set; }
        public string? Address { get; set; }
        public bool IsPrimary { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
     //. Foreign Keys for the Emergency Contacts
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }
    }
}
