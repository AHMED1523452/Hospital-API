using Hospital_API.DTO;
using Hospital_API.Model;
using Hospital_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.JSInterop.Infrastructure;
using Microsoft.OpenApi.Writers;
using System.Security.Claims;

namespace Hospital_API.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IRegisterationProcessesRepository registerationProcesses;
        private readonly IWebHostEnvironment env;
        private readonly IDoctorRepository doctorRepository;
        private readonly IDoctorAuthService doctorAuth;
        private readonly IUserService userService;

        public AccountController(IRegisterationProcessesRepository registerationProcesses, 
            IWebHostEnvironment env, 
            IDoctorRepository doctorRepository,
            IDoctorAuthService doctorAuth,
            IUserService userService)
        {
            this.registerationProcesses = registerationProcesses;
            this.env = env;
            this.doctorRepository = doctorRepository;
            this.doctorAuth = doctorAuth;
            this.userService = userService;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginDTO dTO)
        {
             return Ok(await registerationProcesses.Login(dTO));
        }

        [HttpPost("refreshToken")]
        [ProducesResponseType(typeof(AuthResponseNewAccessTokenDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseNewAccessTokenDTO>> RefreshToken(RefreshTokenDTO refreshToken)
        {
             return Ok(await registerationProcesses.RefreshToken(refreshToken));
        }
        
        [HttpPatch("revoke-refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RevokeRefreshToken(RefreshTokenDTO refreshToken)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var getexistingRefreshToken = await registerationProcesses.GetRefreshTokenUsing(refreshToken.RefreshToken, currentUserId);
            if (getexistingRefreshToken == null)
                return BadRequest();

            await registerationProcesses.RevokeRefreshToken(getexistingRefreshToken);

            return Ok(new
            {
                IsSuccess = true,
                message = "Token has been revoked"
            });
        }

        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Authorize(Roles = "ApplicationUser")]
        public async Task<ActionResult> ChangePasswordForDoctorRole(ChangeDoctorPasswordDTO changeDTO)
        {
            var user = await doctorRepository.GetDoctorByEmail(changeDTO.Email);
            await userService.CheckActivationForUser(await userService.GetUserByEmail(changeDTO.Email));
            await doctorAuth.ChangePassword(changeDTO, changeDTO.CurrentPassword, changeDTO.NewPassword);


            return Ok(new
            {
                IsSuccess = true ,
                message = "Password has been changed successfully"
            });
        }

        [HttpPost("RegisterDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //.[Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RegisterDoctor([FromForm] registerDoctorDTO dTO)
        {
            var validation = await registerationProcesses.RegisterDoctor(dTO, env);
            if (!validation.IsSuccess)
                return BadRequest();
            return Ok(new
            {
                FullName = dTO.FullName,
                Email = dTO.Email,
            });
        }

        [HttpPost("RegisterAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromForm] registerAdminDTO dTO)
        {
            await registerationProcesses.RegisterAdmin(dTO, env);
            return Ok(new
            {
                FullName = dTO.FullName,
                Email = dTO.Email,
            });
        }

        [HttpPost("RegisterNurse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> RegisterNurse([FromForm] registerNurseDTO dTO)
        {
            await registerationProcesses.RegisterNurse(dTO, env);
            return Ok(new
            {
                FullName = dTO.FullName,
                Email = dTO.Email,
                PhoneNumber = dTO.PhoneNumber,
            });
        }

        [HttpPost("RegisterPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterPatient([FromForm] registerPatientDTO dTO)
        {
            await registerationProcesses.RegisterPatient(dTO, env);
            return Ok(new
            {
                FullName = dTO.FullName,
                Email = dTO.Email,
                PhoneNumber = dTO.PhoneNumber
            });
        }

        [HttpPost("ForgotPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPassword)
        { 
             await registerationProcesses.ForgotPassword(forgotPassword);
             return Ok(new
             {
                 IsSuccess = true,
                 message = "If the email exists, a reset link has been sent."
             });
        }

        [HttpPost("ResetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPassword)
        {
                await registerationProcesses.ResetPassword(resetPassword);
                return Ok(new
                {
                    IsSuccess = true,
                    message = "Password has been successfully reset"
                });
        }

        

        [HttpGet("debug-token")]
        //[Authorize]
        public async Task<IActionResult> DebugToken()
        {
            return Ok(User?.Identity?.IsAuthenticated);
        }
    }
}
