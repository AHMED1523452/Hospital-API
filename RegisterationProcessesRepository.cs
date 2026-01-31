using HealthSync.Services;
using Hospital_API.ApplicationDbContext;
using Hospital_API.DTO;
using Hospital_API.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Runtime.Versioning;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Hospital_API.Services
{
    /// <summary>
    /// you need in this Service to reduce the time complexity and remove the foreach or any loops as you can
    /// </summary>
    public class RegisterationProcessesRepository : IRegisterationProcessesRepository
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AppDbContext dbContext;
        private readonly IHttpContextAccessor httpContext;
        private readonly ITokenIPService tokenIP;
        private readonly IEmailService emailService;
        private readonly ISideService sideService;
        private readonly ILogger logger;

        public IConfiguration Configuration { get; }

        public RegisterationProcessesRepository(UserManager<ApplicationUser> userManager, 
            IConfiguration configuration,AppDbContext dbContext
            , IHttpContextAccessor httpContext , 
            ITokenIPService tokenIP,
            IEmailService emailService, 
            ISideService sidService,
            ILogger<RegisterationProcessesRepository> logger)
        {
            this.userManager = userManager;
            Configuration = configuration;
            this.dbContext = dbContext;
            this.httpContext = httpContext;
            this.tokenIP = tokenIP;
            this.emailService = emailService;
            this.sideService = sidService;
            this.logger = logger;
        }
        public async Task<Result> RegisterDoctor(registerDoctorDTO registerDTO, IWebHostEnvironment env)
        {
            string? imagePath = null;
            if (registerDTO.imageFile != null)
            {
                var validationImage = await sideService.CheckingFileValidation(registerDTO.imageFile);
                if (!validationImage.IsSuccess)
                    return new Result().Failure("something wrong in your file");

                imagePath = await sideService.UploadImage(registerDTO.imageFile, env);
            }
            var user = new ApplicationUser
            {
                FullName = registerDTO.FullName,
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                Gender = registerDTO.Gender.ToString(),
                ImagePath = imagePath,
                IsActive = true,
            };

            var CorrectPhoneNumberAndNationalId = await sideService.checkingPhoneNumverAndNationalId(registerDTO.NationalId,registerDTO.PhoneNumber);
            if (!CorrectPhoneNumberAndNationalId.IsSuccess)
                return new Result().Failure("Invalid nationalId or PhoneNumber");


            user.NationalId = registerDTO.NationalId;
            user.PhoneNumber = registerDTO.PhoneNumber;
            user.Address = registerDTO.Address;

            //. this in memory
            var doctor = new Doctor
            {
                UserId = user.Id,
                LicenseNumber = registerDTO.LicenseNumber,
                Specialty = registerDTO.Specialty.ToString(),
                Department = registerDTO.DoctorDepartment.ToString()
            };

            if(await userManager.Users.Where(op => op.NationalId == user.NationalId || op.Email == user.Email).FirstOrDefaultAsync() != null)
            {
                return new Result().Failure("This user is already exist"); 
            }

            var result = await userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Doctor");
            }
            else
            {
                var errors = string.Join("|", result.Errors.Select(op => op.Description));

                return new Result().Failure(errors);
            }

            await dbContext.doctors.AddAsync(doctor);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(
                "User {UserName} is registered at {CreatedAt}",
                 user.FullName,
                 DateTime.Now);

            return new Result().Success;
        }

        public async Task<Result> RegisterNurse(registerNurseDTO dTO, IWebHostEnvironment env)
        {
            string imagePath = null;
            if (dTO.imageFile != null)
            {
                var validation = await sideService.CheckingFileValidation(dTO.imageFile);
                if (!validation.IsSuccess)
                    return new Result().Failure("Something wrong in your file");

                imagePath = await sideService.UploadImage(dTO.imageFile, env);
            }
            var user = new ApplicationUser
            {
                FullName = dTO.FullName,
                UserName = dTO.Email,
                Email = dTO.Email,
                Gender = dTO.Gender.ToString(),
                ImagePath = imagePath,
            };

            var CorrectPhoneNumberAndNationalId = await sideService.checkingPhoneNumverAndNationalId(dTO.NationalId, dTO.PhoneNumber);
            if (!CorrectPhoneNumberAndNationalId.IsSuccess)
                return new Result().Failure("Invalid nationalId or PhoneNumber");

            user.NationalId = dTO.NationalId;
            user.PhoneNumber = dTO.PhoneNumber;
            user.Address = dTO.Address;

            var Nurse = new Nurse
            {
                UserId = user.Id,
                Department = dTO.Department.ToString()
            };

            var result = await userManager.CreateAsync(user, dTO.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Nurse");
            }
            else
            {
                var errors = string.Join("|", result.Errors.Select(op => op.Description));
                return new Result().Failure(errors);
            }

            //. adding a new nurse in the table of nurse 
            await dbContext.nurses.AddAsync(Nurse);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(
                "User {UserName} is registered at {CreatedAt}",
                 user.FullName,
                 DateTime.Now);

            return new Result().Success;
        }

        public async Task<Result> RegisterPatient(registerPatientDTO dTO, IWebHostEnvironment env)
        { 
            string imagePath = null;
            if (dTO.imagePath != null)
            {
                var validation = await sideService.CheckingFileValidation(dTO.imagePath);
                if (!validation.IsSuccess)
                    return new Result().Failure("Something wrong in your file");

                imagePath = await sideService.UploadImage(dTO.imagePath, env);

            }
            //. patient is not a department property
            var user = new ApplicationUser
            {
                FullName = dTO.FullName,
                UserName = dTO.Email,
                Email = dTO.Email,
                Gender = dTO.Gender.ToString(),
                ImagePath = imagePath,
                //IsActive= true //. that is the default value for the IsActive property
            };

            var CorrectPhoneNumberAndNationalId = await sideService.checkingPhoneNumverAndNationalId(dTO.NationalId, dTO.PhoneNumber);
            if (!CorrectPhoneNumberAndNationalId.IsSuccess)
                return new Result().Failure("Invalid nationalId or PhoneNumber");


            user.NationalId = dTO.NationalId;
            user.PhoneNumber = dTO.PhoneNumber;
            user.Address = dTO.Address;

            Guid UserId = Guid.Parse(user.Id);

            var patient = new Patient
            {
                UserId = UserId,
                bloodTypes = dTO.bloodTypes.ToString()
            };

            //. adding a new user as a patient 
            var result = await userManager.CreateAsync(user, dTO.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Patient");
            }

            else
            {
                var errors = string.Join("|", result.Errors.Select(op => op.Description));
                return new Result().Failure(errors);
            }

            //. adding a patient in the table of Patint
            await dbContext.patients.AddAsync(patient);
            //. don't need to add a unitOfWork due to existing a one savechanges Async not always will need it
            await dbContext.SaveChangesAsync();

            logger.LogInformation(
                "User {UserName} is registered at {CreatedAt}",
                 user.FullName,
                 DateTime.Now);


            return new Result().Success;
        }

        public async Task<Result> RegisterAdmin(registerAdminDTO dTO , IWebHostEnvironment env)
        {
            string? imagePath = null;
            if (dTO.imagePath != null)
            {
                var validtion  = await sideService.CheckingFileValidation(dTO.imagePath);
                if (!validtion.IsSuccess)
                    return new Result().Failure("Something wrong in your file");
                imagePath = await sideService.UploadImage(dTO.imagePath, env);
            }

            var user = new ApplicationUser
            {
                FullName = dTO.FullName,
                UserName = dTO.Email,
                Email = dTO.Email,
                ImagePath = imagePath,
                Gender = dTO.Gender.ToString(),
                NationalId = dTO.NationalId
            };

            if (dTO.NationalId.Length < 14 || dTO.NationalId.Length > 14)
            {
                foreach (var items in dTO.NationalId)
                {
                    if (!char.IsDigit(items))
                    {
                        return new Result().Failure("invalid National ID ");
                    }
                }
            }

            var result = await userManager.CreateAsync(user, dTO.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
                return new Result().Failure("Something invalid in the inputs");

            logger.LogInformation(
                "User {UserName} is registered at {CreatedAt}",
                 user.FullName,
                 DateTime.Now);

            return new Result().Success;
        }

        //. don't match with the SRP solid principles
        //. hard in testing
        public async Task<AuthResponseDTO> Login(LoginDTO dTO)
        {
            var user = await userManager.FindByEmailAsync(dTO.Email);
            if (user == null)
                throw new Exception("this user is not exist");
            if (!user.IsActive)
                throw new Exception("Your account has been deactivated. Please contact the administrator.");

            if (await userManager.IsLockedOutAsync(user))
                throw new Exception("Your account is temporarily locked. Try again later.");

            bool CheckPasswordOfUser = await userManager.CheckPasswordAsync(user, dTO.Password);
            if (!CheckPasswordOfUser) {
                user.AccessFailedCount += 1;
                throw new Exception("Invalid password or Email");
            }

            var Authclaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),

                new Claim(ClaimTypes.Email, user.Email),
            };

            //. adding the roles of each user 
            var authRoles = await userManager.GetRolesAsync(user);
            foreach(var items in authRoles)
            {
                Authclaims.Add(new Claim(ClaimTypes.Role, items));
            }
            //. Signing Key from the json file 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessToken = new JwtSecurityToken
                (
                   issuer: Configuration["JWT:Issuer"],
                   audience: Configuration["JWT:Audience"],
                   claims: Authclaims,
                   expires: DateTime.UtcNow.AddMinutes(double.Parse(Configuration["JWT:AccessTokenDurationMinutes"])),
                   signingCredentials: cred
                );

            var refreshToken = new JwtSecurityToken
                (
                   issuer: Configuration["JWT:Issuer"],
                   audience: Configuration["JWT:Audience"],
                   claims: Authclaims,
                   expires: DateTime.UtcNow.AddDays(double.Parse(Configuration["JWT:RefreshTokenDurationDays"])),
                   signingCredentials: cred
                );
            string OriginAccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken);
            var RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken);

            var AccessTokenModel = new RefreshTokens
            {
                UserID = user.Id,
                RefreshToken = RefreshToken,
                ExpireDateForRefreshToken = refreshToken.ValidTo,
                CreatedAt = accessToken.ValidFrom,
                CreatedByIp = tokenIP.GetClientIP(),
                IsRevoked = false
            };

            await dbContext.refreshTokens.AddAsync(AccessTokenModel);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(
                "User {UserName} logged in at {loggedAt}",
                 user.FullName,
                 DateTime.Now);

            return new AuthResponseDTO
            {
                AccessToken = OriginAccessToken,
                RefreshToken = RefreshToken,
                ExpireDateForAccessToken = accessToken.ValidTo,
                ExpireDateForRefreshToken = refreshToken.ValidTo
            };
        }

        public async Task<AuthResponseNewAccessTokenDTO> RefreshToken (RefreshTokenDTO refreshToken)
        {
            var FindExistingRefreshToken = await dbContext.refreshTokens
                .Where(op => op.RefreshToken == refreshToken.RefreshToken && op.IsRevoked == false).FirstOrDefaultAsync();

            //. checking if the refresh token is already exist or not 
            if (FindExistingRefreshToken == null)
                throw new Exception("invalid Refresh token");

            //. checking if the expire date is done or not 
            if (FindExistingRefreshToken.ExpireDateForRefreshToken < DateTime.UtcNow)
                throw new Exception("Invalid refresh token");

            //.checking if the refresh token is revoked or not 
            if (FindExistingRefreshToken.IsRevoked) //. has initial value true not false --> note this 
                throw new Exception("token is revoked");

            //. checking if the current Ip is the Ip of stored refresh token 
            //. if not will make the refresh token is revoked to make no body can use this refresh token
            if (FindExistingRefreshToken.CreatedByIp != tokenIP.GetClientIP())
            {
                FindExistingRefreshToken.IsRevoked = true;

                dbContext.refreshTokens.Update(FindExistingRefreshToken);
                await dbContext.SaveChangesAsync();
                throw new Exception("token has not been allow to use");
            }

            var user = await userManager.FindByIdAsync(FindExistingRefreshToken.UserID);
            if (user == null)
                throw new Exception("User not found");

            var AuthClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var AuthRoles = await userManager.GetRolesAsync(user);
            foreach(var items in AuthRoles)
            {
                AuthClaims.Add(new Claim(ClaimTypes.Role, items));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var NewAccessToken = new JwtSecurityToken
                (
                   issuer: Configuration["JWT:Issuer"],
                   audience: Configuration["JWT:Audience"],
                   claims: AuthClaims,
                   signingCredentials: creds,
                   expires: DateTime.UtcNow.AddMinutes(double.Parse(Configuration["JWT:AccessTokenDurationMinutes"]))
                );

            logger.LogInformation(
                "User {UserName} requested to get refresh token at {refreshTokenAt}",
                 user.FullName,
                 DateTime.Now);

            return new AuthResponseNewAccessTokenDTO
            {
                NewAccessToken = new JwtSecurityTokenHandler().WriteToken(NewAccessToken),
                ExpireDateForNewAccessToken = NewAccessToken.ValidTo
            }; 
        }

        public async Task ForgotPassword(ForgotPasswordDTO forgotPassword)
        {
            //. check if the user is existing in the DB or not
            var user = await userManager.FindByEmailAsync(forgotPassword.Email);
            if (user == null)
                throw new Exception("Invalid Email");
            try
            {
                //. Generating Token to send it in mail 
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"https://frontend.com/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";

                var htmlBody = $@"
                    <div style='font-family: Arial, sans-serif; padding:20px; border:1px solid #ddd; border-radius:10px;'>
                        <h2>Hello {user.FullName},</h2>
                        <p>You requested a password reset.</p>
                        <p><strong>Your token is:</strong> {token}</p>
                        <p>Or you can click the link below to reset your password directly:</p>
                        <a href='{resetLink}' style='display:inline-block; padding:10px 20px; background-color:#007bff; color:#fff; text-decoration:none; border-radius:5px;'>Reset Password</a>
                        <p style='margin-top:15px;'>If you did not request this, please ignore this email.</p>
                    </div>
                      ";

                logger.LogInformation(
                    "User {UserName} forgot password and has been sent to him an email at {SentAt}",
                    user.FullName,
                    DateTime.Now);

                await emailService.SendEmail(user.Email, "Password Reset Request", htmlBody);
            }
            catch(SmtpException ex)
            {
                logger.LogError("Something fault occured at Forgot password service");
                throw new SmtpException(ex.Message);
            }
        }

        public async Task ResetPassword(ResetPasswordDTO resetPassword)
        {
            var user = await userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null) throw new Exception("Invalid data");

            var result = await userManager.ResetPasswordAsync(user, resetPassword.token, resetPassword.NewPassword);

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            if (!result.Succeeded)
            {
                logger.LogError("Something invalid in the reset password service");

                throw new Exception(errors); //. will be handeled at Global middleware 
            }
            logger.LogInformation(
                 "User {UserName} reseted his password at {CreatedAt}",
                  user.FullName,
                  DateTime.Now);

        }

        public async Task<RefreshTokens> GetRefreshTokenUsing(string refreshToken, string UserId)
        {
            //. if the Authorize is running the query will be like that 
            /*
             * var getExistingRefreshToken = await dbContext.refreshTokens.FirsOrDefaultAsync(op => op.RefreshToken == refreshTokens && op.UserId == UserId
                 && op.IsRevoked == false
             */
            var GetExistingRefreshToken = await dbContext.refreshTokens.FirstOrDefaultAsync(op => op.RefreshToken == refreshToken && op.IsRevoked == false);
            return GetExistingRefreshToken;
        }

        public async Task RevokeRefreshToken(RefreshTokens refresh)
        {
            refresh.IsRevoked = true;
            dbContext.refreshTokens.Update(refresh);
            await dbContext.SaveChangesAsync();

            logger.LogInformation(
                "User {tokenId} has been revoked at {CreatedAt}",
                 refresh.Id,
                 DateTime.Now);
        }
    }
}
