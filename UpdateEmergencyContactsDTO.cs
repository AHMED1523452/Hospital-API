using Hospital_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Application.EmergencyContactsDTO
{
    public class UpdateEmergencyContactsDTO
    {
        public string EC_FullName { get; set; }
        public RelationEnum Relation { get; set; }
        public string? OtherRelationText { get; set; }
        [Phone(ErrorMessage = "Send valid phone number")]
        public string PhoneNumber { get; set; }
        [Phone(ErrorMessage = "Send valid phone number")]
        public string AlternatePhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsPrimary { get; set; }
    }
}
