using System.ComponentModel.DataAnnotations;

namespace Team1.Model
{
    public class TaskMemberTypeBase
    {
        [Display(Name = "Member Type")]
        public Enums.MemberTypeEnum MemberTypeId { get; set; }

        public int TaskId { get; set; }
    }

    public class TaskMemberType : TaskMemberTypeBase
    {
        #region Navigation Links

        public virtual MemberType MemberType { get; set; }

        public virtual Task Task { get; set; }

        #endregion
    }
}
