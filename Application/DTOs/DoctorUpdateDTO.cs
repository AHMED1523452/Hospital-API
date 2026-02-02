using Hospital_API.Enums;
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Application.DoctorDTO
{
    public class DoctorUpdateDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
