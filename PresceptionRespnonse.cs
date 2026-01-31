using HealthSync.Model;

namespace Hospital_API.Application.PrescptionsDTO
{
    public class PresceptionRespnonse
    {
        public Guid PresceptionID { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
