namespace Hospital_API.Application.PatientDTO
{
    public class PatientsList
    {
        public string PatientId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string BloodType { get; set; }
        public string ChronicDisease  { get; set; }
        public bool IsActive { get; set; }
    }
}
