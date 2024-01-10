using System.ComponentModel.DataAnnotations;
using Team1.Model;

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
      if (item.AddressObj != null && AddressObj == null)
        return false;
      else if (item.AddressObj == null && AddressObj != null)
        return false;

      return item.FAAWaiver.IfNullThenEmptyString() == FAAWaiver.IfNullThenEmptyString() && item.IsActive == IsActive
        && item.LocationDescription.IfNullThenEmptyString() == LocationDescription.IfNullThenEmptyString()
        && item.LocationName.IfNullThenEmptyString() == LocationName.IfNullThenEmptyString() && item.LocationId == LocationId
        && (item.AddressObj?.Equals(AddressObj) ?? true);
    }

    return false;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}
