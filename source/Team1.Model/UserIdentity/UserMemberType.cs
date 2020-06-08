using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Team1.Model.UserIdentity
{
    public class UserMemberTypeBase
    {
        [Display(Name = "Member Type")]
        public Enums.MemberTypeEnum MemberTypeId { get; set; }

        public int UserId { get; set; }
    }

    public class UserMemberType : UserMemberTypeBase
    {
        #region Navigation Links

        public virtual MemberType MemberType { get; set; }

        public virtual User User { get; set; }

        #endregion
    }
}
