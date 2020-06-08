using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Team1.Infrastructure.Services.Logs;

namespace Team1.Infrastructure.Services.Users
{
    public class UserRefreshTokensGet:BaseService
    {
        private readonly SystemLogsCreate _systemLogCreate;
        private AppSettings _appSettings;
        public UserRefreshTokensGet(SystemLogsCreate systemLogCreate, AppSettings appSettings)
        {
            _systemLogCreate = systemLogCreate;
            _appSettings = appSettings;
        }

        /// <summary>
        /// checks if refresh token is valid.
        /// </summary>
        /// <param name="refreshToken">token to find and validate</param>
        public async Task<BaseServiceResponse<int>> IsRefreshTokenValid(string refreshToken)
        {
            var timestamp = DateTime.UtcNow;
            var dbObj = await db.UserRefreshTokens.SingleOrDefaultAsync(w => w.RefreshToken == refreshToken && w.ExpiresOnDateTime > timestamp);

            var response = new BaseServiceResponse<int>(dbObj?.UserId ?? 0);
            if (dbObj == null)
            {
                response.Status = System.Net.HttpStatusCode.BadRequest;
                response.Message = "Invalid refresh token";
                await _systemLogCreate.SaveLog($"Failed to update refresh token.  Provided token was invalid", Model.Enums.LogTypeEnum.User);
            }
            else if (dbObj.ExpiresOnDateTime < timestamp)
            {
                response.Status = System.Net.HttpStatusCode.BadRequest;
                response.Message = "Invalid refresh token";
                await _systemLogCreate.SaveLog(dbObj.UserId, null, $"Refresh Token has Expired {dbObj.ExpiresOnDateTime.ToString("yyyy-MM-dd hh:mm")}", Model.Enums.LogTypeEnum.User);
            }
            else
            {
                //This is to add an additional delay should someone on the site be forcing the refresh token process.  This will hopefully help reduce the effectiveness of such a vector
                if (timestamp.AddMinutes(_appSettings.JWTSettings.RefreshTokenExpirationMinutes).Subtract(dbObj.ExpiresOnDateTime).TotalSeconds < 60)
                {
                    await Task.Delay(5000);
                    await _systemLogCreate.SaveLog(dbObj.UserId, null, $"High Frequency of token refresh requests.  Adding additional delay ({dbObj.UserId})", Model.Enums.LogTypeEnum.User);
                }
            }
            return response;
        }
    }
}
