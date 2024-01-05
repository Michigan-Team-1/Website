using Team1.Infrastructure.UserIdentity;
using Team1.Model;
using Team1.Model.UserIdentity;

public static class UserPermissionServiceRoleRestrict
{
    public static IQueryable<Address> RoleRestrictAddresses(this UserPermissionService ups, IQueryable<Address> query, bool viewOnly)
    {
        if (ups.UserClaimModel.IsAdmin)
        { /* do nothing */ }
        else
            query = query.Where(w => w.UserId == ups.UserClaimModel.UserId);

        return query;
    }

    public static IQueryable<Announcement> RoleRestrictAnnouncements(this UserPermissionService ups, IQueryable<Announcement> query, bool viewOnly)
    {
        if (ups.UserClaimModel.IsAdmin)
        { /* do nothing */ }
        else
        {
            if (!viewOnly)
                query = query.Where(w => false); // They don't have access, filter them out.
        }

        return query;
    }

    public static IQueryable<Event> RoleRestrictEvents(this UserPermissionService ups, IQueryable<Event> query, bool viewOnly)
    {
        if (ups.UserClaimModel.IsAdmin)
        { /* do nothing */ }
        else
        {
            if(!viewOnly)
                query = query.Where(w => false); // They don't have access, filter them out.
        }

        return query;
    }

    public static IQueryable<Location> RoleRestrictLocations(this UserPermissionService ups, IQueryable<Location> query, bool viewOnly)
    {
        if (ups.UserClaimModel.IsAdmin)
        { /* do nothing */ }
        else
        {
            if (!viewOnly)
                query = query.Where(w => false); // They don't have access, filter them out.
        }

        return query;
    }

    public static IQueryable<Picture> RoleRestrictPictures(this UserPermissionService ups, IQueryable<Picture> query, bool viewOnly)
    {
        if (ups.UserClaimModel.IsAdmin)
        { /* do nothing */ }
        else
        {
            if (!viewOnly)
                query = query.Where(w => w.OwnerUserId == ups.UserClaimModel.UserId); // can only see their own
            else
                query = query.Where(w => w.ApprovedDateTime.HasValue);
        }

        return query;
    }

    public static IQueryable<Role> RoleRestrictRoles(this UserPermissionService ups, IQueryable<Role> query, bool grantingRoles, bool viewOnly)
    {
        if (grantingRoles && ups.UserClaimModel.RoleData != null && ups.UserClaimModel.RoleData.GrantableRoleIds != null)
            query = query.Where(w => ups.UserClaimModel.RoleData.GrantableRoleIds.Contains(w.RoleId));
        else if (grantingRoles || !viewOnly)
            query = query.Where(w => false);

        return query.OrderBy(o => o.Level);
    }

    public static IQueryable<User> RoleRestrictUsers(this UserPermissionService ups, IQueryable<User> query, bool viewOnly)
    {
        if (ups.UserClaimModel.IsAdmin)
        { /* do nothing */ }
        else
        {
            if (!viewOnly)
                query = query.Where(w => w.UserId == ups.UserClaimModel.UserId); // They don't have access, filter them out.
        }

        return query;
    }
}
