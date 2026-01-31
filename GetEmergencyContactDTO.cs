namespace Hospital_API.Application.EmergencyContactsDTO
{
    public class GetEmergencyContactDTO
    {
        public string FullName { get; set; }
        public string Relation { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhone { get; set; }
        public string Address { get; set; }
        public bool IsPrimary { get; set; }
    }
}
