using Microsoft.EntityFrameworkCore;
using Team1.Entities;
using Team1.Infrastructure.UserIdentity;
using Team1.Model;
using Team1.Model.UserIdentity;
using System.Linq;

public static class AddressQueries
{
    /// <summary>
    /// Get all restricted addresses
    /// </summary>
    /// <param name="db">dataContext</param>
    /// <param name="ups">IUserPermissionService</param>
    /// <param name="viewOnly">if View only then entity change tracking is turned off</param>
    /// <returns>IQueryable of T</returns>
    public static IQueryable<Address> RoleRestrictedAddresses(this DataContext db, UserPermissionService ups, bool viewOnly = true)
    {
        var query = ups.RoleRestrictAddresses(db.Addresses.AsQueryable(), viewOnly);
        if (viewOnly)
            query = query.AsNoTracking();
        return query;
    }

    /// <summary>
    /// Get all addresses based on filter criteria
    /// </summary>
    /// <param name="db">dataContext</param>
    /// <param name="ups">IUserPermissionService</param>
    /// <param name="activeOnly">if true, then return active only</param>
    /// <returns>IQueryable of T</returns>
    public static IQueryable<Address> AddressesByFilter(this DataContext db, UserPermissionService ups, bool activeOnly)
    {
        var query = db.RoleRestrictedAddresses(ups);
        if (activeOnly)
            query = query.Where(w => !w.AuditFields.InactiveDateTime.HasValue);
        return query;
    }
}
