using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Helpers;
using Team1.Infrastructure.Services;
using Team1.Infrastructure.Services.Pictures;
using Team1.Model.Enums;

namespace Team1.Web.Controllers;

/// <summary>
/// users controller
/// </summary>
[Route("api/[controller]")]
public class PicturesController : BaseController
{
  private readonly ILogger<PicturesController> _logger;

  public PicturesController(ILogger<PicturesController> logger)
  {
    _logger = logger;
  }

  /// <summary>
  /// Get Picture
  /// </summary>
  /// <returns>list of Picture</returns>
  [HttpGet("public")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(List<PictureDto>), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetPublicPictures()
  {
    var service = GetService<PicturesGet>();
    return Ok(await service.GetPicturesForBrowsing());
  }

  /// <summary>
  /// Get My Picture
  /// </summary>
  /// <returns>list of Picture</returns>
  [HttpGet("admin")]
  [Authorize]
  [ProducesResponseType(typeof(List<PictureDto>), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetAdminMedia()
  {
    var service = GetService<PicturesGet>();
    return Ok(await service.GetPictures(false));
  }

  /// <summary>
  /// Get My Picture
  /// </summary>
  /// <returns>list of Picture</returns>
  [HttpGet("mine")]
  [Authorize]
  [ProducesResponseType(typeof(List<PictureDto>), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetMyMedia()
  {
    var service = GetService<PicturesGet>();
    return Ok(await service.GetMyPictures());
  }

  /// <summary>
  /// Gets random pictures
  /// </summary>
  /// <returns>list of Picture</returns>
  [HttpGet]
  [AllowAnonymous]
  [ProducesResponseType(typeof(List<PictureDto>), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetRandomPictures()
  {
    var service = GetService<PicturesGet>();
    return Ok(await service.GetRandomPictures());
  }

  /// <summary>
  /// Get a picture for viewing
  /// </summary>
  /// <returns>list of Picture</returns>
  [HttpGet("{id}/viewing")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(List<PictureDto>), (int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetPictureForViewing(int id)
  {
    var service = GetService<PicturesGet>();
    _logger.LogInformation("User {0}:  Getting picture {1} for viewing", UserPermissionService.UserClaimModel.UserId, id);
    var response = await service.GetPictureForViewing(id);
    return File(response.FileBytes, response.MimeType);
  }

  /// <summary>
  /// Gets the gallery types
  /// </summary>
  /// <returns>list of SelectOptionDto of byte</returns>
  [HttpGet("GalleryTypes")]
  [ProducesResponseType(typeof(List<SelectOptionDto<byte>>), (int)HttpStatusCode.OK)]
  public IActionResult GetGalleryTypes()
  {
    var service = GetService<PicturesGet>();
    return Ok(service.GetGalleryTypes());
  }


  /// <summary>
  /// Create a picture
  /// </summary>
  /// <param name="dto">picture object</param>
  /// <returns>updated picture object</returns>
  [HttpPost]
  [ProducesResponseType(typeof(PictureDto), (int)HttpStatusCode.OK)]
  [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
  public async Task<IActionResult> CreatePicture([FromBody] PictureDto dto)
  {
    return await SavePicture(dto);
  }

  /// <summary>
  /// Update a picture
  /// </summary>
  /// <param name="dto">Picture object</param>
  /// <returns>updated Picture object</returns>
  [HttpPut]
  [ProducesResponseType(typeof(PictureDto), (int)HttpStatusCode.OK)]
  [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
  public async Task<IActionResult> UpdatePicture([FromBody] PictureDto dto)
  {
    return await SavePicture(dto);
  }

  /// <summary>
  /// Delete or inactivate
  /// </summary>
  /// <param name="id">id to delete or inactivate</param>
  [HttpDelete("{id}")]
  [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
  [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
  public async Task<IActionResult> DeletePicture(int id)
  {
    if (id <= 0)
      return CreateResponse(new BaseServiceResponse<int>(id, System.Net.HttpStatusCode.BadRequest));

    var service = GetService<PicturesCreateUpdate>();
    return CreateResponse(await service.DeletePicture(id));
  }

  #region private helpers

  private async Task<IActionResult> SavePicture(PictureDto dto)
  {
    if (!ModelState.IsValid)
      return CreateResponse(new BaseServiceResponse<PictureDto>(dto, System.Net.HttpStatusCode.BadRequest));

    var service = GetService<PicturesCreateUpdate>();
    var response = await service.SavePicture(dto);
    return CreateResponse(response);
  }

  #endregion
}
