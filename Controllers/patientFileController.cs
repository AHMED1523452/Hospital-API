using HealthSync.DTOs;
using Hospital_API.DTO;
using Hospital_API.Model;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace HealthSync.Controllers
{
    [ApiController]
    [Route("api/patient-file")]
    public class patientFileController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment env;

        public patientFileController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            this.env = env;
        }

        [HttpPost("patient-file")]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Nurse,Doctor")]
        public async Task<ActionResult> UploadPatientFile([FromForm] patientFileRequestDTO requestDTO)
        {
            var result = await unitOfWork.patientFileRepository.UploadFile(requestDTO, env);
            if (!result.IsSuccess)
                return BadRequest("couldn't upload this file");

            await unitOfWork.SaveAsync();

            return Ok(new
            {
                Message = "File has uploaded succesfully"
            });
        }

        [HttpGet("{fileId}/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest )]
        [Authorize(Roles = "Admin,Doctor,Nurse,Patient")]
        public async Task<IActionResult> DownloadFile(Guid fileId)
        {
            var file = await unitOfWork.patientFileRepository.GetPatientFileWithId(fileId);
            if (file == null)
                return NotFound();
            string patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(patientId, out Guid result))
            {
                //. reading file content 
                var bytes = await System.IO.File.ReadAllBytesAsync(file.FilePath);
                var contentType = "application/octet-stream";

                //. checking if the file is exist or not on the device
                if (!System.IO.File.Exists(file.FilePath))
                    return BadRequest();

                if (User.IsInRole("Patient"))
                {
                    if (result != file.PatientId)
                        return Unauthorized();
                }
                //. the response will be considered as File 
                return File(bytes, contentType, file.FileName);
            }
            return Unauthorized();
        }


        [HttpGet("patients/{patientId}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<ListOfPatientFileDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Admin,Nurse,Patient")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<List<ListOfPatientFileDTO>>>> listOfpatientFile(Guid patientId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out Guid result))
            {
                if (User.IsInRole("Patient"))
                {
                    if (patientId != result)
                        return Unauthorized();
                }
                return Ok(new ResponseDTOForGettingAPIs<List<ListOfPatientFileDTO>>
                {
                    Data = await unitOfWork.patientFileRepository.GetListOfPatientFileDependingOfPatientId(patientId)
                });
            }
            return Unauthorized();
        }


        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<ListOfPatientFileDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Admin,Nurse,Patient")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<ListOfPatientFileDTO>>> PatientFileById(Guid Id)
        {
            string patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(patientId, out Guid result))
            {
                if (User.IsInRole("Patient"))
                {
                    if (Id != result)
                        return Unauthorized();
                }
                return Ok(new ResponseDTOForGettingAPIs<ListOfPatientFileDTO>
                {
                    Data = await unitOfWork.patientFileRepository.GetpatientFileById(Id)
                });
            }
            return Unauthorized();
        }


        [HttpDelete("{fileId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult> RemovePatientFile(Guid fileId)
        {
            var patientFile = await unitOfWork.patientFileRepository.GetPatientFileWithId(fileId);
            if (patientFile == null)
                return BadRequest();

            if (User.IsInRole("Doctor"))
            {
                Guid DoctorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (await unitOfWork.patientFileRepository.HasActiveAppointmentForDoctor(fileId, DoctorId))
                    return Unauthorized();
            }
            //. for the admin role (can delete any patient file ) 
            await unitOfWork.patientFileRepository.Delete(patientFile);
            await unitOfWork.SaveAsync();
            return Ok(new
            {
                message = "File has been removed"
            });
        }
    }
}
