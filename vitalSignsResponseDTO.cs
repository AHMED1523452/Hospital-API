using Microsoft.Identity.Client;

namespace Hospital_API.Application.VitalSignsDTO
{
    public class vitalSignsResponseDTO
    {
        public string Id { get; set; }
        public string appoinmentId { get; set; }
        public string NurseId { get; set; }
        public string RecordedAt { get; set; }
    }
}
