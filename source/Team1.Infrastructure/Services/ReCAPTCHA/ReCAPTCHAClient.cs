using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Team1.Infrastructure.Services.ReCAPTCHA
{
    public class ReCAPTCHAClient
    {
        private HttpClient _client;
        private Logs.SystemLogsCreate _systemLogCreates;
        private ReCAPTCHASettings _settings;

        public ReCAPTCHAClient(HttpClient client, Logs.SystemLogsCreate systemLogCreates, IOptions<ReCAPTCHASettings> settings)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://www.google.com/recaptcha/api/siteverify");
            _settings = settings.Value;
            _systemLogCreates = systemLogCreates;
        }

        public async Task<HttpResponseMessage> VerifyToken(string userIpAddress, string token)
        {
            var url = new Uri($"?secret={_settings.SecretKey}&response={token}&remoteip={userIpAddress}", UriKind.Relative);
            var res = await _client.PostAsync(url, new StringContent(""));
            return res;
        }
    }
}
