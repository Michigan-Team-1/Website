using Team1.Model.UserIdentity;

namespace Team1.Infrastructure.Dtos;

public class RoleDto : RoleBase
{
  public RoleDataDto DataObj { get; set; }
}
