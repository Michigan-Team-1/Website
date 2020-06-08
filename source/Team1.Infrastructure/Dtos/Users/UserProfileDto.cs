using Team1.Model.UserIdentity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Team1.Infrastructure.Dtos.Users
{
    public class UserProfileDto : UserRoot
    {
        [Display(Name ="Addresses")]
        public List<AddressDto> Addresses { get; set; }

        public bool IsUpdated { get; set; }
    }
}
