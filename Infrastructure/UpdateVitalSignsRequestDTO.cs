namespace Hospital_API.Application.VitalSignsDTO
{
    public class UpdateVitalSignsRequestDTO
    {
        public string bloodPressure { get; set; }
        public int heartRate { get; set; }
        public decimal temperature { get; set; }
        public string Notes { get; set; }
    }
}
