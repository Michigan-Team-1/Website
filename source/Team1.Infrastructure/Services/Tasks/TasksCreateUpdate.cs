using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.UserIdentity;
using Team1.Model;
using Team1.Model.OwnedTypes;

namespace Team1.Infrastructure.Services.Tasks
{
    public class TasksCreateUpdate:BaseService
    {
        public TasksCreateUpdate()
        {
        }
        
        /// <summary>
        /// Saves a Task
        /// </summary>
        /// <param name="dto">dto to save</param>
        /// <returns>updated dto object</returns>
        public async Task<BaseServiceResponse<T>> SaveTask<T>(T dto) where T : TaskDto
        {
            var response = new BaseServiceResponse<T>(dto);
            // make sure user has access to this
            if (!UserPermissionService.UserClaimModel.UserPolicies.TaskAddEditDelete)
            {
                response.Message = "You are not authorized to add/edit a task.";
                response.Status = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            // nothing changed, return
            if (!dto.IsUpdated)
                return response;

            var timestamp = DateTime.UtcNow;
            Team1.Model.Task dbObj;
            var isNew = dto.TaskId == 0;
            if (isNew)
            {
                dbObj = new Model.Task()
                {
                    AuditFields = new AuditFields(UserPermissionService.UserClaimModel.UserId, timestamp),
                };
                db.Tasks.Add(dbObj);
            }
            else
            {
                dbObj = await db.RoleRestrictedTasks(UserPermissionService, false).Include(i => i.TaskMemberTypes).SingleOrDefaultAsync(w => w.TaskId == dto.TaskId);
                if (dbObj == null)
                {
                    response.Message = "You are not authorized to edit this task.";
                    response.Status = System.Net.HttpStatusCode.Unauthorized;
                    return response;
                }
            }

            dbObj.Description = dto.Description;
            dbObj.DueDay = dto.DueDay;
            dbObj.DueMonth = dto.DueMonth;
            dbObj.TaskCategoryId = dto.TaskCategoryId;

            if (dbObj.TaskMemberTypes == null)
                dbObj.TaskMemberTypes = new List<TaskMemberType>();
            foreach (var item in dto.TaskMemberTypes)
            {
                var taskMemberType = dbObj.TaskMemberTypes.FirstOrDefault(w => w.MemberTypeId == item.MemberTypeId);
                if (taskMemberType != null && item.IsDeleted)
                {
                    db.TaskMemberTypes.Remove(taskMemberType);
                    continue;
                }
                else if (taskMemberType == null)
                {
                    dbObj.TaskMemberTypes.Add(new Model.TaskMemberType() { MemberTypeId = item.MemberTypeId });
                }
            }

            dbObj.AuditFields.SetActiveInactive(dto.IsActive, UserPermissionService.UserClaimModel.UserId, timestamp);

            dbObj.AuditFields.SetUpdated(UserPermissionService.UserClaimModel.UserId, timestamp);

            await db.SaveChangesAsync();

            if (isNew)
                dto.TaskId = dbObj.TaskId;

            dto.TaskMemberTypes = dto.TaskMemberTypes.Where(w => !w.IsDeleted).Select(s =>
            {
                s.IsAdded = false;
                return s;
            }).ToList();
            dto.IsUpdated = false;

            return response;
        }

        /// <summary>
        /// Deletes or inactivates a task (based on client needs)
        /// </summary>
        /// <param name="id">id to delete</param>
        public async Task<BaseServiceResponse<int>> DeleteTask(int id)
        {
            var response = new BaseServiceResponse<int>(id);

            // make sure user has access to this
            if (!UserPermissionService.UserClaimModel.UserPolicies.TaskAddEditDelete)
            {
                response.Message = "You are not authorized to add/edit a task.";
                response.Status = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            // make sure logged in user has permission to the requested user
            var dbObj = await db.RoleRestrictedTasks(UserPermissionService, false).FirstOrDefaultAsync(w => w.TaskId == id);
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
