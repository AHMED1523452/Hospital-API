using Hospital_API.DTO;
using Hospital_API.DTOs;

namespace Hospital_API.Services
{
    public interface INurseRepository
    {
        Task<ResponseDTOForGettingAPIs<List<NurseWithAppoinmentDTO>>> NurseAppoinments(Guid appoinmentId);
        Task<ResponseDTOForGettingAPIs<DisplayAppoinmentFromNurseTableDTO>> GetSpecificAppoinment(Guid appoinmentId);
        Task<ResponseDTOForGettingAPIs<NurseDTO>> GetNurseProfile(Guid NurseId);
        Task<Result> makingSpecificAppoinmentPrepare(Guid appoinmentId);
        Task<ResponseDTOForGettingAPIs<List<WaitingRoomDTO>>> WaitingRoomService(Guid nurseId);
    }
}
