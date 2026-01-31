using Microsoft.Identity.Client;

namespace Hospital_API.Application.PrescptionsDTO
{
    public class GetListOfPresceptionDependOnPatientIdDTO
    {
        public Guid presceptionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DoctorName { get; set; }
        public int MedicinesCount { get; set; }
    }
}
