using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team1.Model
{
  public class MemberTypeBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public Enums.MemberTypeEnum MemberTypeId { get; set; }

        [StringLength(Constants.FieldSizes.NameLength)]
        public string Name { get; set; }
    }

    public class MemberType : MemberTypeBase
    {
        public virtual ICollection<UserIdentity.UserMemberType> UserMemberTypes { get; set; }
    }
}
