using Microsoft.Identity.Client;

namespace Hospital_API.Application.PrescptionsDTO
{
    public class PresceptionMedicineDTO
    {
        public Guid medicineId { get; set; }
        public string dosage { get; set; }
        public string frequency { get; set; }
        public int durationInDays { get; set; }
        public string route { get; set; }
        public string instructions { get; set; }
    }
}
