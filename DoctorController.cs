using HealthSync.DTOs;
using Hospital_API.DTO;
using Hospital_API.Model;
using Hospital_API.Services;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace Hospital_API.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IRegisterationProcessesRepository registerationProcesses;
        private readonly IUserService userService;

        public DoctorController(IDoctorRepository doctorRepository,
            IRegisterationProcessesRepository registerationProcesses
            ,IUserService userService)
        {
            this.doctorRepository = doctorRepository;
            this.registerationProcesses = registerationProcesses;
            this.userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<DoctorDTO>>), StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,ApplicationUser")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<List<DoctorDTO>>>> GetDoctors()
        {
            var users = await doctorRepository.Doctors();

            var Reponse = new ResponseDTOForGettingAPIs<List<DoctorDTO>>
            {
                Data = users
            };
            return Ok(Reponse);
        }

        [HttpGet("{doctorId}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<DoctorDTO>) , StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Nurse,ApplicationUser")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<DoctorDTO>>> GetDoctorById(string doctorId)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!Guid.TryParse(doctorId, out Guid guid))
                return BadRequest(new BadRequesDTO
                {
                    Success = false,
                    Message = "Invalid Id"
                });
            var doctor = await doctorRepository.GetDoctorById(doctorId);
            if (doctor == null)
                return NotFound();
            return Ok(doctor);
        }

        [HttpDelete("{Email:required}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDoctor(string Email)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var doctor = await doctorRepository.GetDoctorByEmail(Email);
            if (doctor == null) 
                return NotFound("Invalid Email Or NationalId");

            await doctorRepository.Delete(doctor);
            await doctorRepository.SaveAsync(); 

            await userService.RemoveUser(await userService.GetUserByEmail(Email));
            return Ok(new
            {
                Success = true,
                Message = "User removed successfully"
            });
        }

        [HttpPut ("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Doctor")]
        public async Task<IActionResult> DoctorUpdateHimSelf(DoctorUpdateDTO updateDTO)
        {
            string UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserID == null)
                return Unauthorized(); //. the userID is not exis meaning that the doctor not logged in and unAuthorized
            var result = await doctorRepository.CheckCurrentRoleForDoctorUpdate(UserID);
            if (result.IsSuccess)
            {
                var user = await userService.GetUserByID(UserID);

                //. Updating for the User table only
                user.FullName = updateDTO.FullName;

                await userService.Update(user);

                return Ok(new
                {
                    Success = true,
                    Message = "Doctor updated successfully"
                });
            }
            return Unauthorized();
        } 

        [HttpPut("{doctorId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles= "Admin")]
        public async Task<IActionResult> Edit(string doctorId,[FromForm] AdminUpdateDoctorDataDTO doctorDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!Guid.TryParse(doctorId, out Guid guid))
                return BadRequest(new BadRequesDTO
                {
                    Success = false,
                    Message = "Invalid Id"
                });
            var doctor = await doctorRepository.GetDoctorById(doctorId);
            if (doctor == null)
                return NotFound();

            doctor.Specialty = doctorDTO.Specialty;
            await doctorRepository.Update(doctor);
            await doctorRepository.SaveAsync();

            await userService.Update(await userService.GetUserByID(doctor.UserId));
            return Ok(new
            {
                success = true,
                Message = "ApplicationUser updated successfully"
            });
        }
    }
}
