using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;

namespace Hospital_API.Application.MedialRecordDTO
{
    public class MedicalRecordByPatientFileDTO
    {
        public string recordId { get; set; }
        public string diagnosis { get; set; }
        public bool isClosed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
