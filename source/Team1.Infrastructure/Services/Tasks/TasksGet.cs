using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;

namespace Team1.Infrastructure.Services.Tasks
{
    public class TasksGet : BaseService
    {
        /// <summary>
        /// Gets all Tasks that the logged in user has access to.
        /// </summary>
        /// <param name="activeOnly">active only items</param>
        public async Task<IEnumerable<TaskDto>> GetTasks(bool activeOnly)
        {
            return await (from u in db.TasksByFilter(UserPermissionService, activeOnly)
                          select new TaskDto()
                          {
                              Description = u.Description,
                              DueDay = u.DueDay,
                              DueMonth = u.DueMonth,
                              TaskMemberTypes = u.TaskMemberTypes.Select(s=> new TaskMemberTypeDto() { MemberTypeId = s.MemberTypeId }).ToList(),
                              TaskCategoryId = u.TaskCategoryId,
                              TaskId = u.TaskId,
                              IsActive = !u.AuditFields.InactiveDateTime.HasValue,
                          }).ToListAsync();
        }

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="id">id to get</param>
        /// <returns>taskdto</returns>
        public async Task<TaskDto> GetTask(int id)
        {
            return await (from u in db.RoleRestrictedTasks(UserPermissionService)
                          where u.TaskId == id
                          select new TaskDto()
                          {
                              Description = u.Description,
                              DueDay = u.DueDay,
                              DueMonth = u.DueMonth,
                              TaskMemberTypes = u.TaskMemberTypes.Select(s => new TaskMemberTypeDto() { MemberTypeId = s.MemberTypeId }).ToList(),
                              TaskCategoryId = u.TaskCategoryId,
                              TaskId = u.TaskId,
                              IsActive = !u.AuditFields.InactiveDateTime.HasValue,
                          }).SingleOrDefaultAsync();
        }
    }
}
