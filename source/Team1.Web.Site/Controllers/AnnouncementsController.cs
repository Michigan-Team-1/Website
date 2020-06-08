using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Services;
using Team1.Infrastructure.Services.Announcements;
using Team1.Web.Site.Infrastructure.UserIdentity.Policies;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// users controller
    /// </summary>
    [Authorize(Policy = PolicyNames.AnnouncementAddEditDelete)]
    [Route("api/[controller]")]
    public class AnnouncementsController : BaseController
    {
        /// <summary>
        /// Get Announcement
        /// </summary>
        /// <returns>list of Announcement</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<AnnouncementDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAnnouncements()
        {
            var service = GetService<AnnouncementsGet>();
            return Ok(await service.GetAnnouncements(false));
        }
        
        /// <summary>
        /// Get Announcement
        /// </summary>
        /// <returns>list of Announcement</returns>
        [HttpGet("ForDashboard")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<AnnouncementDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAnnouncementsForDashboard()
        {
            var service = GetService<AnnouncementsGet>();
            return Ok(await service.GetAnnouncementsForDashboard());
        }

        /// <summary>
        /// Create a Announcement
        /// </summary>
        /// <param name="dto">Announcement object</param>
        /// <returns>updated Announcement object</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AnnouncementDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAnnouncement([FromBody]AnnouncementDto dto)
        {
            return await SaveAnnouncement(dto);
        }

        /// <summary>
        /// Update a Announcement
        /// </summary>
        /// <param name="dto">Announcement object</param>
        /// <returns>updated Announcement object</returns>
        [HttpPut]
        [ProducesResponseType(typeof(AnnouncementDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateAnnouncement([FromBody]AnnouncementDto dto)
        {
            return await SaveAnnouncement(dto);
        }

        /// <summary>
        /// Delete or inactivate
        /// </summary>
        /// <param name="id">id to delete or inactivate</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            if (id <= 0)
                return CreateResponse(new BaseServiceResponse<int>(id, System.Net.HttpStatusCode.BadRequest));

            var service = GetService<AnnouncementsCreateUpdate>();
            return CreateResponse(await service.DeleteAnnouncement(id));
        }

        #region private helpers

        private async Task<IActionResult> SaveAnnouncement(AnnouncementDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<AnnouncementDto>(dto, System.Net.HttpStatusCode.BadRequest));

            var service = GetService<AnnouncementsCreateUpdate>();
            var response = await service.SaveAnnouncement(dto);
            return CreateResponse(response);
        }

        #endregion
    }
}
