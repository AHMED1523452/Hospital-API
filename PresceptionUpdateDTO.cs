using HealthSync.Model;
using Microsoft.Identity.Client;

namespace Hospital_API.Application.PrescptionsDTO
{
    public class PresceptionUpdateDTO
    {
        public string Notes { get; set; }
        public bool IsActive { get; set; }
    }
}
