using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Services;
using Team1.Infrastructure.Services.Events;
using Team1.Web.Site.Infrastructure.UserIdentity.Policies;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// users controller
    /// </summary>
    [Authorize(Policy = PolicyNames.EventAddEditDelete)]
    [Route("api/[controller]")]
    public class EventsController : BaseController
    {
        /// <summary>
        /// Get Event
        /// </summary>
        /// <returns>list of Event</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<EventDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetEvents()
        {
            var service = GetService<EventsGet>();
            return Ok(await service.GetEvents(false));
        }
        
        /// <summary>
        /// Get Events for dashboard
        /// </summary>
        /// <returns>list of Event</returns>
        [HttpGet("ForDashboard")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<EventDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetEventsForDashboard()
        {
            var service = GetService<EventsGet>();
            return Ok(await service.GetEventsForDashboard());
        }

        /// <summary>
        /// Create a Event
        /// </summary>
        /// <param name="dto">Event object</param>
        /// <returns>updated Event object</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EventDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateEvent([FromBody]EventDto dto)
        {
            return await SaveEvent(dto);
        }

        /// <summary>
        /// Update a Event
        /// </summary>
        /// <param name="dto">Event object</param>
        /// <returns>updated Event object</returns>
        [HttpPut]
        [ProducesResponseType(typeof(EventDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateEvent([FromBody]EventDto dto)
        {
            return await SaveEvent(dto);
        }

        /// <summary>
        /// Delete or inactivate
        /// </summary>
        /// <param name="id">id to delete or inactivate</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (id <= 0)
                return CreateResponse(new BaseServiceResponse<int>(id, System.Net.HttpStatusCode.BadRequest));

            var service = GetService<EventsCreateUpdate>();
            return CreateResponse(await service.DeleteEvent(id));
        }

        #region private helpers

        private async Task<IActionResult> SaveEvent(EventDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<EventDto>(dto, System.Net.HttpStatusCode.BadRequest));

            var service = GetService<EventsCreateUpdate>();
            var response = await service.SaveEvent(dto);
            return CreateResponse(response);
        }

        #endregion
    }
}
