using Hospital_API.DTO;
using Hospital_API.DTOs;
using Hospital_API.Enums;
using Hospital_API.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace Hospital_API.Services
{
    public interface IAppoinmentRepository
    {
        Task<Result> Create(Appoinment appoinment);
        Task<Appoinment> GetById(string Id);
        Task<List<AppoinmentResponseDTO>> GetSpecificAppoinmentsForSpecificPatientAsync(Guid PatientID);
        Task<List<AppoinmentResponseDTO>> GetAppoinmentsUsingDoctorId(Guid DoctorId);
        Task<Result> CheckExstingThisDoctorInThisAppoinment(Guid DoctorId, Guid PatientId);
        Task changeAppoinmentDate(Guid DepartmentId, ChangeDate_sAppoinmentDTO changeDate_S);
        Task<AppoinmentResponseDTO> GetAppoinmentById(Guid? Id);

        Task<Appoinment> GetApById(Guid Id);

        Task<List<AppoinmentResponseDTO>> Appoinments();

        Task<ResponseDTO<ChangeStatusForSpecificAppoinmentDTO>> ChangeStatusForSpecificAppoinment(ChangeAppoinmentStatusDTO appoinmentDTO);

        Task Update(Appoinment appoinment);
        Task SoftRemove(Appoinment appoinment);
        Task Remove(Appoinment appoinment);
        Task SaveAsync();
    }
}
