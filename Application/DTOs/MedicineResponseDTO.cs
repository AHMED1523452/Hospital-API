using Microsoft.Identity.Client;

namespace Hospital_API.Application.PrescptionsDTO.MedicineDTO
{
    public class MedicineResponse
    {
        public string medicineId { get; set; }
        public string medicineName { get; set; }
        public string message { get; set; }
    }
}
