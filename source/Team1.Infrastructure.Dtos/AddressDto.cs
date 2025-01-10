using FluentValidation;
using Team1.Model;

namespace Team1.Infrastructure.Dtos;

public class AddressDto : AddressBase
{
  public AddressObjDto AddressObj { get; set; } = default!;
  public bool IsActive { get; set; }
  public bool IsDeleted { get; set; }

  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;

    if (obj is AddressDto item)
    {
      return item.IsActive == IsActive && item.IsDeleted == IsDeleted && AddressObj.Equals(item.AddressObj);
    }

    return false;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
  public AddressDtoValidator()
  {
    RuleFor(x => x.AddressObj).SetValidator(new AddressObjDtoValidator()).NotNull();
  }
}