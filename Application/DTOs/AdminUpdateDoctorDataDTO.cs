namespace Hospital_API.Application.DoctorDTO
{
    public class AdminUpdateDoctorDataDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialty { get; set; }
        public bool? IsActive { get; set; }
    }
}
