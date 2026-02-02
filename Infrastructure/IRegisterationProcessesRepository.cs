using Hospital_API.DTO;
using Hospital_API.Model;

namespace Hospital_API.Services
{
    public interface IRegisterationProcessesRepository
    {
        Task<AuthResponseDTO> Login(LoginDTO dTO);
        Task<Result> RegisterDoctor(registerDoctorDTO registerDTO, IWebHostEnvironment env);
        Task<Result> RegisterAdmin(registerAdminDTO dTO, IWebHostEnvironment env);
        Task<Result> RegisterNurse(registerNurseDTO dTO, IWebHostEnvironment env);
        Task<Result> RegisterPatient(registerPatientDTO dTO, IWebHostEnvironment env);
        Task<AuthResponseNewAccessTokenDTO> RefreshToken(RefreshTokenDTO refreshToken);
        Task ForgotPassword(ForgotPasswordDTO forgotPassword);
        Task ResetPassword(ResetPasswordDTO resetPassword);
        Task<RefreshTokens> GetRefreshTokenUsing(string refreshToken, string UserId);
        Task RevokeRefreshToken(RefreshTokens refresh);
    }
}
