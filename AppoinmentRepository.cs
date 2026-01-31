using Hospital_API.ApplicationDbContext;
using Hospital_API.DTO;
using Hospital_API.DTOs;
using Hospital_API.Enums;
using Hospital_API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hospital_API.Services
{
    public class AppoinmentRepository : IAppoinmentRepository
    {
        public AppDbContext dbContext { get; }

        public AppoinmentRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Result> Create(Appoinment appoinment)
        {
            //. all of the fienlds will be as an input without Status 
            //. checking if there is an appoinment in the same time or not 
            //. here can not use the AsNoTracking due to exiting of creation process 
            if (await dbContext.appoinments.AnyAsync(op =>
                                op.DoctorId == appoinment.DoctorId &&
                                !op.IsCancelled &&
                                op.StartAt < appoinment.EndAt &&
                                appoinment.StartAt < op.EndAt
                            ))
            {
                if (await dbContext.doctors.AnyAsync(d => d.Id == appoinment.DoctorId) &&
                                 await dbContext.patients.AnyAsync(p => p.Id == appoinment.PatientId))
                {
                    await dbContext.appoinments.AddAsync(appoinment);
                    return new Result().Success;
                }
                else if (!await dbContext.doctors.AnyAsync(op => op.Id == appoinment.DoctorId))
                    return new Result().Failure("Doctor is not existing right now");
                else if (!await dbContext.patients.AnyAsync(op => op.Id == appoinment.PatientId))
                    return new Result().Failure("Patient didn't come yet!!");
            }
            return new Result().Failure("There is an appoinment in the same time");
        }

        //. this service without any CRUD operation
        public async Task<Appoinment> GetById(string Id)
        {
            if (Guid.TryParse(Id, out Guid result))
            {
                var appoinment = await dbContext.appoinments.AsNoTracking().FirstOrDefaultAsync(op => op.Id == result);
                return appoinment;
            }
            throw new Exception("Invalid Id");
        }

        public async Task<List<AppoinmentResponseDTO>> GetSpecificAppoinmentsForSpecificPatientAsync(Guid PatientID)
        {
            var Appoinments = await dbContext.appoinments.Where(op => op.PatientId == PatientID).Select(op => new AppoinmentResponseDTO
            {
                DoctorName = op.Doctor.User.FullName,
                DoctorDepartment = op.Doctor.Department,
                StartAt = op.StartAt,
                EndAt = op.EndAt,
                AppoinmentDate = op.AppointmentDate,
                reason = op.reason,
                IsCancelled = op.IsCancelled,
                Status = op.Status
            }).ToListAsync();
            return Appoinments;
        }
        //. Get Appoinments Using DoctorID
        public async Task<List<AppoinmentResponseDTO>> GetAppoinmentsUsingDoctorId(Guid DoctorId)
        {
            var Appoinments = await dbContext.appoinments.Where(op => op.PatientId == DoctorId).Select(op => new AppoinmentResponseDTO
            {
                DoctorName = op.Doctor.User.FullName,
                DoctorDepartment = op.Doctor.Department,
                PatientName = op.Patient.User.FullName,
                StartAt = op.StartAt,
                EndAt = op.EndAt,
                AppoinmentDate = op.AppointmentDate,
                reason = op.reason,
                IsCancelled = op.IsCancelled,
                Status = op.Status
            }).ToListAsync();
            return Appoinments;
        }

        public async Task changeAppoinmentDate(Guid AppoinmentId, ChangeDate_sAppoinmentDTO changeDate_S)
        {
            Appoinment appoinment = await dbContext.appoinments.FirstOrDefaultAsync(op => op.Id == AppoinmentId);
            if (appoinment == null)
                throw new Exception("this appoinment is not exist");
            //. manual mapping 
            appoinment.AppointmentDate = changeDate_S.AppoinmentDate;
            appoinment.StartAt = changeDate_S.StartAt;
            appoinment.EndAt = changeDate_S.EndAt;

            dbContext.appoinments.Update(appoinment);
        }

        public async Task<Result> CheckExstingThisDoctorInThisAppoinment(Guid DoctorId, Guid PatientId)
        {
            var checking = await dbContext.appoinments.
                AnyAsync(op => op.DoctorId == DoctorId || op.PatientId == PatientId);
            if (!checking)
                return new Result().Failure("Something invalid occured");
            return new Result().Success;
        }

        //. this service for bringing the appoinment with any process of CRUD operation
        public async Task<AppoinmentResponseDTO> GetAppoinmentById(Guid? Id)
        {
            //. we can use the pagination here but we don't alot of data or raws 
            var appoinment = await dbContext.appoinments.Where(op => op.Id == Id).Select(op => new AppoinmentResponseDTO
            {
                Id = Id.ToString(),
                DoctorName = op.Doctor.User.FullName,
                DoctorDepartment = op.Doctor.Department,
                PatientName = op.Patient.User.FullName,
                StartAt = op.StartAt,
                EndAt = op.EndAt,
                AppoinmentDate = op.AppointmentDate,
                reason = op.reason,
                IsCancelled = op.IsCancelled,
                Status = op.Status
            }).Take(50).FirstOrDefaultAsync();
            return appoinment;
        }

        public async Task<Appoinment> GetApById(Guid Id)
        {
            var appoinment = await dbContext.appoinments.FirstOrDefaultAsync(op => op.Id == Id);
            return appoinment;
        }

        public async Task<List<AppoinmentResponseDTO>> Appoinments()
        {
            var appoinments = await dbContext.appoinments.AsNoTracking()
                .Where(op => op.IsCancelled != true || op.Status != AppoinmentStatus.cancelled).Select(op => new AppoinmentResponseDTO
                {
                    Id = op.Id.ToString(),
                    DoctorName = op.Doctor.User.FullName,
                    DoctorDepartment= op.Doctor.Department,
                    PatientName = op.Patient.User.FullName,
                    AppoinmentDate =op.AppointmentDate ,
                    StartAt = op.StartAt,
                    EndAt = op.EndAt,
                    reason = op.reason,
                    Status = op.Status,
                    IsCancelled= op.IsCancelled
                }).Take(50).ToListAsync();
            return appoinments;
        }

        public async Task<ResponseDTO<ChangeStatusForSpecificAppoinmentDTO>> ChangeStatusForSpecificAppoinment(ChangeAppoinmentStatusDTO appoinmentDTO)
        {
                var appoinment = await dbContext.appoinments.FirstOrDefaultAsync(op => op.Id == appoinmentDTO.Id);
                if (appoinment == null)
                    throw new Exception("This appoinment is not exist");
                var changeResponse = new ChangeStatusForSpecificAppoinmentDTO
                {
                    Id = appoinment.Id,
                    UpdatedAt = DateTime.UtcNow,
                    oldStatus= appoinment.Status
                };

                appoinment.Status = appoinmentDTO.NewStatus;
                dbContext.appoinments.Update(appoinment);

                changeResponse.NewStatus = appoinmentDTO.NewStatus;
                return new ResponseDTO<ChangeStatusForSpecificAppoinmentDTO>
                {
                    Success = false ,
                    Message = "Status has been changed successfully",
                    Data = changeResponse
                };
        }

        public async Task Update(Appoinment appoinment)
        {
            var result = dbContext.appoinments.Update(appoinment);
        }

        public async Task SoftRemove(Appoinment appoinment)
        {
            appoinment.IsCancelled = true;
            appoinment.Status = AppoinmentStatus.cancelled;

            dbContext.appoinments.Update(appoinment);
        }

        public async Task Remove(Appoinment appoinment)
        {
            dbContext.appoinments.Remove(appoinment);
        }

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
