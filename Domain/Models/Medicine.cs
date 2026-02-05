using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Domain.Model
{
    public class Medicine
    {
        public Guid Id { get; set; }
        public string MedicineName { get; set; }

        public string ScientificName { get; set; }
        public string DefaultDosage { get; set; }
        public string Form { get; set; }
        public string Strength { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PresceptionMedicine> presceptionMedicines { get; set; }
    }
}
