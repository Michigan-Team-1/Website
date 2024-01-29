using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Team1.Model.Constants;
using Team1.Model.UserIdentity;

namespace Team1.Infrastructure.Dtos.Users;

public class UserDto : UserRoot
{
  [Display(Name = "Role")]
  public List<RoleDto> Roles { get; set; } = default!;

  [Display(Name = "Roles")]
  public string? RoleNames
  {
    get
    {
      if (Roles == null || Roles.Count == 0)
        return null;

      return string.Join(", ", Roles.OrderBy(o => o.NormalizedName).Select(s => s.Name));
    }
  }

  [Display(Name = "Paid Up")]
  public bool PaidUp { get { return DateTime.Today.Year <= PaidThroughYear; } }

  [Display(Name = "Member Types")]
  public List<UserMemberTypeDto> UserMemberTypes { get; set; } = default!;

  [Display(Name = "Member Types")]
  public string? UserMemberTypesString {  get { return String.Join("/", UserMemberTypes.Select(s => s.MemberTypeString)); } }

  [Display(Name = "Addresses")]
  public List<AddressDto> Addresses { get; set; } = default!;

  [Display(Name = "Is Active")]
  public bool IsActive { get; set; }
  public bool IsUpdated { get; set; }
  public AuditFieldsDto AuditFieldsDto { get; set; } = default!;

  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;

    if (obj is UserDto item)
    {
      return item.PhoneNumber.IfNullThenEmptyString() == PhoneNumber.IfNullThenEmptyString() && item.Email.IfNullThenEmptyString() == Email.IfNullThenEmptyString()
                    && item.FirstName.IfNullThenEmptyString() == FirstName.IfNullThenEmptyString() && LastName.IfNullThenEmptyString() == item.LastName.IfNullThenEmptyString()
                    && item.UserId == UserId && IsActive == item.IsActive && item.IsLoginEnabled == IsLoginEnabled && item.PaidUp == PaidUp
                    && item.TripoliNumber == item.TripoliNumber && item.NarNumber == NarNumber && item.BirthDate == BirthDate
                    && item.MobileCarrierId == MobileCarrierId;
    }

    return false;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}

public class UserDtoValidator : AbstractValidator<UserDto>
{
  public UserDtoValidator()
  {
    RuleFor(x => x.FirstName).NotEmpty().WithMessage(ErrorMessages.FVRequiredField)
      .MaximumLength(FieldSizes.NameLength).WithMessage(ErrorMessages.FVStringLengthMax);
    RuleFor(x => x.LastName).NotEmpty().WithMessage(ErrorMessages.FVRequiredField)
      .MaximumLength(FieldSizes.DescriptionLength).WithMessage(ErrorMessages.FVStringLengthMax);
    //RuleFor(x => x.Addresses).SetValidator(new AddressObjDtoValidator()).NotNull();

  }
}