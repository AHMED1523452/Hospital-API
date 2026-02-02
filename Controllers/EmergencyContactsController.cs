using HealthSync.DTOs;
using HealthSync.Model;
using Hospital_API.DTO;
using Hospital_API.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace HealthSync.Controllers
{ 
    [ApiController]
    [Route("api/emergency-contacts")]
    public class EmergencyContactsController : ControllerBase
    {
        private readonly UnitOfWork unitOfWork;

        public EmergencyContactsController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("patients/{PatientId}")]
        [ProducesResponseType(typeof(ResponseDTOForGettingAPIs<GetEmergencyContactDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles= "Admin,Patient")]
        public async Task<ActionResult<ResponseDTOForGettingAPIs<GetEmergencyContactDTO>>> GetListOfEmergencyContacts(Guid PatientId)
        {
            //. Checking the role from the token and the user should be logged in 
            //. For Patient role 
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(UserId, out Guid result))
            {
                if (User.IsInRole("Patient"))
                {
                    if (result != PatientId)
                        return Unauthorized();
                    return Ok(await unitOfWork.emergencyContacts.GetListOfEmergencyContactsWithPatientId(PatientId));
                }
                //. for Admin Role 
                return Ok(await unitOfWork.emergencyContacts.GetListOfEmergencyContactsWithPatientId(PatientId));
            }
            return Unauthorized();
        }

        [HttpDelete("{EC_Id}")]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<ActionResult> RemoveEmergencyContact(Guid EC_Id)
        {
            string patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(patientId, out Guid result))
            {
                if (User.IsInRole("Patient"))
                {
                    if (result != EC_Id)
                        return Unauthorized("Not allow for you to update this EC way, Roll back to Adminstrator");

                    var emergencyContacts = await unitOfWork.emergencyContacts.GetEmergencyContact(EC_Id);
                    if (emergencyContacts == null)
                        return BadRequest();

                    await unitOfWork.emergencyContacts.Delete(emergencyContacts);
                    await unitOfWork.SaveAsync();

                    return Ok(new
                    {
                        message = "This contact is removed successfully "
                    });
                }

                //. this condition has no important or another thing 
                else if (User.IsInRole("Admin"))
                {
                    var emergencyContacts = await unitOfWork.emergencyContacts.GetEmergencyContact(EC_Id);
                    if (emergencyContacts == null)
                        return BadRequest();

                    await unitOfWork.emergencyContacts.Delete(emergencyContacts);
                    await unitOfWork.SaveAsync();

                    return Ok(new
                    {
                        message = "contact has been removed successfully"
                    });
                }
            }
            return Unauthorized();
        }

        [HttpPost]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<ActionResult> CreateEmergenecyContacts(EmergencyContactDTO emergencyContact)
        {
            var emergency = new EmergencyContact
            {
                Name = emergencyContact.EC_FullName,
                CreatedAt = DateTime.UtcNow,
                Address = emergencyContact.Address,
                AlternatePhone = emergencyContact.AlternatePhone,
                IsActive = true, //. as default,
                IsPrimary = emergencyContact.IsPrimary,
                Notes = emergencyContact.Notes,
                PhoneNumber = emergencyContact.PhoneNumber,
            };
            emergency.Patient.User.FullName = emergencyContact.EC_FullName; //. this query will upload the patient and User table as eager loading in background of the query

            if (emergencyContact.Relation.ToString() == "Other")
            {
                if (emergencyContact.OtherRelationText == null)
                    return BadRequest("you must fill the other relation text field");
            }
            emergency.Relation = emergencyContact.Relation.ToString();

            await unitOfWork.emergencyContacts.Create(emergency);
            await unitOfWork.SaveAsync();

            return Ok(new
            {
                IsSuccess = true,
                message = "Contact has been created"
            });
        }


        [HttpPut("{EC_id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<ActionResult> UpdateEmergencyContacts(Guid EC_id, UpdateEmergencyContactsDTO emergencyContactsDTO)
        {
            string patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(patientId, out Guid result))
            {
                var emergencyContacts = await unitOfWork.emergencyContacts.GetEmergencyContact(EC_id);
                if (emergencyContacts == null)
                    return BadRequest();

                if (User.IsInRole("Patient") && User.Identity.IsAuthenticated)
                {
                    if (result != EC_id)
                        return Unauthorized("Not allow for you to update this EC way, Roll back to Adminstrator");

                    //. manual mapping 
                    emergencyContacts.Name = emergencyContactsDTO.EC_FullName;
                    emergencyContacts.PhoneNumber = emergencyContactsDTO.PhoneNumber;
                    emergencyContacts.AlternatePhone = emergencyContactsDTO.AlternatePhoneNumber;
                    emergencyContacts.Address = emergencyContactsDTO.Address;
                    emergencyContacts.UpdatedAt = DateTime.UtcNow;

                    if (emergencyContactsDTO.Relation.ToString() == "Other")
                    {
                        if (emergencyContactsDTO.OtherRelationText == null)
                            return BadRequest("you must fill the other relation text field");
                    }

                    emergencyContacts.Relation = emergencyContactsDTO.Relation.ToString();

                    await unitOfWork.emergencyContacts.Update(emergencyContacts);
                    await unitOfWork.SaveAsync();

                    return Ok(new
                    {
                        IsSuccess = true,
                        message = "contact has been updated"
                    });
                }

                //. as default this for the admin role 
                //. manual mapping 
                emergencyContacts.Name = emergencyContactsDTO.EC_FullName;
                emergencyContacts.PhoneNumber = emergencyContactsDTO.PhoneNumber;
                emergencyContacts.AlternatePhone = emergencyContactsDTO.AlternatePhoneNumber;
                emergencyContacts.Address = emergencyContactsDTO.Address;
                emergencyContacts.UpdatedAt = DateTime.UtcNow;
                
                if (emergencyContactsDTO.Relation.ToString() == "Other")
                {
                    if (emergencyContactsDTO.OtherRelationText == null)
                        return BadRequest("you must fill the other relation text field");
                }
                emergencyContacts.Relation = emergencyContactsDTO.Relation.ToString();
                
                await unitOfWork.emergencyContacts.Update(emergencyContacts);
                await unitOfWork.SaveAsync();
                
                return Ok(new
                {
                    message = "contact has updated successfully"
                });
            }
            return Unauthorized();
        }

    } 
}
