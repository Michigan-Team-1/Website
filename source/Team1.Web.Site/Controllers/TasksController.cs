using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Services;
using Team1.Infrastructure.Services.Tasks;
using Team1.Web.Site.Infrastructure.UserIdentity.Policies;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// users controller
    /// </summary>
    [Authorize(Policy = PolicyNames.TaskAddEditDelete)]
    [Route("api/[controller]")]
    public class TasksController : BaseController
    {
        /// <summary>
        /// Get tasks
        /// </summary>
        /// <returns>list of tasks</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<TaskDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTasks()
        {
            var service = GetService<TasksGet>();
            return Ok(await service.GetTasks(false));
        }

        /// <summary>
        /// Create a task
        /// </summary>
        /// <param name="dto">task object</param>
        /// <returns>updated task object</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaskDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateTask([FromBody]TaskDto dto)
        {
            return await SaveTask(dto);
        }

        /// <summary>
        /// Update a task
        /// </summary>
        /// <param name="dto">task object</param>
        /// <returns>updated task object</returns>
        [HttpPut]
        [ProducesResponseType(typeof(TaskDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateTask([FromBody]TaskDto dto)
        {
            return await SaveTask(dto);
        }

        /// <summary>
        /// Delete or inactivate
        /// </summary>
        /// <param name="id">id to delete or inactivate</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (id <= 0)
                return CreateResponse(new BaseServiceResponse<int>(id, System.Net.HttpStatusCode.BadRequest));

            var service = GetService<TasksCreateUpdate>();
            return CreateResponse(await service.DeleteTask(id));
        }

        #region private helpers

        private async Task<IActionResult> SaveTask(TaskDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<TaskDto>(dto, System.Net.HttpStatusCode.BadRequest));

            var service = GetService<TasksCreateUpdate>();
            var response = await service.SaveTask(dto);
            return CreateResponse(response);
        }

        #endregion
    }
}
