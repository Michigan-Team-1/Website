using FluentValidation;
using Team1.Model.Constants;
using Team1.Model.UserIdentity;

namespace Team1.Infrastructure.Dtos.Users;

public class UserMemberTypeDto : UserMemberTypeBase
{
  public string MemberTypeString { get { return MemberTypeId.GetDisplayName(); } }
  public bool IsDeleted { get; set; }
}

public class UserMemberTypeDtoValidator : AbstractValidator<UserMemberTypeDto>
{
  public UserMemberTypeDtoValidator()
  {
    When(x => !x.IsDeleted, () =>
    {
      RuleFor(x => x.MemberTypeId).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
    });
  }
}