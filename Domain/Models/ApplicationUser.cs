using Hospital_API.Controllers;
using Hospital_API.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Domain.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "National ID is required")]
        public string NationalId { get; set; }
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public string? ImagePath { get; set; }

        // Soft Delete / Deactivation
        public bool IsActive { get; set; } = true;

        public DateTime? CreatedAt = DateTime.UtcNow;
        public DateTime? UpdateAt { get; set; }
    }
}
