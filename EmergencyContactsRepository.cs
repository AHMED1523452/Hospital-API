using HealthSync.DTOs;
using HealthSync.Model;
using Hospital_API.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace HealthSync.Services
{
    public class EmergencyContactsRepository : IEmergencyContactsRepository
    {
        private readonly AppDbContext dbCotnext;

        public EmergencyContactsRepository(AppDbContext dbCotnext)
        {
            this.dbCotnext = dbCotnext;
        }

        public async Task Create(EmergencyContact emergencyContact)
        {
            await dbCotnext.emergencyContacts.AddAsync(emergencyContact);
        }

        public async Task Update(EmergencyContact emergencyContact)
        {
            dbCotnext.Update(emergencyContact);
        }

        public async Task Delete(EmergencyContact emergencyContact)
        {
            dbCotnext.emergencyContacts.Remove(emergencyContact);
        }

        public async Task<List<GetEmergencyContactDTO>> GetListOfEmergencyContactsWithPatientId(Guid patientId)
        {
            var EmergencyContactacts = await dbCotnext.emergencyContacts.AsNoTracking().Where(op => op.PatientId == patientId).Select(op => new GetEmergencyContactDTO
            {
                FullName = op.Name,
                PhoneNumber = op.PhoneNumber,
                Address = op.Address,
                AlternatePhone = op.AlternatePhone,
                IsPrimary = op.IsPrimary,
                Relation = op.Relation
            }).ToListAsync();

            return EmergencyContactacts;
        }

        public async Task<EmergencyContact> GetEmergencyContact(Guid emergencyContactId)
        {
            var emergencyContact = await dbCotnext.emergencyContacts.FirstOrDefaultAsync(op => op.Id == emergencyContactId);
            return emergencyContact;
        }

        public async Task<List<EmergencyContact>> GetEmergencyContact()
        {
            return await dbCotnext.emergencyContacts.AsNoTracking().ToListAsync();
        }
    }
}
