using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.UserIdentity;
using Team1.Model;
using Team1.Model.OwnedTypes;

namespace Team1.Infrastructure.Services.Addresses
{
    public class AddressCreateUpdate : BaseService
    {
        public AddressCreateUpdate()
        {
        }

        /// <summary>
        /// Saves an address
        /// </summary>
        /// <param name="dto">dto to save</param>
        /// <returns>updated dto object</returns>
        public async Task<BaseServiceResponse<AddressDto>> SaveAddress(AddressDto dto)
        {
            var response = new BaseServiceResponse<AddressDto>(dto);

            var isNew = dto.AddressId == 0;
            // make sure user has access to this
            if (!UserPermissionService.UserClaimModel.UserPolicies.UserAddEditDelete &&
                !UserPermissionService.UserClaimModel.UserPolicies.UserProfileEdit &&
                !isNew)
            {
                response.Message = "You are not authorized to add/edit an address.";
                response.Status = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            var timestamp = DateTime.UtcNow;
            Address? dbObj;
            
            if (isNew)
            {
                dbObj = new Address()
                {
                    AuditFields = new AuditFields(UserPermissionService.UserClaimModel.UserId, timestamp),
                    UserId = dto.UserId,
                    AddressObj = new AddressObj(),
                };
                db.Addresses.Add(dbObj);
            }
            else
            {
                dbObj = await db.RoleRestrictedAddresses(UserPermissionService, false).SingleOrDefaultAsync(w => w.AddressId == dto.AddressId);
                if (dbObj == null)
                {
                    response.Message = "You are not authorized to edit this address.";
                    response.Status = System.Net.HttpStatusCode.Unauthorized;
                    return response;
                }
            }
            
            dbObj.AddressObj.Update(dto.AddressObj);
            if (dbObj.AddressObj.CountryId == 0)
                dbObj.AddressObj.CountryId = dto.AddressObj.CountryId = 840; // USA
            dbObj.AuditFields.SetActiveInactive(dto.IsActive, UserPermissionService.UserClaimModel.UserId, timestamp);

            dbObj.AuditFields.SetUpdated(UserPermissionService.UserClaimModel.UserId, timestamp);

            await db.SaveChangesAsync();

            if (isNew)
                dto.AddressId = dbObj.AddressId;

            return response;
        }

        /// <summary>
        /// Saves/deletes all addresses
        /// </summary>
        /// <param name="addresses">addresses to save/delete</param>
        /// <param name="userId">userid they are based on</param>
        /// <returns>updated dto object</returns>
        public async Task<BaseServiceResponse<List<AddressDto>>> SaveAddresses(List<AddressDto> addresses, int? userId)
        {
            var response = new BaseServiceResponse<List<AddressDto>>(addresses);
            if (addresses == null || addresses.Count == 0)
                return response;

            for (int i = 0; i < addresses.Count; i++)
            {
                if (addresses[i].IsDeleted)
                {
                    var deleteResponse = await DeleteAddress(addresses[i].AddressId);
                    if (!deleteResponse.Succeeded)
                    {
                        response.UpdateResponseBasedOnResponse(deleteResponse);
                        return response;
                    }
                }
                else
                {
                    if (userId.HasValue && addresses[i].UserId != userId)
                        addresses[i].UserId = userId.Value;
                    var saveResponse = await SaveAddress(addresses[i]);
                    if (!saveResponse.Succeeded)
                    {
                        response.UpdateResponseBasedOnResponse(saveResponse);
                        return response;
                    }
                }
            }

            response.Data = addresses.Where(w => !w.IsDeleted).ToList();

            return response;
        }

        /// <summary>
        /// Deletes or inactivates an address (based on client needs)
        /// </summary>
        /// <param name="id">id to delete</param>
        public async Task<BaseServiceResponse<int>> DeleteAddress(int id)
        {
            var response = new BaseServiceResponse<int>(id);
            if (id == 0)
                return response;

            if (!UserPermissionService.UserClaimModel.UserPolicies.UserAddEditDelete && 
                !UserPermissionService.UserClaimModel.UserPolicies.UserProfileEdit)
            {
                response.Message = "You are not authorized to add/edit an address.";
                response.Status = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            // make sure logged in user has permission to the requested user
            var dbObj = await db.RoleRestrictedAddresses(UserPermissionService, false).FirstOrDefaultAsync(w => w.AddressId == id);
            if (dbObj == null)
            {
                response.Status = System.Net.HttpStatusCode.BadRequest;
                return response;
            }
            // delete (need to delete owned types and dependent tables)
            db.Remove(dbObj.AddressObj);
            db.Remove(dbObj.AuditFields);
            db.Remove(dbObj);
            // inactivate
            //dbObj.AuditFields.SetActiveInactive(false, ups.UserClaimModel.UserId, DateTime.UtcNow);

            await db.SaveChangesAsync();

            return response;
        }
    }
}
