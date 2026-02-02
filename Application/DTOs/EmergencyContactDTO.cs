using Hospital_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Application.EmergencyContactsDTO
{
    public class EmergencyContactDTO
    {
        public string PatientName { get; set; }
        public string EC_FullName { get; set; }
        public RelationEnum Relation { get; set; } 
        public string? OtherRelationText { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternatePhone { get; set; }
        public string? Notes { get; set; }
        public string? Address { get; set; }
        public bool IsPrimary { get; set; }
    }
}
