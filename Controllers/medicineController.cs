using HealthSync.Model;
using HealthSync.PrescptionsDTO.MedicineDTO;
using Hospital_API.DTO;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace HealthSync.Controllers
{
    [ApiController]
    [Route("api/medicines")]
    public class medicineController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public medicineController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost]
        [ProducesResponseType(typeof(MedicineResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MedicineResponse>> CreateMedicine(CreateMedicineDTO medicineDTO)
        {
            //. manual mapping 
            var medicine = new Medicine
            {
                DefaultDosage = medicineDTO.DefaultDosage,
                CreatedAt = DateTime.UtcNow,
                Form = medicineDTO.Form,
                IsActive = true, //. as default
                MedicineName = medicineDTO.MedicineName,
                Strength = medicineDTO.Strength,
                ScientificName = medicineDTO.ScientificName,
            };

            await unitOfWork.medicineService.Create(medicine);
            await unitOfWork.SaveAsync();

            return Ok(new MedicineResponse
            {
                medicineId = medicine.Id.ToString(),
                medicineName = medicine.MedicineName,
                message = "Medicine created successfully"
            });
        }

        [HttpPut]
        [ProducesResponseType(typeof(UpdateMedicineRespsonseDTO), StatusCodes.Status200OK )]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UpdateMedicineRespsonseDTO>> UpdateMedicineDetails(UpdateMedicineDTO updateMedicine)
        {
            //. manual mapping  
            Guid medicineId = Guid.Parse(updateMedicine.medicineId);
            var medicine = await unitOfWork.medicineService.Medicine(medicineId);

            medicine.Strength = updateMedicine.Strength;
            medicine.ScientificName = updateMedicine.ScientificName;
            medicine.MedicineName = updateMedicine.medicineName;
            medicine.DefaultDosage = updateMedicine.DefaultDosage;
            medicine.IsActive = updateMedicine.IsActive;
            medicine.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.medicineService.Update(medicine);
            await unitOfWork.SaveAsync();
            return Ok(new UpdateMedicineRespsonseDTO
            {
                medicineId = medicine.Id.ToString(),
                Message = "Medicine updated successfully"
            });
        }

        [HttpGet("all-medicines")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<UpdateMedicineDTO>>) ,StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<List<UpdateMedicineDTO>>>> getMedicines()
        {
            var listOfMedicines = await unitOfWork.medicineService.GetAllMedicines();
            if (!listOfMedicines.Any())
                return BadRequest();
            return Ok(new ResponseDTOForGettingAPIs<List<UpdateMedicineDTO>>
            {
                Data = listOfMedicines
            });
        }

        [HttpGet("active-medicines")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<UpdateMedicineDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Doctor,Nurse")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<List<UpdateMedicineDTO>>>> getActiveMedicines()
        {
            var listOfMedicines = await unitOfWork.medicineService.GetListOfMedicines();
            if (!listOfMedicines.Any())
                return BadRequest();
            return Ok(new ResponseDTOForGettingAPIs<List<UpdateMedicineDTO>>
            {
                Data = listOfMedicines
            });
        }

        [HttpGet("{medicineId:guid}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<GetMedicineDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Doctor,Nurse")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<GetMedicineDTO>>> GetMedicineById(Guid medicineId)
        {
            var existingMedicine = await unitOfWork.medicineService.GetMedicineWithId(medicineId);
            if (existingMedicine == null)
                return BadRequest();
            return Ok(new ResponseDTOForGettingAPIs<GetMedicineDTO>
            {
                Data = existingMedicine
            });
        }

        [HttpPatch("{medicineId}/inactive")]
        [ProducesResponseType(typeof(UpdateMedicineRespsonseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UpdateMedicineRespsonseDTO>> SoftDelete(Guid medicineId)
        {
            var medicine = await unitOfWork.medicineService.Medicine(medicineId);
            if (medicine == null)
                return BadRequest();

            await unitOfWork.medicineService.SoftDelete(medicine);
            await unitOfWork.medicineService.Update(medicine);

            await unitOfWork.SaveAsync();

            return Ok(new UpdateMedicineRespsonseDTO
            {
                medicineId = medicine.Id.ToString(),
                Message = "The medication was successfully discontinued."
            });
        }

        [HttpPut("{medicineId}/reactive")]
        [ProducesResponseType(typeof(UpdateMedicineRespsonseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UpdateMedicineRespsonseDTO>> ReActivateMedicine(Guid medicineId)
        {
            var medicine = await unitOfWork.medicineService.GetMedicineWithTracking(medicineId);
            if (medicine == null)
                return BadRequest();

            await unitOfWork.medicineService.ReactiveMedicine(medicine);
            await unitOfWork.medicineService.Update(medicine);
            await unitOfWork.SaveAsync();

            return Ok(new UpdateMedicineRespsonseDTO
            {
                medicineId = medicineId.ToString(),
                Message = "The medication has been reactivated"
            });
        }
    }
}
