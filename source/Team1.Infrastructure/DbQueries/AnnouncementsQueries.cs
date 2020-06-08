using Microsoft.EntityFrameworkCore;
using System.Linq;
using Team1.Entities;
using Team1.Infrastructure.UserIdentity;

public static class AnnouncementsQueries
{
    /// <summary>
    /// Get all restricted Announcements
    /// </summary>
    /// <param name="db">dataContext</param>
    /// <param name="ups">IUserPermissionService</param>
    /// <param name="viewOnly">if View only then entity change tracking is turned off</param>
    /// <returns>IQueryable</returns>
    public static IQueryable<Team1.Model.Announcement> RoleRestrictedAnnouncements(this DataContext db, UserPermissionService ups, bool viewOnly = true)
    {
        var query = ups.RoleRestrictAnnouncements(db.Announcements.AsQueryable(), viewOnly);
        if (viewOnly)
            query = query.AsNoTracking();
        return query;
    }

    /// <summary>
    /// Get all Announcements based on filter criteria
    /// </summary>
    /// <param name="db">dataContext</param>
    /// <param name="ups">IUserPermissionService</param>
    /// <param name="activeOnly">if true, then return active only</param>
    /// <returns>IQueryable</returns>
    public static IQueryable<Team1.Model.Announcement> AnnouncementsByFilter(this DataContext db, UserPermissionService ups, bool activeOnly)
    {
        var query = db.RoleRestrictedAnnouncements(ups);
        if (activeOnly)
            query = query.Where(w => !w.AuditFields.InactiveDateTime.HasValue);
        return query;
    }
}
