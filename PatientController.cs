using HealthSync.DTOs;
using Hospital_API.DTO;
using Hospital_API.Model;
using Hospital_API.Services;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace Hospital_API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;

        public PatientController(IUnitOfWork unitOfWork, IUserService userService)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<PatientsList>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Nurse,Doctor")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<List<PatientsList>>>> getListOfPatients()
        {
            //. as default the UserId here is string not Guid 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userId, out Guid result))
            {
                if (User.IsInRole("Doctor"))
                {
                    return Ok(new ResponseDTOForGettingAPIs<List<PatientsList>>
                    {
                        Data = await unitOfWork.patientRepository.GetListForDoctorRole(result)
                    });
                }
                else if (User.IsInRole("Nurse"))
                {
                    return Ok(await unitOfWork.patientRepository.GetListOfPatientsForNurseRole(result));
                }
                else if (User.IsInRole("Admin"))
                {
                    return Ok(await unitOfWork.patientRepository.GetAllPatients());
                }
                return Unauthorized();
            }
            return Unauthorized();
        }
     
        [HttpGet("{patientId:guid}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<GettingPatientDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Doctor,Patient,Nurse")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<GettingPatientDTO>>> GetSpecificPatient(Guid patientId)
        {
            //. this will make O(1) not O(n) for the for complexity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Patient") && userId == patientId.ToString())
            {
                return await unitOfWork.patientRepository.GetPatientById(patientId);
            }
            else if (User.IsInRole("Doctor"))
            {
                if (Guid.TryParse(userId, out Guid result))
                {
                    var checking = await unitOfWork.patientRepository.CheckIfTheUserForTheSameDepartmentOfTheDoctor(patientId, result);
                    if (checking == true)
                        return await unitOfWork.patientRepository.GetPatientById(patientId);
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return await unitOfWork.patientRepository.GetPatientById(patientId);
        }

        [HttpGet("profile/me")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<GettingPatientDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Patient,Admin]")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<GettingPatientDTO>>> CurrentProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out Guid result))
            {
                return Ok(await unitOfWork.patientRepository.GetPatientById(result));
            }return Unauthorized();
        }

        [HttpPut("Update-patient-profile/{Id:guid}")]
        [ProducesResponseType(typeof(NewResponseDTO<DateTime?>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NewResponseDTO<DateTime?>>> UpdatePatientProfile(Guid Id, UpdatePatientDTO patientDTO)
        {
            var patient = await unitOfWork.patientRepository.GetPatient(Id);
            if (patient == null)
                return Unauthorized();

            //. Manual Mapping
            patient.User.FullName = patientDTO.FirstName + patientDTO.LastName;
            patient.User.DateOfBirth = patientDTO.DateOfBirth;
            patient.User.Gender = patientDTO.Gender.ToString();
            patient.User.PhoneNumber = patientDTO.PhoneNumber;
            patient.bloodTypes = patientDTO.BloodType.ToString();
            patient.CheckedIn = true;
            patient.ChronicDiseases = patientDTO.ChronicDiseases;
            patient.User.UpdateAt = DateTime.UtcNow;

            await unitOfWork.patientRepository.Update(patient);
            await unitOfWork.SaveAsync();

            return new NewResponseDTO<DateTime?>
            {
                IsSuccess = true,
                Message = "Patient updated successfully",
                Data = patient.User.UpdateAt
            };
        }

        [HttpPut("Update-current-patient-Profile")]
        [ProducesResponseType(typeof(NewResponseDTO<DateTime?>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<NewResponseDTO<DateTime?>>> UpdateCurrentPatientProfile(UpdatePatientDTO patientDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out Guid result))
            {
                var patient = await unitOfWork.patientRepository.GetPatient(result);
                if (patient == null)
                    return Unauthorized();

                //. not best practice 
                //. Manual Mapping
                patient.User.FullName = patientDTO.FirstName + patientDTO.LastName; 
                patient.User.DateOfBirth = patientDTO.DateOfBirth;
                patient.User.Gender = patientDTO.Gender.ToString();
                patient.User.PhoneNumber = patientDTO.PhoneNumber;
                patient.bloodTypes = patientDTO.BloodType.ToString();
                patient.CheckedIn = true;
                patient.User.UpdateAt = DateTime.UtcNow;

                await unitOfWork.patientRepository.Update(patient);
                await unitOfWork.SaveAsync();

                return new NewResponseDTO<DateTime?>
                {
                    IsSuccess = true,
                    Message = "Patient updated successfully",
                    Data = patient.User.UpdateAt
                };
            }
            return Unauthorized();
        }

        [HttpDelete("{Id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> SoftDelete(Guid Id)
        {
            var patient = await unitOfWork.patientRepository.GetPatient(Id);
            if (patient == null)
                return BadRequest();

            var user = await userService.GetUserByID(patient.UserId.ToString());
            if (user == null)
                return BadRequest();
            var result = await userService.RemoveUser(user);
            if (!result.IsSuccess)
                return BadRequest();

            return Ok(new
            {
                message = "User removed successfully"
            });
        }

        [HttpGet("{Id:guid}/emergency-contacts")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<GetEmergencyContactDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<GetEmergencyContactDTO>>> getEmergencyContact(Guid Id)
        {
            if (User.IsInRole("Patient") && User.Identity.IsAuthenticated)
            {
                Guid UserID = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (UserID != Id)
                    return Unauthorized();
                return Ok(await unitOfWork.patientRepository.GetEmergencyContact(UserID));
            }
            else if (User.IsInRole("Admin"))
                return Ok(await unitOfWork.patientRepository.GetEmergencyContact(Id));
            return Unauthorized();
        }
    }
}
