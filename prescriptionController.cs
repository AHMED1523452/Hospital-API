using HealthSync.DTOs;
using HealthSync.Model;
using HealthSync.PrescptionsDTO;
using HealthSync.PrescptionsDTO.MedicineDTO;
using Hospital_API.DTO;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace Hospital_API.Controllers
{
    [ApiController]
    [Route("api/prescreptions")]
    public class prescriptionController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public prescriptionController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> Create(PresceptionRequest request)
        {
            var result = await unitOfWork.presceptionRepository.Create(request);
            if (!result.IsSuccess)
                return Unauthorized();

            await unitOfWork.SaveAsync();

            return Ok(new  
            {
                message = "prescription created successfully"
            });
        }

        [HttpGet("{presceptionId}")]
        [ProducesResponseType(typeof(GetPresceptionDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Patient,Doctor")]
        public async Task<ActionResult<GetPresceptionDetailsDTO>> GetDetails(Guid presceptionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out Guid result))
            {
                if (User.IsInRole("Patient"))
                {
                    return Ok(await unitOfWork.presceptionRepository.GetPrescreptionWithPatientId(presceptionId, result));
                }
                else if (User.IsInRole("Doctor"))
                {
                    return Ok(await unitOfWork.presceptionRepository.GetPresceptionWithDoctorId(presceptionId, result));
                }
                //. for the admin role 
                return Ok(await unitOfWork.presceptionRepository.GetPresceotionWithOutTracking(presceptionId));
            }
            return Unauthorized();
        }


        [HttpGet("{patientId}/prescreptions")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<GetListOfPresceptionDependOnPatientIdDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<GetListOfPresceptionDependOnPatientIdDTO>>> GetListOfPresceptions(Guid patientId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out Guid result))
            {
                if (User.IsInRole("Doctor"))
                {
                    var check = await unitOfWork.presceptionRepository.HasAnAppoinment(patientId, result);
                    if (!check.IsSuccess)
                        return Unauthorized();

                    var listForDoctorUserRole = await unitOfWork.presceptionRepository.GetListOfPresceptionWithDoctorName(patientId, result);

                    if (!listForDoctorUserRole.Any())
                        return Unauthorized();

                    return Ok(new ResponseDTOForGettingAPIs<List<GetListOfPresceptionDependOnPatientIdDTO>>
                    {
                        Data = listForDoctorUserRole
                    });
                }

                else if (User.IsInRole("Patient"))
                {
                    if (result != patientId)
                        return Unauthorized();
                    var istForPatientRole = await unitOfWork.presceptionRepository.GetListOfPresception(patientId);
                    if (!istForPatientRole.Any())
                        return Unauthorized();

                    return Ok(new ResponseDTOForGettingAPIs<List<GetListOfPresceptionDependOnPatientIdDTO>>
                    {
                        Data = istForPatientRole
                    });
                }
                //. Admin
                var list = await unitOfWork.presceptionRepository.GetListOfPresception(patientId);
                if (!list.Any())
                    return Unauthorized();

                return Ok(new ResponseDTOForGettingAPIs<List<GetListOfPresceptionDependOnPatientIdDTO>>
                {
                    Data = list
                });
            }
            return Unauthorized();
        }

        [HttpPut("{presceptionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> Edit(Guid presceptionId , UpdatePresceptionDTO dTO)
        {
            var presception = await unitOfWork.presceptionRepository.GetPresception(presceptionId);
            if (presception == null)
                return BadRequest();
            presception.IsActive = dTO.IsActive;
            presception.Notes = dTO.Notes;
            presception.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.presceptionRepository.Update(presception);
            await unitOfWork.SaveAsync();

            return Ok(new
            {
                UpdatedAt = DateTime.UtcNow
            }); 
        }

        [HttpPost("{id}/medicines")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> AddMedicineToExistingPresception(Guid id, PresceptionMedicineDTO medicineDTO)
        {
            var result = await unitOfWork.presceptionRepository.AddMedicineToExistingPresception(id, medicineDTO);

            if (!result.IsSuccess)
                return Unauthorized();

            await unitOfWork.SaveAsync();

            return Ok(new
            {
                CreatedAt = DateTime.UtcNow
            });
        }

        [HttpPut("{id:guid}/medicines/{medicineId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> UpdateSpecificPresceptionMedicine(Guid id,Guid medicineId,[FromBody] UpdateMedicineInPresceptionDTO dTO)
        {
            var result = await unitOfWork.presceptionRepository.UpdateMedicineInsidePresception(id, medicineId, dTO);

            if (!result.IsSuccess)
                return Unauthorized();

            await unitOfWork.SaveAsync();

            return Ok(new
            {
                UpdatedAt = DateTime.UtcNow
            });
        }

        [HttpDelete("{id:guid}/medicines/{medicineId:guid}/inactive")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> InActivateMedicine(Guid id, Guid medincineId)
        {
            var result = await unitOfWork.presceptionRepository.SoftDeletePresceptionMedicine(id, medincineId);

            if (!result.IsSuccess)
                return Unauthorized();

            await unitOfWork.SaveAsync();

            return Ok(new
            {
                DeletedAt = DateTime.UtcNow
            });
        }

        [HttpDelete("{id:guid}/medicines/{medicineId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> HardDelete(Guid id, Guid medincineId)
        {
            var result = await unitOfWork.presceptionRepository.HardDeletepresceptionMedicine(id, medincineId);

            if (!result.IsSuccess)
                return Unauthorized();

            await unitOfWork.SaveAsync();

            return Ok(new
            {
                DeletedAt = DateTime.UtcNow
            });
        }
    }
}
