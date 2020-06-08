using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Team1.Infrastructure.Services.Users;
using Team1.Infrastructure.Dtos.Users;
using Team1.Infrastructure.Services;
using Team1.Web.Site.Infrastructure.UserIdentity.Policies;
using Team1.Infrastructure.Services.Addresses;
using System.Net;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// users controller
    /// </summary>
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        /// <summary>
        /// Get users
        /// </summary>
        /// <returns>list of users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUsers()
        {
            var service = GetService<UsersGet>();
            return Ok(await service.GetUsers(false));
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="dto">user object</param>
        /// <returns>updated user object</returns>
        [HttpPost]
        [Authorize(Policy = PolicyNames.UserAddEditDelete)]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody]UserDto dto)
        {
            return await SaveUser(dto);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="dto">user object</param>
        /// <returns>updated user object</returns>
        [HttpPut]
        [Authorize(Policy = PolicyNames.UserAddEditDelete)]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateUser([FromBody]UserDto dto)
        {
            return await SaveUser(dto);
        }

        /// <summary>
        /// Delete or inactivate
        /// </summary>
        /// <param name="id">id to delete or inactivate</param>
        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyNames.UserAddEditDelete)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
                return CreateResponse(new BaseServiceResponse<int>(id, System.Net.HttpStatusCode.BadRequest));

            var service = GetService<UsersCreateUpdate>();
            return CreateResponse(await service.DeleteUser(id));
        }

        #region private helpers

        private async Task<IActionResult> SaveUser(UserDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<UserDto>(dto, System.Net.HttpStatusCode.BadRequest));

            using (var transaction = await SpudContext.BeginTransactionAsync())
            {
                var service = GetService<UsersCreateUpdate>();
                var response = await service.SaveUser(dto);
                //var loggingService = GetService<Team1.Infrastructure.Services.Logs.SystemLogsCreate>();
                //await loggingService.SaveLog(Model.Enums.LogTypeEnum.Unknown, new Exception("blah"));
                if (response.Succeeded)
                {
                    var addressService = GetService<AddressCreateUpdate>();
                    var addressResponse = await addressService.SaveAddresses(dto.Addresses, dto.UserId);
                    if (addressResponse.Succeeded)
                        response.Data.Addresses = addressResponse.Data;
                    else
                        return CreateResponse(addressResponse);

                    transaction.Commit();
                }

                return CreateResponse(response);
            }
        }

        #endregion
    }
}
