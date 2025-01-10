using Microsoft.EntityFrameworkCore;
using Services.FileManager;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Helpers;

namespace Team1.Infrastructure.Services.Pictures;

public class PicturesGet : BaseService
{
  private IFileManager fileManager;

  public PicturesGet(IFileManager fileManager)
  {
    this.fileManager = fileManager;
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
            orderby x.AuditFields.CreatedDateTime descending
            select new PictureDto()
            {
              IsApproved = x.ApprovedDateTime.HasValue,
              Description = x.Description,
              PictureId = x.PictureId,
              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
              IsVideoLink = x.IsVideoLink,
              Document = new DocumentObjDto()
              {
                DocumentFilename = x.IsVideoLink ? x.DocumentObj.DocumentFilename : null!
              },
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
            orderby x.AuditFields.CreatedDateTime descending
            select new PictureDto()
            {
              IsApproved = x.ApprovedDateTime.HasValue,
              Description = x.Description,
              PictureId = x.PictureId,
              IsVideoLink = x.IsVideoLink,
              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
              Document = new DocumentObjDto()
              {
                DocumentFilename = x.IsVideoLink ? x.DocumentObj.DocumentFilename : null!
              },
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
                 where x.ApprovedDateTime.HasValue && !x.IsVideoLink
                 orderby x.AuditFields.CreatedDateTime descending
                 select new PictureDto()
                 {
                   IsApproved = x.ApprovedDateTime.HasValue,
                   Description = x.Description,
                   PictureId = x.PictureId,
                   IsVideoLink = x.IsVideoLink,
                   IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                   AuditFieldsDto = new AuditFieldsDto()
                   {
                     UpdatedDateTime = x.AuditFields.UpdatedDateTime,
                   },
                 }).Take(20);
    return query.OrderBy(o => Guid.NewGuid()).Take(10).ToListAsync();
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
            orderby x.AuditFields.CreatedDateTime descending
            select new PictureDto()
            {
              IsApproved = x.ApprovedDateTime.HasValue,
              Description = x.Description,
              PictureId = x.PictureId,
              IsVideoLink = x.IsVideoLink,
              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
              Document = new DocumentObjDto()
              {
                DocumentFilename = x.IsVideoLink ? x.DocumentObj.DocumentFilename : null!
              },
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
    using (var stream = fileManager.OpenFile(filePathInfo, true))
    {
      await stream.CopyToAsync(ms);
      response.MimeType = picture.DocumentObj.MimeType;
      ms.Position = 0;
      response.FileBytes = ms.ToArray();
    }

    return response;
  }
}
