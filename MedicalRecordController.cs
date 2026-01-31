using Azure.Core;
using HealthSync.MedialRecordDTO;
using Hospital_API.DTO;
using Hospital_API.Model;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace Hospital_API.Controllers
{
    [ApiController]
    [Route("api/medical-record")]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public MedicalRecordController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        [ProducesResponseType(typeof(MedicalRecordResponseDTO) , StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Doctor")]
        public async Task<ActionResult<MedicalRecordResponseDTO>> Create(MedicalRecordRequestDTO requestDTO)
        {
            //. manual mapping 
            var record = new MedicalRecord
            {
                Diagnosis = requestDTO.diagnosis,
                DoctorId = requestDTO.DoctorId,
                CreatedAt = DateTime.UtcNow,
                PatientFileId = requestDTO.patientFileId,
                Treatment = requestDTO.Treatment,
            };

            await unitOfWork.medicalRecord.Create(record);
            await unitOfWork.SaveAsync();

            return Ok(new MedicalRecordResponseDTO
            {
                Id = record.Id.ToString(),
                patientFileId = record.PatientFileId.ToString(),
                DoctorId = record.DoctorId.ToString(),
                Diagnosis = record.Diagnosis,
                Treatment = record.Treatment,
                CreatedAt = DateTime.UtcNow
            });
        }

        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(MedicalRecordResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Doctor")]
        public async Task<ActionResult<MedicalRecordResponseDTO>> GetMedicalRecord(Guid Id)
        {
            var medical = await unitOfWork.medicalRecord.GetMedicalRecord(Id);
            if (medical == null)
                return BadRequest(new BadRequesDTO
                {
                    Success = false,
                    Message = "This record is not found"
                });

            return Ok(medical);
        }

        [HttpGet("{patientFileId}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<MedicalRecordByPatientFileDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Doctor")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<List<MedicalRecordByPatientFileDTO>>>>
            medicalRecordByPatientFileId(Guid patientFileId)
        {
            var records = await unitOfWork.medicalRecord.GetMedicalRecordUsingPatientFileId(patientFileId);
            if (!records.Any())
                return BadRequest();
            return Ok(new ResponseDTOForGettingAPIs<List<MedicalRecordByPatientFileDTO>>
            {
                Data = records
            });
        }

        [HttpPut("edit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Doctor")]
        public async Task<ActionResult> UpdateMedicalRecord(Guid Id, UpdateMedicalRecordDTO recordDTO)
        {
            var record = await unitOfWork.medicalRecord.GetMedicalRecordById(Id);
            if (record == null)
                return BadRequest();
            record.Diagnosis = recordDTO.diagnosis;
            record.Treatment = recordDTO.treatment;

            await unitOfWork.medicalRecord.Update(record);
            await unitOfWork.SaveAsync();
            return Ok(new
            {
                message = "Medical record updated successfully"
            });
        }

        [HttpPatch("{Id}/close")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Doctor")]
        public async Task<ActionResult> CloseMedicalRecord(Guid Id)
        {
            var record = await unitOfWork.medicalRecord.GetMedicalRecordById(Id);
            if (record == null)
                return BadRequest();

            record.IsClosed = true;

            await unitOfWork.medicalRecord.Update(record);
            await unitOfWork.SaveAsync();
            return Ok(new
            {
                message = "Medical record closed successfully"
            });
        }


        [HttpPatch("{Id:guid}/reopen")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Doctor")]
        public async Task<ActionResult> reopenMedicalRecord(Guid Id)
        {
            var record = await unitOfWork.medicalRecord.GetMedicalRecordById(Id);
            if (record == null)
                return BadRequest();

            record.IsClosed = false;

            await unitOfWork.medicalRecord.Update(record);
            await unitOfWork.SaveAsync();
            return Ok(new
            {
                message = "Medical record reopened successfully"
            });
        }


        [HttpPatch("{Id:guid}/archive")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Doctor")]
        public async Task<ActionResult> archiveMedicalRecord(Guid Id)
        {
            var record = await unitOfWork.medicalRecord.GetMedicalRecordById(Id);
            if (record == null)
                return BadRequest();

            record.IsArchieved = true;

            await unitOfWork.medicalRecord.Update(record);
            await unitOfWork.SaveAsync();
            return Ok(new
            {
                message = "Medical record archived"
            });
        }


        [HttpPatch("{Id}/restore")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Doctor")]
        public async Task<ActionResult> restoreMedicalRecord(Guid Id)
        {
            var record = await unitOfWork.medicalRecord.GetMedicalRecordById(Id);
            if (record == null)
                return BadRequest();

            record.IsArchieved = false;

            await unitOfWork.medicalRecord.Update(record);
            await unitOfWork.SaveAsync();
            return Ok(new
            {
                message = "Medical record restored successfully"
            });
        }
    }
}
