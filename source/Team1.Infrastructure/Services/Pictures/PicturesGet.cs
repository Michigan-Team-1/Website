using Microsoft.EntityFrameworkCore;
using Services.FileManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Helpers;
using Team1.Model.Enums;

namespace Team1.Infrastructure.Services.Pictures
{
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
        public async Task<IEnumerable<PictureDto>> GetPictures(bool activeOnly)
        {
            return await (from x in db.PicturesByFilter(UserPermissionService, activeOnly)
                          select new PictureDto()
                          {
                              IsApproved = x.ApprovedDateTime.HasValue,
                              Description = x.Description,
                              PictureId = x.PictureId,
                              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                          }).ToListAsync();
        }

        /// <summary>
        /// Gets all pictures for gallery
        /// </summary>
        /// <param name="activeOnly">active only items</param>
        public async Task<IEnumerable<PictureDto>> GetPicturesForBrowsing()
        {
            return await (from x in db.PicturesByFilter(UserPermissionService, true)
                          where x.ApprovedDateTime.HasValue
                          select new PictureDto()
                          {
                              IsApproved = x.ApprovedDateTime.HasValue,
                              Description = x.Description,
                              PictureId = x.PictureId,
                              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                          }).ToListAsync();
        }

        /// <summary>
        /// Gets random pictures
        /// </summary>
        public async Task<IEnumerable<PictureDto>> GetRandomPictures()
        {
            return await (from x in db.PicturesByFilter(UserPermissionService, true)
                          where x.ApprovedDateTime.HasValue
                          select new PictureDto()
                          {
                              IsApproved = x.ApprovedDateTime.HasValue,
                              Description = x.Description,
                              PictureId = x.PictureId,
                              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                          }).OrderBy(o => Guid.NewGuid()).Take(5).ToListAsync();
        }

        /// <summary>
        /// Gets my Pictures
        /// </summary>
        public async Task<IEnumerable<PictureDto>> GetMyPictures()
        {
            return await (from x in db.PicturesByFilter(UserPermissionService, false)
                          where x.OwnerUserId == UserPermissionService.UserClaimModel.UserId
                          select new PictureDto()
                          {
                              IsApproved = x.ApprovedDateTime.HasValue,
                              Description = x.Description,
                              PictureId = x.PictureId,
                              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
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
        public async Task<PictureDto> GetPicture(int id)
        {
            return await (from x in db.RoleRestrictedPictures(UserPermissionService)
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
}
