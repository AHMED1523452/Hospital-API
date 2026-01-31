using HealthSync.MedialRecordDTO;
using Hospital_API.ApplicationDbContext;
using Hospital_API.DTO;
using Hospital_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Services
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly AppDbContext dbContext;

        public MedicalRecordRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Create(MedicalRecord record)
        {
            await dbContext.records.AddAsync(record);
        }

        public async Task<MedicalRecord> GetMedicalRecordById(Guid medicalRecordID)
        {
            var record = await dbContext.records.FirstOrDefaultAsync(op => op.Id == medicalRecordID && op.IsClosed == false);
            return record;
        }

        public async Task<MedicalRecordResponseDTO> GetMedicalRecord(Guid Id)
        {
            var record = await dbContext.records.AsNoTracking().Where(op => op.Id == Id && op.IsArchieved == false).Select(op => new MedicalRecordResponseDTO
            {
                Id = op.Id.ToString(),
                DoctorId = op.DoctorId.ToString(),
                patientFileId = op.PatientFile.ToString(),
                Diagnosis = op.Diagnosis,
                Treatment = op.Treatment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = op.UpdatedAt,
                IsArchieved = op.IsArchieved,
                IsClosed = op.IsClosed
            }).FirstOrDefaultAsync();

            return record;
        }

        public async Task<List<MedicalRecordByPatientFileDTO>> GetMedicalRecordUsingPatientFileId(Guid patientFileId)
        {
            var medicalRecord = await dbContext.records.AsNoTracking().Where(op => op.PatientFileId == patientFileId && op.IsArchieved == false).Select(op => new MedicalRecordByPatientFileDTO
            {
                recordId = op.Id.ToString(),
                diagnosis = op.Diagnosis,
                isClosed = op.IsClosed,
                CreatedAt = op.CreatedAt
            }).ToListAsync();

            return medicalRecord;
        }

        public async Task Update(MedicalRecord record)
        {
            dbContext.records.Update(record);
        }
    }
}
