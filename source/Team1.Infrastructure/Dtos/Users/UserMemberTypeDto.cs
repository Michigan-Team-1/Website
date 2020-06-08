using Team1.Model.UserIdentity;

namespace Team1.Infrastructure.Dtos.Users
{
    public class UserMemberTypeDto : UserMemberTypeBase
    {
        public string MemberTypeString { get { return MemberTypeId.GetDisplayName(); } }
        public bool IsDeleted { get; set; }
        public bool IsAdded { get; set; }
    }
}
