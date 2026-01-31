using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Application.DoctorDTO
{
    public class ChangeDoctorPasswordDTO
    {
        [EmailAddress, Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage= "This input field is required")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "Password is required"),DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Compare("NewPassword"), Required(ErrorMessage ="This input field is required")]
        public string ConfirmNewPassword { get; set; }
    }
}
