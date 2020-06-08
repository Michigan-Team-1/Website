using Microsoft.EntityFrameworkCore;
using Team1.Entities;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.UserIdentity;
using System.Linq;

public static class TaskQueries
{
    /// <summary>
    /// Get all restricted Task
    /// </summary>
    /// <param name="db">dataContext</param>
    /// <param name="ups">IUserPermissionService</param>
    /// <param name="viewOnly">if View only then entity change tracking is turned off</param>
    /// <returns>IQueryable</returns>
    public static IQueryable<Team1.Model.Task> RoleRestrictedTasks(this DataContext db, UserPermissionService ups, bool viewOnly = true)
    {
        var query = ups.RoleRestrictTasks(db.Tasks.AsQueryable(), viewOnly);
        if (viewOnly)
            query = query.AsNoTracking();
        return query;
    }

    /// <summary>
    /// Get all Tasks based on filter criteria
    /// </summary>
    /// <param name="db">dataContext</param>
    /// <param name="ups">IUserPermissionService</param>
    /// <param name="activeOnly">if true, then return active only</param>
    /// <returns>IQueryable</returns>
    public static IQueryable<Team1.Model.Task> TasksByFilter(this DataContext db, UserPermissionService ups, bool activeOnly)
    {
        var query = db.RoleRestrictedTasks(ups);
        if (activeOnly)
            query = query.Where(w => !w.AuditFields.InactiveDateTime.HasValue);
        return query;
    }
}
