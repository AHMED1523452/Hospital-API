namespace Hospital_API.Application.DTOs
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string token { get; set; }
        public string NewPassword { get; set; } //. this  for any user not doctor
    }
}
