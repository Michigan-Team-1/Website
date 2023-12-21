using Microsoft.EntityFrameworkCore;
using Services.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team1.Infrastructure.BusinessLogic;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.UserIdentity;
using Team1.Model;
using Team1.Model.OwnedTypes;

namespace Team1.Infrastructure.Services.Pictures
{
    public class PicturesCreateUpdate : BaseService
    {
        private IFileManager _fileManager;

        public PicturesCreateUpdate(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }
        
        /// <summary>
        /// Saves a Task
        /// </summary>
        /// <param name="dto">dto to save</param>
        /// <returns>updated dto object</returns>
        public async Task<BaseServiceResponse<T>> SavePicture<T>(T dto) where T : PictureDto
        {
            var response = new BaseServiceResponse<T>(dto);
            // make sure user has access to this
            if (!UserPermissionService.UserClaimModel.UserPolicies.PictureAddEditDelete)
            {
                response.Message = "You are not authorized to add/edit a picture.";
                response.Status = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            // nothing changed, return
            if (!dto.IsUpdated)
                return response;

            var timestamp = DateTime.UtcNow;
            Team1.Model.Picture dbObj;
            var isNew = dto.PictureId == 0;

            if(isNew && dto.FileUpload == null)
            {
                response.Message = "You must have a file uploaded.";
                response.Status = System.Net.HttpStatusCode.BadRequest;
                return response;
            }
            if (isNew)
            {
                dbObj = new Model.Picture()
                {
                    OwnerUserId = UserPermissionService.UserClaimModel.UserId,
                    AuditFields = new AuditFields(UserPermissionService.UserClaimModel.UserId, timestamp),
                };
                db.Pictures.Add(dbObj);
            }
            else
            {
                dbObj = await db.RoleRestrictedPictures(UserPermissionService, false).SingleOrDefaultAsync(w => w.PictureId == dto.PictureId);
                if (dbObj == null)
                {
                    response.Message = "You are not authorized to edit this picture.";
                    response.Status = System.Net.HttpStatusCode.Unauthorized;
                    return response;
                }
            }

            dbObj.Description = dto.Description;

            if (dto.FileUpload != null)
            {
                if (dbObj.ApprovedDateTime.HasValue && !UserPermissionService.UserClaimModel.IsAdmin)
                {
                    response.Message = "You can not change an image once it has been approved.  Contact an admin for more information.";
                    response.Status = System.Net.HttpStatusCode.Unauthorized;
                    return response;
                }

                if (!isNew)
                {
                    // delete old file
                    _fileManager.DeleteFile(new FilePathInfo()
                    {
                        FileName = dbObj.DocumentObj.DocumentFilename,
                        Path = PathManager.GetUserGalleryPath(dbObj.OwnerUserId)
                    });
                }

                //var fileName = dto.FileUpload.FileName;
                //var fileExtensions = AllowedFileExtensions.PictureDocuments.Split(',');
                //var extension = System.IO.Path.GetExtension(fileName).Replace(".", "");
                //if (!fileExtensions.Contains(extension.ToLower()))
                //{
                //    response.Status = System.Net.HttpStatusCode.BadRequest;
                //    response.Message = $"{System.IO.Path.GetFileName(fileName)} not an allowed file type.";
                //    return response;
                //}

                //var newFileName = PathManager.GetUserGalleryFilename(fileName, timestamp);
                //dto.Document.DocumentFilename = newFileName;
                //if (dbObj.DocumentObj == null)
                //    dbObj.DocumentObj = new DocumentObj();
                //dbObj.DocumentObj.DocumentFilename = dto.Document.DocumentFilename;
                //dbObj.DocumentObj.DocumentDisplayName = dto.Document.DocumentDisplayName;
                //dbObj.DocumentObj.MimeType = dto.Document.MimeType;

                //var filePathInfo = new FilePathInfo()
                //{
                //    Path = PathManager.GetUserGalleryPath(dbObj.OwnerUserId),
                //    FileName = newFileName,
                //    MimeType = dto.FileUpload.ContentType
                //};

                //if (!_fileManager.DirectoryExists(filePathInfo))
                //    _fileManager.CreateDirectory(filePathInfo);

                //// encrypt / save the file
                //using (var stream = _fileManager.OpenFile(filePathInfo, true))
                //{
                //    await dto.FileUpload.OpenReadStream().CopyToAsync(stream);
                //}

                // clear out file upload
                dto.FileUpload = null;
            }

            if (UserPermissionService.UserPolicies.CanApprovePicture)
            {
                if (dto.IsApproved && !dbObj.ApprovedDateTime.HasValue)
                {
                    dbObj.ApprovedDateTime = timestamp;
                    dbObj.ApprovedByUserId = UserPermissionService.UserClaimModel.UserId;
                }
                else if (!dto.IsApproved && dbObj.ApprovedDateTime.HasValue)
                {
                    dbObj.ApprovedDateTime = null;
                    dbObj.ApprovedByUserId = null;
                }
            }

            dbObj.AuditFields.SetActiveInactive(dto.IsActive, UserPermissionService.UserClaimModel.UserId, timestamp);

            dbObj.AuditFields.SetUpdated(UserPermissionService.UserClaimModel.UserId, timestamp);

            await db.SaveChangesAsync();

            if (isNew)
                dto.PictureId = dbObj.PictureId;

            dto.IsUpdated = false;

            return response;
        }

        /// <summary>
        /// Deletes or inactivates a picture (based on client needs)
        /// </summary>
        /// <param name="id">id to delete</param>
        public async Task<BaseServiceResponse<int>> DeletePicture(int id)
        {
            var response = new BaseServiceResponse<int>(id);

            // make sure user has access to this
            if (!UserPermissionService.UserClaimModel.UserPolicies.PictureAddEditDelete)
            {
                response.Message = "You are not authorized to add/edit a picture.";
                response.Status = System.Net.HttpStatusCode.Unauthorized;
                return response;
            }

            // make sure logged in user has permission to the requested user
            var dbObj = await db.RoleRestrictedPictures(UserPermissionService, false).FirstOrDefaultAsync(w => w.PictureId == id);
            if (dbObj == null)
            {
                response.Status = System.Net.HttpStatusCode.BadRequest;
                return response;
            }
            // delete (need to delete owned types and dependent tables)
            //db.Remove(dbObj);
            //db.Remove(dbObj.AuditFields);
            // inactivate
            dbObj.AuditFields.SetActiveInactive(false, UserPermissionService.UserClaimModel.UserId, DateTime.UtcNow);

            await db.SaveChangesAsync();

            return response;
        }
    }
}
