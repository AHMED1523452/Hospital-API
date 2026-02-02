using Hospital_API.DTO;
using Hospital_API.DTOs;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("nurses/{nurseId:guid}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<NurseDTO>), StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<NurseDTO>>> getNurse(Guid nurseId)
        {
            return Ok(await unitOfWork.adminRepository.GetNurse(nurseId));
        }

    }
}
