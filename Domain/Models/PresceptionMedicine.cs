using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Domain.Model
{
    public class PresceptionMedicine
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PresceptionId { get; set; }
        public Presception Presception { get; set; }
        public Guid MedicineId { get; set; }
        public Medicine Medicine { get; set; }
        [Required(ErrorMessage = "Dosage field is required")]
        public string Dosage { get; set; }
        [Required(ErrorMessage = "Frequency field is required")]
        public string Frequency { get; set; }
        [Required(ErrorMessage = "Duration In Days field is required")]
        public int DurationInDays { get; set; }
        [Required(ErrorMessage = "Route field is required ")]
        public string Route { get; set; }
        public string? Instructions { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
