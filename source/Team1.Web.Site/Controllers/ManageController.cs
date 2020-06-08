using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Team1.Infrastructure.Dtos.Users;
using Team1.Infrastructure.Services;
using Team1.Infrastructure.Services.Addresses;
using Team1.Infrastructure.Services.Users;
using Team1.Web.Site.Infrastructure.UserIdentity.Policies;
using System.Net;
using System.Threading.Tasks;

namespace Team1.Web.Site.Controllers
{
    /// <summary>
    /// Manage controller
    /// </summary>
    [Produces("application/json")]
    [Authorize(Policy = PolicyNames.UserProfileEdit)]
    [Route("api/Manage")]
    public class ManageController : BaseController
    {
        /// <summary>
        /// Get user profile
        /// </summary>
        /// <returns>user profile</returns>
        [HttpGet]
        [ProducesResponseType(typeof(UserProfileDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserProfile()
        {
            var service = GetService<UsersGet>();
            return Ok(await service.GetUserProfile());
        }

        /// <summary>
        /// Update an user profile
        /// </summary>
        /// <param name="dto">user object</param>
        /// <returns>updated user object</returns>
        [HttpPut]
        [ProducesResponseType(typeof(UserProfileDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateUserProfile([FromBody]UserProfileDto dto)
        {
            return await SaveUser(dto);
        }

        #region private helpers

        private async Task<IActionResult> SaveUser(UserProfileDto dto)
        {
            if (!ModelState.IsValid)
                return CreateResponse(new BaseServiceResponse<UserProfileDto>(dto, System.Net.HttpStatusCode.BadRequest));

            using (var transaction = await SpudContext.BeginTransactionAsync())
            {
                var service = GetService<UsersCreateUpdate>();
                var response = await service.SaveUserProfile(dto);
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
