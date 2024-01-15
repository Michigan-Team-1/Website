using FluentValidation;
using Team1.Model.Constants;
using Team1.Model.OwnedTypes;

namespace Team1.Infrastructure.Dtos;

public class AddressObjDto : AddressObjBase
{
  /// <summary>
  /// Used when you only want to display an address
  /// </summary>
  public string? GoverningDistrictName { get; set; }
  /// <summary>
  /// Used when you only want to display an address
  /// </summary>
  public string? CountryName { get; set; }

  /// <summary>
  /// postal code mask for the country
  /// </summary>
  public string? PostalCodeMask { get; set; }

  /// <summary>
  /// Used when editing an address
  /// </summary>
  public CountryDto? Country { get; set; }

  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;

    if (obj is AddressObjDto item)
    {
      return item.CountryId == CountryId && item.GoverningDistrictId == GoverningDistrictId && item.Address1.IfNullThenEmptyString() == Address1.IfNullThenEmptyString()
        && item.Address2.IfNullThenEmptyString() == Address2.IfNullThenEmptyString() && item.Address3.IfNullThenEmptyString() == Address3.IfNullThenEmptyString()
        && item.City.IfNullThenEmptyString() == City.IfNullThenEmptyString() && item.PostalCode.IfNullThenEmptyString() == PostalCode.IfNullThenEmptyString();
    }

    return false;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}

public class AddressObjDtoValidator : AbstractValidator<AddressObjDto>
{
  public AddressObjDtoValidator()
  {
    RuleFor(x => x.CountryId).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
    RuleFor(x => x.Address1).MaximumLength(FieldSizes.AddressFieldLengths).WithMessage(ErrorMessages.FVStringLengthMax)
      .NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
    RuleFor(x => x.Address2).MaximumLength(FieldSizes.AddressFieldLengths).WithMessage(ErrorMessages.FVStringLengthMax);
    RuleFor(x => x.Address3).MaximumLength(FieldSizes.AddressFieldLengths).WithMessage(ErrorMessages.FVStringLengthMax);
    RuleFor(x => x.City).MaximumLength(FieldSizes.AddressFieldLengths).WithMessage(ErrorMessages.FVStringLengthMax)
      .NotEmpty().WithMessage(ErrorMessages.FVRequiredField);

    When(x => !string.IsNullOrWhiteSpace(x.GoverningDistrictName), () =>
    {
      RuleFor(x => x.GoverningDistrictId).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
    });
    When(x => !string.IsNullOrWhiteSpace(x.PostalCodeMask), () => 
    {
      RuleFor(x => x.PostalCode).NotEmpty().WithMessage(ErrorMessages.FVRequiredField);
    });
  }
}