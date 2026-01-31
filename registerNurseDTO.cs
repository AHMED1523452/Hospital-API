using Hospital_API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Application.NurseDTO
{
    public class registerNurseDTO
    {
        [Required(ErrorMessage ="Full NewRoleName is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string  Email { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Send valid phone number ")]
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }

        [Required(ErrorMessage = "(male | female) Gender is required")]
        public GenderEnum Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Department field is required")]
        public Department Department { get; set; }

        [Required(ErrorMessage = "NationlId is required")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "Invalid National ID")]
        public string NationalId { get; set; }
        public IFormFile? imageFile { get; set; } = null;
    }
}
