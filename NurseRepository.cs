using Hospital_API.ApplicationDbContext;
using Hospital_API.DTO;
using Hospital_API.DTOs;
using Hospital_API.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Net.WebSockets;

namespace Hospital_API.Services
{
    public class NurseRepository : INurseRepository 
    {
        private readonly AppDbContext dbContext;

        public NurseRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //. also here will take the current id from token 
        public async Task<ResponseDTOForGettingAPIs<List<NurseWithAppoinmentDTO>>> NurseAppoinments(Guid nurseId)
        { 
            var appoinment = await dbContext.appoinments.Where(op => op.Id == nurseId)
                .Select(op => new NurseWithAppoinmentDTO
                {
                    AppoinmentId = op.Id,
                    PatientName = op.Patient.User.FullName,
                    DoctorName = op.Doctor.User.FullName,
                    StartAt = op.StartAt,
                    EndAt = op.EndAt,
                    Status = op.Status.ToString ()
                }).ToListAsync();
            return new ResponseDTOForGettingAPIs<List<NurseWithAppoinmentDTO>>
            {
                Data = appoinment
            };
        }

        public async Task<ResponseDTOForGettingAPIs<DisplayAppoinmentFromNurseTableDTO>> GetSpecificAppoinment(Guid appoinmentId)
        {
            var appoinment = await dbContext.appoinments.AsNoTracking().Where(op => op.Id == appoinmentId).Select(op => new DisplayAppoinmentFromNurseTableDTO
            {
                appoinmentId = op.Id,
                PatientName = op.Patient.User.FullName,
                DoctorName = op.Doctor.User.FullName,
                AppoinmentDate = op.AppointmentDate,
                StartTime = op.StartAt,
                EndTime = op.EndAt,
                reason = op.reason,
                Status = op.Status.ToString(),
                IsPrepared = op.IsPrepared
            }).FirstOrDefaultAsync();

            if (appoinment == null)
                throw new Exception("Appoinment is not found"); //. will be handeled using the global middleware 

            return new ResponseDTOForGettingAPIs<DisplayAppoinmentFromNurseTableDTO>
            {
                Data= appoinment
            };
        }

        //. this service for only Getting Details for specific nurse without any operation of CUD  
        //. here will take the current userId from the JWT token 
        public async Task<ResponseDTOForGettingAPIs<NurseDTO>> GetNurseProfile(Guid NurseId)
        {
            var nurse = await dbContext.nurses.AsNoTracking().Where(op => op.Id == NurseId).Select(op => new NurseDTO
            {
                NurseId = op.Id,
                FullName = op.User.FullName,
                Email = op.User.Email,
                Department = op.Department,
                PhoneNumber = op.User.PhoneNumber
            }).FirstOrDefaultAsync();

            return new ResponseDTOForGettingAPIs<NurseDTO>
            {
                Data = nurse
            };
        }

        public async Task<Result> makingSpecificAppoinmentPrepare(Guid appoinmentId)
        {
            var appoinment = await dbContext.appoinments.FirstOrDefaultAsync(op => op.Id == appoinmentId);
            if (appoinment == null)
                return new Result().Failure("Invalid op id");
            appoinment.IsPrepared = true;

            dbContext.Update(appoinment);
            return new Result().Success;
        }

        public async Task<ResponseDTOForGettingAPIs<List<WaitingRoomDTO>>> WaitingRoomService(Guid nurseId)
        {
            var nurseDepartment = await dbContext.nurses.Where(op => op.Id == nurseId).Select(op => op.Department).FirstOrDefaultAsync();
            if (nurseDepartment == null)
                throw new Exception("Something invalid occured");
            //.should be in the appoinment repository 
            var appoinments = await dbContext.appoinments
                .Where(op => op.Doctor.Department == nurseDepartment && op.IsCancelled != true && op.Status == AppoinmentStatus.Scheduled && op.IsPrepared == true).Select(op => new WaitingRoomDTO
                {
                    appoinmentId = op.Id,
                    DoctorName = op.Doctor.User.FullName,
                    PatientName = op.Patient.User.FullName,
                    startTime = op.StartAt,
                    Status = op.Status.ToString(),
                    IsPrepared = true
                }).ToListAsync();

            return new ResponseDTOForGettingAPIs<List<WaitingRoomDTO>>
            {
                Data = appoinments
            };
        }
    }
}
