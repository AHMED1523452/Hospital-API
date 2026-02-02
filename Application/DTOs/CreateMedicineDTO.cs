using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Application.PrescptionsDTO.MedicineDTO
{
    public class CreateMedicineDTO
    {
        [Required(ErrorMessage = "Medicine name field is required")]
        public string MedicineName { get; set; }
        [Required(ErrorMessage = "Scientific Name is required")]
        public string ScientificName { get; set; }
        [DataType(DataType.Text)]
        public string DefaultDosage { get; set; }
        public string Form { get; set; }
        public string Strength { get; set; }
    }
}
