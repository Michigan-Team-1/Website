using Team1.Model;

namespace Team1.Infrastructure.Dtos
{
    public class TaskMemberTypeDto : TaskMemberTypeBase
    {
        public string MemberTypeString { get { return MemberTypeId.GetDisplayName(); } }
        public bool IsDeleted { get; set; }
        public bool IsAdded { get; set; }
    }
}
