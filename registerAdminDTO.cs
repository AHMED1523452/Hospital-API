using Hospital_API.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Application.DTOs
{
    public class registerAdminDTO
    {
        [Required(ErrorMessage = "Full NewRoleName is required"), DataType(DataType.Text)]
        public string FullName { get; set; }

        [EmailAddress, Required(ErrorMessage ="Email is required")]
        public string Email { get; set; }


        [DataType(DataType.Password),  Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string? Address { get; set; }

        [Required(ErrorMessage = "NationlId is required")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "Invalid National ID")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "Gender field is required")]
        public GenderEnum Gender { get; set; }

        public IFormFile? imagePath { get; set; } = null;
    }
}
