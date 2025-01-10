using Microsoft.EntityFrameworkCore;
using Team1.Infrastructure.Dtos;

namespace Team1.Infrastructure.Services.Announcements;

public class AnnouncementsGet : BaseService
{
  /// <summary>
  /// Gets all Announcements that the logged in user has access to.
  /// </summary>
  /// <param name="activeOnly">active only items</param>
  public Task<List<AnnouncementDto>> GetAnnouncements(bool activeOnly)
  {
    return (from x in db.AnnouncementsByFilter(UserPermissionService, activeOnly)
            join createdBy in db.Users on x.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
            from createdBy in ljCreatedBy.DefaultIfEmpty()
            join updatedBy in db.Users on x.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
            from updatedBy in ljUpdatedBy.DefaultIfEmpty()
            select new AnnouncementDto()
            {
              AnnouncementId = x.AnnouncementId,
              Body = x.Body,
              StartShowingOnDate = x.StartShowingOnDate,
              Title = x.Title,
              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
              AuditFieldsDto = new AuditFieldsDto()
              {
                CreatedDateTime = x.AuditFields.CreatedDateTime,
                UpdatedDateTime = x.AuditFields.UpdatedDateTime,
                CreatedByName = createdBy != null ? string.Concat(createdBy.FirstName, " ", createdBy.LastName) : " - ",
                UpdatedByName = updatedBy != null ? string.Concat(updatedBy.FirstName, " ", updatedBy.LastName) : " - "
              },
            }).ToListAsync();
  }

  /// <summary>
  /// Gets a Announcement
  /// </summary>
  /// <param name="id">id to get</param>
  /// <returns>dto</returns>
  public Task<AnnouncementDto?> GetAnnouncement(int id)
  {
    return (from x in db.RoleRestrictedAnnouncements(UserPermissionService)
            join createdBy in db.Users on x.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
            from createdBy in ljCreatedBy.DefaultIfEmpty()
            join updatedBy in db.Users on x.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
            from updatedBy in ljUpdatedBy.DefaultIfEmpty()
            where x.AnnouncementId == id
            select new AnnouncementDto()
            {
              AnnouncementId = x.AnnouncementId,
              Body = x.Body,
              StartShowingOnDate = x.StartShowingOnDate,
              Title = x.Title,
              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
              AuditFieldsDto = new AuditFieldsDto()
              {
                CreatedDateTime = x.AuditFields.CreatedDateTime,
                UpdatedDateTime = x.AuditFields.UpdatedDateTime,
                CreatedByName = createdBy != null ? string.Concat(createdBy.FirstName, " ", createdBy.LastName) : " - ",
                UpdatedByName = updatedBy != null ? string.Concat(updatedBy.FirstName, " ", updatedBy.LastName) : " - "
              },
            }).SingleOrDefaultAsync();
  }

  /// <summary>
  /// Gets all Announcements to display for dashboard
  /// </summary>
  /// <returns>list of dto</returns>
  public Task<List<AnnouncementDto>> GetAnnouncementsForHome()
  {
    var currentDate = DateTime.Today;
    return (from x in db.AnnouncementsByFilter(UserPermissionService, true)
            join createdBy in db.Users on x.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
            from createdBy in ljCreatedBy.DefaultIfEmpty()
            join updatedBy in db.Users on x.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
            from updatedBy in ljUpdatedBy.DefaultIfEmpty()
            where x.StartShowingOnDate <= currentDate
            orderby x.StartShowingOnDate descending
            select new AnnouncementDto()
            {
              Body = x.Body,
              Title = x.Title,
              StartShowingOnDate = x.StartShowingOnDate,
              AuditFieldsDto = new AuditFieldsDto()
              {
                CreatedDateTime = x.AuditFields.CreatedDateTime,
                UpdatedDateTime = x.AuditFields.UpdatedDateTime,
                CreatedByName = createdBy != null ? string.Concat(createdBy.FirstName, " ", createdBy.LastName) : " - ",
                UpdatedByName = updatedBy != null ? string.Concat(updatedBy.FirstName, " ", updatedBy.LastName) : " - "
              },
            }).Take(18).ToListAsync();
  }
}
