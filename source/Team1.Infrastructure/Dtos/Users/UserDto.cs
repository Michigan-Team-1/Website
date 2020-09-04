using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Team1.Model.UserIdentity;

namespace Team1.Infrastructure.Dtos.Users
{
    public class UserDto : UserRoot
    {
        [Display(Name = "Role")]
        public List<RoleDto> Roles { get; set; }

        public string RoleNames
        {
            get
            {
                if (Roles == null || Roles.Count == 0)
                    return null;

                return string.Join(", ", Roles.OrderBy(o => o.NormalizedName).Select(s => s.Name));
            }
        }

        [Display(Name = "Paid Up")]
        public bool PaidUp { get { return DateTime.Today.Year <= PaidThroughYear; } }

        [Display(Name = "Member Types")]
        public List<UserMemberTypeDto> UserMemberTypes { get; set; }

        [Display(Name = "Addresses")]
        public List<AddressDto> Addresses { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public bool IsUpdated { get; set; }
    }
}
