using Hospital_API.DTO;
using Hospital_API.DTOs;

namespace Hospital_API.Services
{
    public interface IAdminRepository
    {
        Task<ResponseDTOForGettingAPIs<NurseDTO>> GetNurse(Guid nurseId);
    }
}
