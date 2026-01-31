using Hospital_API.DTO;

namespace HealthSync.Services
{
    public interface ISideService
    {
        Task<string> UploadImage(IFormFile file, IWebHostEnvironment env);
        Task<Result> CheckingFileValidation(IFormFile file);
        Task<Result> checkingPhoneNumverAndNationalId(string nationalId, string PhoneNumber);
    }
}
