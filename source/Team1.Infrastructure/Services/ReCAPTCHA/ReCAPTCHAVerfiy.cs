using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Team1.Entities;
using Team1.Infrastructure.UserIdentity;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Team1.Infrastructure.Services.ReCAPTCHA
{
    public class ReCAPTCHAVerfiy : BaseService
    {
        public ReCAPTCHAVerfiy(Logs.SystemLogsCreate systemLogCreates, ReCAPTCHAClient httpClient)
        {
            _systemLogCreates = systemLogCreates;
            _httpClient = httpClient;
        }

        
        private Logs.SystemLogsCreate _systemLogCreates;
        private readonly ReCAPTCHAClient _httpClient;

        /// <summary>
        /// Verifies reCaptcha.
        /// </summary>
        /// <param name="action">expected action to return from verification</param>
        /// <param name="token">user's token from ReCaptcha</param>
        public async Task<BaseServiceResponse<decimal>> Verify(string action, string token)
        {
            var reCaptchaResponse = await _httpClient.VerifyToken(UserPermissionService.IpAddress, token);
            var reCaptchaResponseContent = await reCaptchaResponse.Content.ReadAsStringAsync();
            var reCaptchaResponseObj = reCaptchaResponseContent.DeserializeJsonNet<ReCAPTCHAResponse>();

            await _systemLogCreates.SaveLog($"Token:  {token}, Response:  {reCaptchaResponseContent}", Model.Enums.LogTypeEnum.ReCaptcha);

            var response = new BaseServiceResponse<decimal>(reCaptchaResponseObj.Score);

            if (!reCaptchaResponseObj.Success)
            {
                response.Status = System.Net.HttpStatusCode.BadRequest;
                response.Message = $"ReCaptcha Error.  {string.Join(",", reCaptchaResponseObj.Error_Codes ?? new List<string>())}\nPlease try again.";
                return response;
            }

            if (reCaptchaResponseObj.Action?.ToLower() != action.ToLower())
            {
                response.Status = System.Net.HttpStatusCode.BadRequest;
                response.Message = "ReCaptcha Error.  Please try again.";
            }

            return response;
        }
    }

    public class ReCAPTCHASettings
    {
        public string SiteKey { get; set; }
        public string SecretKey { get; set; }
    }

    public class ReCAPTCHAResponse
    {
        public bool Success { get; set; }
        public decimal Score { get; set; }
        public string Action { get; set; }
        public DateTime Challenge_TS { get; set; }
        public string Hostname { get; set; }
        [JsonProperty(PropertyName = "Error-Codes")]
        public List<string> Error_Codes { get; set; }
    }
}
