using Hospital_API.DTO;
using Hospital_API.DTOs;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace Hospital_API.Controllers
{
    [ApiController]
    [Route("api/nurses")]
    public class NurseController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public NurseController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<DisplayAppoinmentFromNurseTableDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Nurse")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<DisplayAppoinmentFromNurseTableDTO>>> GetSpecificAppoinment()
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if(Guid.TryParse(userId, out Guid result))
            {
                return BadRequest();
            }

            return Ok(await unitOfWork.nurseRepository.GetSpecificAppoinment(result));
        }

        [HttpGet("appoinments")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<DisplayAppoinmentFromNurseTableDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Nurse")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<List<DisplayAppoinmentFromNurseTableDTO>>>> GetAppoinments()
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out Guid result))
            {
                return Unauthorized();
            }
            return Ok(await unitOfWork.nurseRepository.NurseAppoinments(result));
        }

        [HttpGet("appoinments/{appoinmentId:guid}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<DisplayAppoinmentFromNurseTableDTO>>), StatusCodes.Status200OK)]
        [Authorize(Roles = "Nurse")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<List<DisplayAppoinmentFromNurseTableDTO>>>> GetAppoinment(Guid appoinmentId)
        {
            return Ok(await unitOfWork.nurseRepository.GetSpecificAppoinment(appoinmentId));
        }

        [HttpGet("waiting-room")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<List<WaitingRoomDTO>>), StatusCodes.Status200OK)]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<List<WaitingRoomDTO>>>> WaitingRoom()
        {
            string nurseId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!Guid.TryParse(nurseId, out Guid result))
            {
                return BadRequest("Something wrong occured");
            }

            return Ok(await unitOfWork.nurseRepository.WaitingRoomService(result));
        }

        [HttpPatch("appoinments/{appoinmentId:guid}/prepare")]
        [ProducesResponseType(typeof(Result) , StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Authorize(Roles = "Nurse")]
        public async Task<ActionResult<Result>> makingAppoinmentIsPrepare(Guid appoinmentId)
        {
            var result = await unitOfWork.nurseRepository.makingSpecificAppoinmentPrepare(appoinmentId);
            if (!result.IsSuccess)
                return Unauthorized();
            return Ok(new
            {
                message = $"appoinment with id : {appoinmentId} is prepare"
            });
        }
    }
}
