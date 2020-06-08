using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Team1.Entities;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.UserIdentity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Team1.Infrastructure.Services.Users
{
    public class UserRefreshTokensCreateUpdate : BaseService
    {
        private AppSettings _appSettings;

        public UserRefreshTokensCreateUpdate(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        /// <summary>
        /// Saves a user refresh token
        /// </summary>
        /// <param name="userid">user's refresh token</param>
        /// <returns>refresh token</returns>
        public async Task<BaseServiceResponse<string>> SaveUserRefreshToken(int userId)
        {
            var timestamp = DateTime.UtcNow;
            var response = new BaseServiceResponse<string>(null);

            var dbObj = await db.UserRefreshTokens.SingleOrDefaultAsync(w => w.UserId == userId);
            if (dbObj == null)
            {
                dbObj = new Model.UserIdentity.UserRefreshToken()
                {
                    UserId = UserPermissionService.UserClaimModel.UserId > 0 ? UserPermissionService.UserClaimModel.UserId : userId,
                };
                db.UserRefreshTokens.Add(dbObj);
            }

            dbObj.ExpiresOnDateTime = timestamp.AddMinutes(_appSettings.JWTSettings.RefreshTokenExpirationMinutes);
            response.Data = dbObj.RefreshToken = $"{Guid.NewGuid()}{Guid.NewGuid()}".Replace("-", "");

            await db.SaveChangesAsync();

            return response;
        }

        /// <summary>
        /// Expires a user refresh token
        /// </summary>
        /// <param name="userid">user's refresh token</param>
        /// <returns>refresh token</returns>
        public async Task<BaseServiceResponse<bool>> ExpireUserRefreshToken(int userId)
        {
            var response = new BaseServiceResponse<bool>(true);

            var dbObj = await db.UserRefreshTokens.SingleAsync(w => w.UserId == userId);

            dbObj.ExpiresOnDateTime = new DateTime(1990, 1, 1);

            await db.SaveChangesAsync();

            return response;
        }
    }
}
