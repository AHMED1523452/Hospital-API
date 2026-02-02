using HealthSync.Model;

namespace Hospital_API.Application.PrescptionsDTO.MedicineDTO
{
    public class GetMedicineDTO
    {
        public string MedicineName { get; set; }
        public string ScientificName { get; set; }
        public string DefaultDosage { get; set; }
        public string Form { get; set; }
        public string Strength { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
