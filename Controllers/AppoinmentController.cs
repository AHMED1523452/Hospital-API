using Hospital_API.DTO;
using Hospital_API.DTOs;
using Hospital_API.Model;
using Hospital_API.Services;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Transactions;

namespace Hospital_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppoinmentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public AppoinmentController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ProducesResponseType(typeof(AppoinmentResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Authorize(Roles = "")]
        public async Task<ActionResult<List<AppoinmentResponseDTO>>> GetAppoinments()
        {
            var listofResponse = await unitOfWork.appoinmentRepository.Appoinments();
            if (!listofResponse.Any())
                return NotFound();
            return Ok(listofResponse);
        }

        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<AppoinmentResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<AppoinmentResponseDTO>?>> GetAppoinment(Guid Id)
        {
            if (Id == null) return BadRequest("Invalid Id");
            Appoinment appoinment = await unitOfWork.appoinmentRepository.GetApById(Id);
            var test = await unitOfWork.appoinmentRepository.CheckExstingThisDoctorInThisAppoinment(appoinment.DoctorId, appoinment.PatientId);
            if (appoinment == null)
                return BadRequest();

            if (!test.IsSuccess)
                return Unauthorized();

            var response = new ResponseDTOForGettingAPIs<AppoinmentResponseDTO>
            {
                Data = await unitOfWork.appoinmentRepository.GetAppoinmentById(appoinment.Id)
            };

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Authorize(Roles= "")]
        public async Task<ActionResult<Result>> Create([FromForm] CreateAppoinmentDTO responseDTO)
        {
            if (!Guid.TryParse(responseDTO.DoctorId, out Guid result) || !Guid.TryParse(responseDTO.PatientId, out Guid patientGuid))
            {
                return new Result().Failure("The selected time slot is not available");
            }
            var appoinment = new Appoinment
            {
                DoctorId = result,
                PatientId = patientGuid,
                AppointmentDate = responseDTO.AppoinmentDate,
                StartAt = responseDTO.StartAt,
                EndAt = responseDTO.EndAt,
                reason = responseDTO.reason
            };
            await unitOfWork.appoinmentRepository.Create(appoinment);
            await unitOfWork.SaveAsync();
            return new Result().Success;
        }

        [HttpGet("doctor/{doctorId:guid}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<AppoinmentResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<AppoinmentResponseDTO>>> gettingAppoinmentByDoctorId(Guid doctorId)
        {
            List<AppoinmentResponseDTO> responseDTO = await unitOfWork.appoinmentRepository.GetAppoinmentsUsingDoctorId(doctorId);
            if (!responseDTO.Any())
                return NotFound("No appoinemnts");
            return Ok(new ResponseDTOForGettingAPIs<List<AppoinmentResponseDTO>>
            {
                Data = responseDTO
            });
        }

        [HttpGet("patient/{patientId:guid}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<AppoinmentResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize(Roles = "Admin,patient")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<AppoinmentResponseDTO>>> getPatientAppoinment(Guid patientId)
        {
            List<AppoinmentResponseDTO> responseDTO = await unitOfWork.appoinmentRepository.GetSpecificAppoinmentsForSpecificPatientAsync(patientId);
            if (!responseDTO.Any())
                return NotFound("No appoinemnts");
            return Ok(new ResponseDTOForGettingAPIs<List<AppoinmentResponseDTO>>
            {
                Data = responseDTO
            });
        }

        [HttpPatch("{appoinmentId:guid}/cancel")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[Authorize(Roles ="Patient,Admin,Doctor")]
        public async Task<ActionResult<Result>> SoftDelete(Guid appoinmentId)
        {
            var appoinment = await unitOfWork.appoinmentRepository.GetApById(appoinmentId);
            if (appoinment == null) return NotFound("Appoinment is not exist");
            //. checking if the current doctor and patientID will delete the own appoinment
            var test = await unitOfWork.appoinmentRepository.CheckExstingThisDoctorInThisAppoinment(appoinment.DoctorId, appoinment.PatientId);
            if (!test.IsSuccess)
                return Unauthorized();

            await unitOfWork.appoinmentRepository.SoftRemove(appoinment); //. soft delete 
            await unitOfWork.SaveAsync();
            return Ok(new
            {
                message = "Appoinment has be cancelled"
            });
        }

        [HttpPost("{appoinmentId:guid}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> RemoveAppoinment(Guid appoinmentId)
        {
            var appoinment = await unitOfWork.appoinmentRepository.GetApById(appoinmentId);
            if (appoinment == null) return new Result().Failure("Appoinment is not exist");
            var test = await unitOfWork.appoinmentRepository.CheckExstingThisDoctorInThisAppoinment(appoinment.DoctorId, appoinment.PatientId);
            if (!test.IsSuccess)
                return Unauthorized();
            await unitOfWork.appoinmentRepository.Remove(appoinment); //. soft delete 
            await unitOfWork.SaveAsync();
            return Ok(new
            {
                message = "Apoinment has been removed"
            });
        }


        [HttpPost("changeAppoinmanteDate/{appoinmentId:guid}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<ActionResult<Result>> ChangeAppoinmentDate(Guid appoinmentId,ChangeDate_sAppoinmentDTO changeDate)
        {
            var appoinment = await unitOfWork.appoinmentRepository.GetApById(appoinmentId);
            if (appoinment == null)
                return NotFound("invalid appoinment Id");
            var test = await unitOfWork.appoinmentRepository.CheckExstingThisDoctorInThisAppoinment(appoinment.DoctorId, appoinment.PatientId);
            if (!test.IsSuccess)
                return Unauthorized();
            await unitOfWork.appoinmentRepository.changeAppoinmentDate(appoinmentId, changeDate);
            await unitOfWork.SaveAsync();

            return Ok(new
            {
                message = "Appinment date has been changed"
            });
        }

        [HttpPut("ChangeStatus")]
        [ProducesResponseType(typeof(ResponseDTO<ChangeStatusForSpecificAppoinmentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Authorize(Roles = "")]
        public async Task<ActionResult<ResponseDTO<ChangeStatusForSpecificAppoinmentDTO>>> ChangeStatus([FromForm] ChangeAppoinmentStatusDTO changeStatus)
        {
            var appoinment = await unitOfWork.appoinmentRepository.GetApById(changeStatus.Id);
            if (appoinment == null)
                return BadRequest();
            return Ok(await unitOfWork.appoinmentRepository.ChangeStatusForSpecificAppoinment(changeStatus));
        }

    }
}
