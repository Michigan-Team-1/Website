using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;

namespace Team1.Infrastructure.Services.Announcements
{
    public class AnnouncementsGet : BaseService
    {
        /// <summary>
        /// Gets all Announcements that the logged in user has access to.
        /// </summary>
        /// <param name="activeOnly">active only items</param>
        public async Task<IEnumerable<AnnouncementDto>> GetAnnouncements(bool activeOnly)
        {
            return await (from x in db.AnnouncementsByFilter(UserPermissionService, activeOnly)
                          select new AnnouncementDto()
                          {
                              AnnouncementId = x.AnnouncementId,
                              Body = x.Body,
                              StartShowingOnDate = x.StartShowingOnDate,
                              Title = x.Title,
                              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                          }).ToListAsync();
        }

        /// <summary>
        /// Gets a Announcement
        /// </summary>
        /// <param name="id">id to get</param>
        /// <returns>dto</returns>
        public async Task<AnnouncementDto> GetAnnouncement(int id)
        {
            return await (from x in db.RoleRestrictedAnnouncements(UserPermissionService)
                          where x.AnnouncementId == id
                          select new AnnouncementDto()
                          {
                              AnnouncementId = x.AnnouncementId,
                              Body = x.Body,
                              StartShowingOnDate = x.StartShowingOnDate,
                              Title = x.Title,
                              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                          }).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Gets all Announcements to display for dashboard
        /// </summary>
        /// <returns>list of dto</returns>
        public async Task<IEnumerable<AnnouncementDto>> GetAnnouncementsForDashboard()
        {
            var currentDate = DateTime.Today;
            return await (from x in db.AnnouncementsByFilter(UserPermissionService, true)
                          where x.StartShowingOnDate <= currentDate
                          orderby x.StartShowingOnDate descending
                          select new AnnouncementDto()
                          {
                              Body = x.Body,
                              Title = x.Title,
                              StartShowingOnDate = x.StartShowingOnDate
                          }).Take(20).ToListAsync();
        }
    }
}
