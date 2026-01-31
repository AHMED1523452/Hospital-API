namespace Hospital_API.Application.NurseDTO
{
    public class NurseDTO
    {
        public Guid? NurseId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string  PhoneNumber { get; set; }
        public string Department { get; set; }
    }
}
