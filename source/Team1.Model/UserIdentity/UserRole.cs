using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team1.Model.UserIdentity
{
  public class UserRoleBase
  {
    public int UserId { get; set; }
    [Display(Name = "Role")]
    public byte RoleId { get; set; }
  }

  public class UserRole : UserRoleBase
  {
    #region navigation links

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }

    [ForeignKey(nameof(RoleId))]
    public virtual Role Role { get; set; }

    #endregion
  }
}
