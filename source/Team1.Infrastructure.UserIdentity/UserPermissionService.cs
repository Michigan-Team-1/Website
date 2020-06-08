using Team1.Model.UserIdentity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Team1.Infrastructure.UserIdentity
{
    public class UserPermissionService
    {
        public UserPermissionService(string ipAddress)
        {
            IpAddress = ipAddress;
        }

        public bool IsSetup { get; private set; }

        public string IpAddress { get; private set; }

        public string FirstLastName
        {
            get
            {
                if (UserClaimModel != null)
                    return $"{UserClaimModel.FirstName} {UserClaimModel.LastName}";
                return null;
            }
        }

        public string LastFirstName
        {
            get
            {
                if (UserClaimModel != null)
                    return $"{UserClaimModel.LastName}, {UserClaimModel.FirstName}";
                return null;
            }
        }

        public bool IpAddressHasChanged
        {
            get
            {
                if (UserClaimModel != null)
                    return UserClaimModel.IpAddress != IpAddress;

                return true;
            }
        }

        public UserClaimModel UserClaimModel { get; set; }

        public UserPolicies UserPolicies { get { return UserClaimModel?.UserPolicies; } }

        public void Setup(UserClaimModel ucm)
        {
            UserClaimModel = ucm;
            IsSetup = true;
        }
    }
}
