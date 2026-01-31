namespace Hospital_API.Application.PatientFileDTO
{
    public class patientFileRequestDTO
    {
        public IFormFile File { get; set; }
        public Guid patientId { get; set; }
    }
}
