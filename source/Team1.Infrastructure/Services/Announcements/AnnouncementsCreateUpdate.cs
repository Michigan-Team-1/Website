using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.OwnedTypes;

namespace Team1.Infrastructure.Services.Announcements
{
    public class AnnouncementsCreateUpdate : BaseService
    {
        /// <summary>
        /// Saves an Announcement
        /// </summary>
        /// <param name="dto">dto to save</param>
        /// <returns>updated dto object</returns>
        public async Task<BaseServiceResponse<T>> SaveAnnouncement<T>(T dto) where T : AnnouncementDto
        {
            var response = new BaseServiceResponse<T>(dto);
            // make sure user has access to this
            if (!UserPermissionService.UserClaimModel.UserPolicies.AnnouncementAddEditDelete)
            {
                response.Message = "You are not authorized to add/edit a announcement.";
                response.Status = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            // nothing changed, return
            if (!dto.IsUpdated)
                return response;

            var timestamp = DateTime.UtcNow;
            Team1.Model.Announcement dbObj;
            var isNew = dto.AnnouncementId == 0;
            if (isNew)
            {
                dbObj = new Model.Announcement()
                {
                    AuditFields = new AuditFields(UserPermissionService.UserClaimModel.UserId, timestamp),
                };
                db.Announcements.Add(dbObj);
            }
            else
            {
                dbObj = await db.RoleRestrictedAnnouncements(UserPermissionService, false).SingleOrDefaultAsync(w => w.AnnouncementId == dto.AnnouncementId);
                if (dbObj == null)
                {
                    response.Message = "You are not authorized to edit this announcement.";
                    response.Status = System.Net.HttpStatusCode.Unauthorized;
                    return response;
                }
            }

            dbObj.Body = dto.Body;
            dbObj.StartShowingOnDate = dto.StartShowingOnDate;
            dbObj.Title = dto.Title;

            dbObj.AuditFields.SetActiveInactive(dto.IsActive, UserPermissionService.UserClaimModel.UserId, timestamp);

            dbObj.AuditFields.SetUpdated(UserPermissionService.UserClaimModel.UserId, timestamp);

            await db.SaveChangesAsync();

            if (isNew)
                dto.AnnouncementId = dbObj.AnnouncementId;

            dto.IsUpdated = false;

            return response;
        }

        /// <summary>
        /// Deletes or inactivates a Announcement (based on client needs)
        /// </summary>
        /// <param name="id">id to delete</param>
        public async Task<BaseServiceResponse<int>> DeleteAnnouncement(int id)
        {
            var response = new BaseServiceResponse<int>(id);

            // make sure user has access to this
            if (!UserPermissionService.UserClaimModel.UserPolicies.AnnouncementAddEditDelete)
            {
                response.Message = "You are not authorized to add/edit a announcement.";
                response.Status = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            // make sure logged in user has permission to the requested user
            var dbObj = await db.RoleRestrictedAnnouncements(UserPermissionService, false).FirstOrDefaultAsync(w => w.AnnouncementId == id);
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
