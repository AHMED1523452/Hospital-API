using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Hospital_API.Domain.Model
{
    public class vitalSigns
    {
        public Guid Id { get; set; }
        public Guid? NurseId { get; set; }
        public Nurse? Nurse { get; set; }
        public Guid? AppoinmentId { get; set; }
        public Appoinment? Appoinment { get; set; }

        public string BloodPressure { get; set; }
        public int HeartRate { get; set; }
        [Precision(18, 3)]
        public decimal Temperature { get; set; }
        public int OxygenSaturation { get; set; }
        public int RespiratoryRate { get; set; }

        [Precision(18,3)]
        public decimal? Height { get; set; }
        [Precision(18, 3)]
        public decimal? Weight { get; set; }
        [Precision(18, 3)]
        public string Notes { get; set; }
        public bool IsDeleted { get; set; } = false;

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}
