using Hospital_API.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Application.DoctorDTO
{
    public class registerDoctorDTO
    {
        [Required(ErrorMessage = "Full NewRoleName is required")]
        public string FullName { get; set; }
        [EmailAddress,Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [DataType(DataType.Password),Required(ErrorMessage = "Password field is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        [Required(ErrorMessage = "(male | female) Gender is required")]
        public GenderEnum Gender { get; set; }
        [Required(ErrorMessage = "Specialty is required")]
        public DoctorSpecialty Specialty { get; set;  }

        [Required (ErrorMessage = "Department filed is required")]
        public Department DoctorDepartment { get; set; }

        [Required(ErrorMessage = "NationlId is required")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "Invalid National ID")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "License Number is required")]
        public string LicenseNumber { get; set; }
        public IFormFile? imageFile { get; set; } = null;
    }
}
