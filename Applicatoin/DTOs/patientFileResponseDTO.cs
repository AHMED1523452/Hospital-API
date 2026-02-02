using Microsoft.Identity.Client;

namespace Hospital_API.Application.PatientFileDTO
{
    public class patientFileResponseDTO
    {
        public string Id { get; set; }
        public string patientId { get; set; }
        public string fileName { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
