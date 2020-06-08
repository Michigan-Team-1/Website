using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.OwnedTypes;

namespace Team1.Infrastructure.Services.Locations
{
    public class LocationsCreateUpdate:BaseService
    {
        public LocationsCreateUpdate()
        {
        }

        /// <summary>
        /// Saves a Location
        /// </summary>
        /// <param name="dto">dto to save</param>
        /// <returns>updated dto object</returns>
        public async Task<BaseServiceResponse<T>> SaveLocation<T>(T dto) where T : LocationDto
        {
            var response = new BaseServiceResponse<T>(dto);
            // make sure user has access to this
            if (!UserPermissionService.UserClaimModel.UserPolicies.LocationAddEditDelete)
            {
                response.Message = "You are not authorized to add/edit a location.";
                response.Status = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            // nothing changed, return
            if (!dto.IsUpdated)
                return response;

            var timestamp = DateTime.UtcNow;
            Team1.Model.Location dbObj;
            var isNew = dto.LocationId == 0;
            if (isNew)
            {
                dbObj = new Model.Location()
                {
                    AddressObj = new AddressObj(),
                    AuditFields = new AuditFields(UserPermissionService.UserClaimModel.UserId, timestamp),
                };
                db.Locations.Add(dbObj);
            }
            else
            {
                dbObj = await db.RoleRestrictedLocations(UserPermissionService, false).SingleOrDefaultAsync(w => w.LocationId == dto.LocationId);
                if (dbObj == null)
                {
                    response.Message = "You are not authorized to edit this location.";
                    response.Status = System.Net.HttpStatusCode.Unauthorized;
                    return response;
                }
            }

            dbObj.FAAWaiver = dto.FAAWaiver;
            dbObj.LocationDescription = dto.LocationDescription;
            dbObj.LocationId = dto.LocationId;
            dbObj.LocationName = dto.LocationName;
            dbObj.AddressObj.Address1 = dto.AddressObj.Address1;
            dbObj.AddressObj.Address2 = dto.AddressObj.Address2;
            dbObj.AddressObj.Address3 = dto.AddressObj.Address3;
            dbObj.AddressObj.City = dto.AddressObj.City;
            dbObj.AddressObj.CountryId = dto.AddressObj.CountryId;
            dbObj.AddressObj.GoverningDistrictId = dto.AddressObj.GoverningDistrictId;
            dbObj.AddressObj.PostalCode = dto.AddressObj.PostalCode;

            dbObj.AuditFields.SetActiveInactive(dto.IsActive, UserPermissionService.UserClaimModel.UserId, timestamp);

            dbObj.AuditFields.SetUpdated(UserPermissionService.UserClaimModel.UserId, timestamp);

            await db.SaveChangesAsync();

            if (isNew)
                dto.LocationId = dbObj.LocationId;

            dto.IsUpdated = false;

            return response;
        }

        /// <summary>
        /// Deletes or inactivates a location (based on client needs)
        /// </summary>
        /// <param name="id">id to delete</param>
        public async Task<BaseServiceResponse<int>> DeleteLocation(int id)
        {
            var response = new BaseServiceResponse<int>(id);

            // make sure user has access to this
            if (!UserPermissionService.UserClaimModel.UserPolicies.LocationAddEditDelete)
            {
                response.Message = "You are not authorized to add/edit a location.";
                response.Status = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            // make sure logged in user has permission to the requested user
            var dbObj = await db.RoleRestrictedLocations(UserPermissionService, false).FirstOrDefaultAsync(w => w.LocationId == id);
            if (dbObj == null)
            {
                response.Status = System.Net.HttpStatusCode.BadRequest;
                return response;
            }
            // delete (need to delete owned types and dependent tables)
            //db.Remove(dbObj);
            //db.Remove(dbObj.AuditFields);
            // inactivate
            dbObj.AuditFields.SetActiveInactive(false, UserPermissionService.UserClaimModel.UserId, DateTime.UtcNow);

            await db.SaveChangesAsync();

            return response;
        }
    }
}
