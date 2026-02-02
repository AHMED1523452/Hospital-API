using Microsoft.Identity.Client;

namespace Hospital_API.Application.PrescptionsDTO.MedicineDTO
{
    public class UpdateMedicineDTO
    {
        public string medicineId { get; set; }
        public string medicineName { get; set; }
        public string ScientificName { get; set; }
        public string DefaultDosage { get; set; }
        public string Form { get; set; }
        public string Strength { get; set; }
        public bool IsActive { get; set; }
    }
}
