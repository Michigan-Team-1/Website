using FluentValidation;
using Team1.Model.Constants;
using Team1.Model.UserIdentity;

namespace Team1.Infrastructure.Dtos.Users;

public class UserRoleDto : UserRoleBase
{
    public bool IsDeleted { get; set; }
}

public class UserRoleDtoValidator : AbstractValidator<UserRoleDto>
{
  public UserRoleDtoValidator()
  {
    When(x => !x.IsDeleted, () =>
    {
      RuleFor(x => x.RoleId).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
    });
  }
}