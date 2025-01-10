using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Team1.Model.Constants;
using Team1.Model.UserIdentity;

namespace Team1.Infrastructure.Dtos.Users;

public class UserDto : UserRoot
{
  [Display(Name = "Role")]
  public List<RoleDto>? Roles { get; set; }

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

  [Display(Name = "Roles")]
  public List<UserRoleDto>? UserRoles { get; set; }

  [Display(Name = "Paid Up")]
  public bool PaidUp { get { return DateTime.Today.Year <= PaidThroughYear; } }

  [Display(Name = "Member Types")]
  public List<UserMemberTypeDto>? UserMemberTypes { get; set; }

  [Display(Name = "Member Types")]
  public string? UserMemberTypesString
  {
    get
    {
      if (UserMemberTypes == null)
        return null;

      return String.Join("/", UserMemberTypes.Select(s => s.MemberTypeString));
    }
  }

  [Display(Name = "Addresses")]
  public List<AddressDto> Addresses { get; set; } = default!;

  [Display(Name = "Is Active")]
  public bool IsActive { get; set; }
  public bool IsUpdated { get; set; }
  public AuditFieldsDto AuditFieldsDto { get; set; } = default!;

  /// <summary>
  /// Only used on register
  /// </summary>
  public string? Password { get; set; }
  [Display(Name = "Confirm Password")]
  public string? ConfirmPassword { get; set; }
  /// <summary>
  /// only used on register
  /// </summary>
  public string? Code { get; set; }

  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;

    if (obj is UserDto item)
    {
      if (!AreRolesEqual(item.UserRoles))
        return false;
      if (!AreUserMemberTypesEqual(item.UserMemberTypes))
        return false;
      if (!AreAddressesEqual(item.Addresses))
        return false;

      return item.PhoneNumber.IfNullThenEmptyString() == PhoneNumber.IfNullThenEmptyString() && item.Email.IfNullThenEmptyString() == Email.IfNullThenEmptyString()
                    && item.FirstName.IfNullThenEmptyString() == FirstName.IfNullThenEmptyString() && LastName.IfNullThenEmptyString() == item.LastName.IfNullThenEmptyString()
                    && item.UserId == UserId && IsActive == item.IsActive && item.IsLoginEnabled == IsLoginEnabled && item.PaidUp == PaidUp
                    && item.TripoliNumber.IfNullThenEmptyString() == TripoliNumber.IfNullThenEmptyString() && item.NarNumber.IfNullThenEmptyString() == NarNumber.IfNullThenEmptyString()
                    && item.BirthDate == BirthDate && item.MobileCarrierId == MobileCarrierId && item.CertificationLevel == CertificationLevel
                    && item.PaidThroughYear == PaidThroughYear && item.EmailConfirmed == EmailConfirmed;
    }

    return false;
  }

  public bool AreAddressesEqual(List<AddressDto> addresses)
  {
    if (addresses == null && Addresses == null)
      return true;
    else if (addresses == null && Addresses != null)
      return false;
    else if (addresses != null && Addresses == null)
      return false;
    else if (addresses!.Count != Addresses!.Count)
      return false;

    foreach (var item in from el in addresses
                         join eel in Addresses on el.AddressId equals eel.AddressId into ljEEL
                         from eel in ljEEL.DefaultIfEmpty()
                         select new { el, eel })
    {
      if (item.eel == null)
        return false;
      if (!item.el.Equals(item.eel))
        return false;
    }

    return true;
  }

  public bool AreRolesEqual(List<UserRoleDto>? userRoles)
  {
    if (userRoles == null && UserRoles == null)
      return true;
    else if (userRoles == null && UserRoles != null)
      return false;
    else if (userRoles != null && UserRoles == null)
      return false;
    else if (userRoles!.Count != UserRoles!.Count)
      return false;
    
    foreach (var item in from el in userRoles
                         join eel in UserRoles on el.RoleId equals eel.RoleId into ljEEL
                         from eel in ljEEL.DefaultIfEmpty()
                         select new { el, eel })
    {
      if (item.eel == null)
        return false;
      if (item.eel.RoleId != item.el.RoleId || item.eel.IsDeleted != item.el.IsDeleted)
        return false;
    }

    return true;
  }

  public bool AreUserMemberTypesEqual(List<UserMemberTypeDto>? userMemberTypes)
  {
    if (userMemberTypes == null && UserMemberTypes == null)
      return true;
    else if (userMemberTypes == null && UserMemberTypes != null)
      return false;
    else if (userMemberTypes != null && UserMemberTypes == null)
      return false;
    else if (userMemberTypes!.Count != UserMemberTypes!.Count)
      return false;

    foreach (var item in from el in userMemberTypes
                         join eel in UserMemberTypes on el.MemberTypeId equals eel.MemberTypeId into ljEEL
                         from eel in ljEEL.DefaultIfEmpty()
                         select new { el, eel })
    {
      if (item.eel == null)
        return false;
      if (item.eel.MemberTypeId != item.el.MemberTypeId || item.eel.IsDeleted != item.el.IsDeleted)
        return false;
    }

    return true;
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
    RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
    RuleFor(x => x.Email).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
    RuleFor(x => x.BirthDate).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
    RuleForEach(x => x.UserRoles).SetValidator(new UserRoleDtoValidator());
    RuleForEach(x => x.UserMemberTypes).SetValidator(new UserMemberTypeDtoValidator());
    RuleFor(x => x.Addresses).Must((x) => x != null && x.Count(w => !w.IsDeleted) > 0).WithMessage("Must have at least one address.");
    RuleForEach(x => x.Addresses).SetValidator(new AddressDtoValidator());

    // userId = -1, then we are on the registration page and password is required.
    When(w => w.UserId == -1, () =>
    {
      RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
      RuleFor(x => x.ConfirmPassword).Equal(s => s.Password).WithMessage("Confirm password must equal password.");
    });
  }
}