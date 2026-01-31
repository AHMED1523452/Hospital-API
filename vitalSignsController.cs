using HealthSync.DTOs;
using Hospital_API.DTO;
using Hospital_API.Model;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Hospital_API.Controllers
{
    [ApiController]
    [Route("api/vital-signs")]
    public class vitalSignsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        public vitalSignsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        [ProducesResponseType(typeof(vitalSignsResponseDTO),StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles= "Admin,Nurse")]
        public async Task<ActionResult<vitalSignsResponseDTO>> Create(vitalSignsRequestDTO vitalSigns)
        {
            //. manual mapping in memory
            var signs = new vitalSigns
            {
                AppoinmentId = vitalSigns.appoinmentId,
                BloodPressure = vitalSigns.bloodPressure,
                HeartRate = vitalSigns.heartRate,
                Height = vitalSigns.height,
                Notes = vitalSigns.Notes,
                OxygenSaturation = vitalSigns.oxygenSaturation,
                RecordedAt = DateTime.UtcNow,
                RespiratoryRate = vitalSigns.respiratoryRate,
                Temperature = vitalSigns.Temperature,
                Weight = vitalSigns.Weight
            };
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out Guid result))
            {
                if (User.IsInRole("Nurse"))
                {
                    signs.NurseId = result;

                    await unitOfWork.vitaSignsRepository.Create(signs);
                    await unitOfWork.SaveAsync();

                    return Ok(new vitalSignsResponseDTO
                    {
                        Id = signs.Id.ToString(),
                        appoinmentId = signs.AppoinmentId.ToString(),
                        NurseId = result.ToString(),
                        RecordedAt = signs.RecordedAt.ToString()
                    });
                }
            }
            await unitOfWork.vitaSignsRepository.Create(signs);
            await unitOfWork.SaveAsync();

            return Ok(new vitalSignsResponseDTO
            {
                Id = signs.Id.ToString(),
                appoinmentId = signs.AppoinmentId.ToString(),
                NurseId = null,
                RecordedAt = signs.RecordedAt.ToString()
            }); 
        }

        [HttpGet("appoinments/{appoinmentId}")]
        [ProducesResponseType(typeof(GetVitalSignsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Doctor,Nurse")]
        public async Task<ActionResult<GetVitalSignsDTO>> vitalSignsByAppoinmentId(Guid appoinmentId)
        {
            var signs = await unitOfWork.vitaSignsRepository.GetVitalSignsAsync(appoinmentId);
            if (signs == null)
                return BadRequest();
            string doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(doctorId, out Guid result)){
                if (User.IsInRole("Doctor"))
                {
                    //. must see the shape details of the vitalsigns for the appoinment that he has , are you understand what i mean ? 
                    if (result != await unitOfWork.vitaSignsRepository.GetDoctorId(appoinmentId))
                        return Unauthorized();
                }
                return Ok(signs);
            }
            return Unauthorized();
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<VitalSignsDTO>),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<VitalSignsDTO>>> GetVitalSignsByPatientId(Guid patientId)
        {
            var signs = await unitOfWork.vitaSignsRepository.GetVitalSignsByPatientId(patientId);
            if (signs == null)
                return NotFound();
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(UserId, out Guid result))
            {
                if (User.IsInRole("Doctor"))
                {
                    if (result != await unitOfWork.vitaSignsRepository.GetDoctorId(patientId, result))
                        return Unauthorized();
                }
                return Ok(new ResponseDTOForGettingAPIs<VitalSignsDTO>
                {
                    Data = signs
                });
            }
            return Unauthorized();
        }

        [HttpPut("{signsId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Admin,Nurse")]
        public async Task<ActionResult> UpdateSigns(Guid signsId,[FromForm] UpdateVitalSignsRequestDTO requestDTO)
        {
            var signs = await unitOfWork.vitaSignsRepository.GetVitalSigns(signsId);
            if (signs == null)
                return NotFound();
            string nurseId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(nurseId, out Guid result))
            {
                if (User.IsInRole("Nurse"))
                {
                    if (result == await unitOfWork.vitaSignsRepository.GetNurseIdThatCreatedThisSigns(signsId))
                        return Unauthorized();
                }
                signs.BloodPressure = requestDTO.bloodPressure;
                signs.Temperature = requestDTO.temperature;
                signs.HeartRate = requestDTO.heartRate;
                signs.Notes = requestDTO.Notes;

                await unitOfWork.vitaSignsRepository.UpdateVitalSigns(signs);
                await unitOfWork.SaveAsync();

                return Ok(new
                {
                    message = "Vital-signs updated successfully"
                });
            }
            return Unauthorized();
        }

        [HttpDelete("{signsId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteVitalSigns(Guid signsId)
        {
            var signs = await unitOfWork.vitaSignsRepository.GetVitalSigns(signsId);
            if (signs == null)
                return NotFound();
            await unitOfWork.vitaSignsRepository.SoftDelete(signs);
            await unitOfWork.vitaSignsRepository.UpdateVitalSigns(signs);
            await unitOfWork.SaveAsync();

            return Ok(new
            {
                message = "Vaital signs deleted successfully"
            });
        }
    }
}