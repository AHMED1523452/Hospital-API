using HealthSync.Model;
using Hospital_API.Domain.Enums;
using Hospital_API.Model;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Application.PatientDTO
{
    public class UpdatePatientDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public GenderEnum Gender { get; set; }

        [Phone(ErrorMessage = "enter valid phone number")]
        [RegularExpression(@"^(\+20)?1[0-9]{9}$", ErrorMessage = "Invaid phone number")]
        public string PhoneNumber { get; set; }

        public BloodTypes? BloodType { get; set; }
        public string ChronicDiseases { get; set; }
    }
}
