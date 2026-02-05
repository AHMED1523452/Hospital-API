namespace Hospital_API.Application.VitalSignsDTO
{
    public class vitalSignsRequestDTO
    {
        public Guid appoinmentId { get; set; }
        public string bloodPressure { get; set; }
        public int heartRate { get; set; }
        public decimal Temperature { get; set; }
        public int oxygenSaturation { get; set; }
        public int respiratoryRate { get; set; }
        public decimal  height { get; set; }
        public decimal Weight { get; set; }
        public string Notes { get; set; }
    }
}

