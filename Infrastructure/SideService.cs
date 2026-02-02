using Hospital_API.DTO;
using Microsoft.AspNetCore.Http.Metadata;
using System.Transactions;

namespace HealthSync.Services
{
    public class SideService : ISideService
    {
        public async Task<string> UploadImage(IFormFile file, IWebHostEnvironment env)
        {
            var folderPath = Path.Combine(env.WebRootPath, "Images/UsersImage");

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string imagePath = "Iamges/UsersImage/" + fileName;
            return imagePath;
        }

        public async Task<Result> CheckingFileValidation(IFormFile file)
        {
            if (file.Length > 5 * 1024 * 1024)
                return new Result().Failure("file more than limit size ");

            var extensions = new[] { ".jpg", ".png", ".jpeg" };
            if (Path.Exists(file.FileName))
                return new Result().Failure("Send another imgae, image already exist");

            if (!extensions.Contains(Path.GetExtension(file.FileName)))
                return new Result().Failure("only image allow to upload");
            return new Result().Success;
        }

        public async Task<Result> checkingPhoneNumverAndNationalId(string nationalId, string PhoneNumber)
        {
            if (nationalId.Length < 14 || nationalId.Length > 14)
            {
                foreach (var items in nationalId)
                {
                    if (!char.IsDigit(items))
                    {
                        return new Result().Failure("invalid National ID ");
                    }
                }
            }

            if (PhoneNumber.Length < 11 || PhoneNumber.Length > 11)
            {
                foreach (var items in PhoneNumber)
                {
                    if (!char.IsDigit(items))
                    {
                        return new Result().Failure("invalid Phone Number");
                    }
                }
            }
            return new Result().Success;
        }
    }
}
