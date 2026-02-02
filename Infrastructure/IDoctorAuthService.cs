using Hospital_API.DTO;

namespace Hospital_API.Services
{
    public interface IDoctorAuthService
    {
        Task ChangePassword(ChangeDoctorPasswordDTO doctorDTO, string currentPassword, string newPassword);
    }
}
