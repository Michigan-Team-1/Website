using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Team1.Model;
using Team1.Model.Constants;

namespace Team1.Infrastructure.Dtos;

public class LocationDto : LocationBase
{
  [Display(Name = "Is Active")]
  public bool IsActive { get; set; }
  public bool IsUpdated { get; set; }

  [Display(Name = "Address")]
  [Required(ErrorMessage = "Address is required.")]
  public AddressObjDto AddressObj { get; set; } = default!;

  public AuditFieldsDto AuditFieldsDto { get; set; } = default!;

  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;

    if (obj is LocationDto item)
    {
      bool addressIsEqual;
      if (item.AddressObj == null && AddressObj == null)
        addressIsEqual = true;
      else if (item.AddressObj != null && AddressObj == null)
        return false;
      else if (item.AddressObj == null && AddressObj != null)
        return false;
      else
        addressIsEqual = item.AddressObj!.Equals(AddressObj);

      return item.FAAWaiver.IfNullThenEmptyString() == FAAWaiver.IfNullThenEmptyString() && item.IsActive == IsActive
        && item.LocationDescription.IfNullThenEmptyString() == LocationDescription.IfNullThenEmptyString()
        && item.LocationName.IfNullThenEmptyString() == LocationName.IfNullThenEmptyString() && item.LocationId == LocationId
        && addressIsEqual;
    }

    return false;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}

public class LocationDtoValidator : AbstractValidator<LocationDto>
{
  public LocationDtoValidator()
  {
    RuleFor(x => x.LocationName).NotEmpty().WithMessage(ErrorMessages.FVRequiredField)
      .MaximumLength(FieldSizes.NameLength).WithMessage(ErrorMessages.FVStringLengthMax);
    RuleFor(x => x.LocationDescription).NotEmpty().WithMessage(ErrorMessages.FVRequiredField)
      .MaximumLength(FieldSizes.DescriptionLength).WithMessage(ErrorMessages.FVStringLengthMax);
    RuleFor(x => x.AddressObj).SetValidator(new AddressObjDtoValidator()).NotNull();
    
  }
}