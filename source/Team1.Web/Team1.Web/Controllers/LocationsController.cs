using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Helpers;
using Team1.Infrastructure.Services;
using Team1.Infrastructure.Services.Locations;
using Team1.Web.Common.UserIdentity.Policies;

namespace Team1.Web.Controllers;

/// <summary>
/// users controller
/// </summary>
[Authorize(Policy = PolicyNames.LocationAddEditDelete)]
[Route("api/[controller]")]
public class LocationsController : BaseController
{
  /// <summary>
  /// Get Locations
  /// </summary>
  /// <returns>list of Locations</returns>
  [HttpGet]
  [ProducesResponseType(typeof(List<LocationDto>), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetLocations()
  {
    var service = GetService<LocationsGet>();
    return Ok(await service.GetLocations(false));
  }


  /// <summary>
  /// Get Locations for selection
  /// </summary>
  /// <returns>list of SelectOptionDto</returns>
  [HttpGet("ForSelection")]
  [ProducesResponseType(typeof(List<SelectOptionDto<int>>), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetLocationsForSelection()
  {
    var service = GetService<LocationsGet>();
    return Ok(await service.GetLocationsForSelection(true));
  }

  /// <summary>
  /// Create a Location
  /// </summary>
  /// <param name="dto">Location object</param>
  /// <returns>updated Location object</returns>
  [HttpPost]
  [ProducesResponseType(typeof(LocationDto), (int)HttpStatusCode.OK)]
  [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
  public async Task<IActionResult> CreateLocation([FromBody] LocationDto dto)
  {
    return await SaveLocation(dto);
  }

  /// <summary>
  /// Update a Location
  /// </summary>
  /// <param name="dto">Location object</param>
  /// <returns>updated Location object</returns>
  [HttpPut]
  [ProducesResponseType(typeof(LocationDto), (int)HttpStatusCode.OK)]
  [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
  public async Task<IActionResult> UpdateLocation([FromBody] LocationDto dto)
  {
    return await SaveLocation(dto);
  }

  /// <summary>
  /// Delete or inactivate
  /// </summary>
  /// <param name="id">id to delete or inactivate</param>
  [HttpDelete("{id}")]
  [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
  [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
  public async Task<IActionResult> DeleteLocation(int id)
  {
    if (id <= 0)
      return CreateResponse(new BaseServiceResponse<int>(id, System.Net.HttpStatusCode.BadRequest));

    var service = GetService<LocationsCreateUpdate>();
    return CreateResponse(await service.DeleteLocation(id));
  }

  #region private helpers

  private async Task<IActionResult> SaveLocation(LocationDto dto)
  {
    if (!ModelState.IsValid)
      return CreateResponse(new BaseServiceResponse<LocationDto>(dto, System.Net.HttpStatusCode.BadRequest));

    var service = GetService<LocationsCreateUpdate>();
    var response = await service.SaveLocation(dto);
    return CreateResponse(response);
  }

  #endregion
}
