using System;
using System.Collections.Generic;
using System.Text;

namespace Team1.Infrastructure
{
    public class JWTSettings
    {
        public string Issuer { get; set; }
        public string Key { get; set; }
        public int TokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationMinutes { get; set; }
    }
}
