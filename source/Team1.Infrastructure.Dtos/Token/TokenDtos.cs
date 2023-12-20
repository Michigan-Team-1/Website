using System;

namespace Team1.Infrastructure.Dtos.Token
{
    public class TokenDto
    {
        public string Token { get; set; }
        public DateTime ValidTo { get; set; }
        public string RefreshToken { get; set; }

        public UserPoliciesDto UserPolicies { get; set; }
        public TokenUserDto UserInfo { get; set; }
        public string[] Roles { get; set; }
    }

    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
    }
}
