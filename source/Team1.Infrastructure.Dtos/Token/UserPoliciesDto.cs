using Team1.Infrastructure.UserIdentity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Team1.Infrastructure.Dtos.Token
{
    public class UserPoliciesDto : UserPolicies
    {
        public UserPoliciesDto() { }

        public UserPoliciesDto(UserPolicies dto) : base(dto)
        {
        }
    }
}
