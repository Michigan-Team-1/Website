using Microsoft.EntityFrameworkCore;
using Services.FileManager;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Helpers;
using Team1.Model.Enums;

namespace Team1.Infrastructure.Services.Pictures;

public class PicturesGet : BaseService
{
  private IFileManager _fileManager;

  public PicturesGet(IFileManager fileManager)
  {
    _fileManager = fileManager;
  }

  /// <summary>
  /// Gets all Pictures that the logged in user has access to.
  /// </summary>
  /// <param name="activeOnly">active only items</param>
  public Task<List<PictureDto>> GetPictures(bool activeOnly)
  {
    return (from x in db.PicturesByFilter(UserPermissionService, activeOnly)
            join createdBy in db.Users on x.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
            from createdBy in ljCreatedBy.DefaultIfEmpty()
            join updatedBy in db.Users on x.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
            from updatedBy in ljUpdatedBy.DefaultIfEmpty()
            select new PictureDto()
            {
              IsApproved = x.ApprovedDateTime.HasValue,
              Description = x.Description,
              PictureId = x.PictureId,
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
  /// Gets all pictures for gallery
  /// </summary>
  /// <param name="activeOnly">active only items</param>
  public Task<List<PictureDto>> GetPicturesForBrowsing()
  {
    return (from x in db.PicturesByFilter(UserPermissionService, true)
            join createdBy in db.Users on x.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
            from createdBy in ljCreatedBy.DefaultIfEmpty()
            join updatedBy in db.Users on x.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
            from updatedBy in ljUpdatedBy.DefaultIfEmpty()
            where x.ApprovedDateTime.HasValue
            select new PictureDto()
            {
              IsApproved = x.ApprovedDateTime.HasValue,
              Description = x.Description,
              PictureId = x.PictureId,
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
  /// Gets random pictures
  /// </summary>
  public Task<List<PictureDto>> GetRandomPictures()
  {
    var query = (from x in db.PicturesByFilter(UserPermissionService, true)
                 where x.ApprovedDateTime.HasValue
                 orderby x.AuditFields.CreatedDateTime descending
                 select new PictureDto()
                 {
                   IsApproved = x.ApprovedDateTime.HasValue,
                   Description = x.Description,
                   PictureId = x.PictureId,
                   IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                 }).Take(20);
    return query.OrderBy(o => Guid.NewGuid()).Take(5).ToListAsync();
  }

  /// <summary>
  /// Gets my Pictures
  /// </summary>
  public Task<List<PictureDto>> GetMyPictures()
  {
    return (from x in db.PicturesByFilter(UserPermissionService, false)
            join createdBy in db.Users on x.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
            from createdBy in ljCreatedBy.DefaultIfEmpty()
            join updatedBy in db.Users on x.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
            from updatedBy in ljUpdatedBy.DefaultIfEmpty()
            where x.OwnerUserId == UserPermissionService.UserClaimModel!.UserId
            select new PictureDto()
            {
              IsApproved = x.ApprovedDateTime.HasValue,
              Description = x.Description,
              PictureId = x.PictureId,
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
  /// Gets gallery types the user has access to
  /// </summary>
  public List<SelectOptionDto<byte>> GetGalleryTypes()
  {
    return GalleryTypeEnum.AdminMode.GetList().Where(w =>
    {
      switch (w)
      {
        case GalleryTypeEnum.Public:
          return true;
        case GalleryTypeEnum.MyGallery:
          return UserPermissionService.UserClaimModel.IsAuthenticated;
        case GalleryTypeEnum.AdminMode:
          return UserPermissionService.UserPolicies.CanApprovePicture;
      }

      return false;
    }).Select(s => new SelectOptionDto<byte>()
    {
      Text = s.GetDisplayName(),
      Value = (byte)s,
    }).ToList();
  }

  /// <summary>
  /// Gets a Picture
  /// </summary>
  /// <param name="id">id to get</param>
  /// <returns>dto</returns>
  public Task<PictureDto?> GetPicture(int id)
  {
    return (from x in db.RoleRestrictedPictures(UserPermissionService)
            where x.PictureId == id
            select new PictureDto()
            {
              IsApproved = x.ApprovedDateTime.HasValue,
              Description = x.Description,
              PictureId = x.PictureId,
              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
            }).SingleOrDefaultAsync();
  }

  /// <summary>
  /// Gets a Picture
  /// </summary>
  /// <param name="id">id to get</param>
  /// <returns>image file stream</returns>
  public async Task<FileDownload> GetPictureForViewing(int id)
  {
    var response = new FileDownload();
    var picture = await db.RoleRestrictedPictures(UserPermissionService).FirstOrDefaultAsync(w => w.PictureId == id);
    if (picture == null)
      return response;

    var filePathInfo = new FilePathInfo()
    {
      Path = PathManager.GetUserGalleryPath(picture.OwnerUserId),
      FileName = picture.DocumentObj.DocumentFilename,
    };

    using (var ms = new MemoryStream())
    using (var stream = _fileManager.OpenFile(filePathInfo, true))
    {
      await stream.CopyToAsync(ms);
      response.MimeType = picture.DocumentObj.MimeType;
      ms.Position = 0;
      response.FileBytes = ms.ToArray();
    }

    return response;
  }
}
