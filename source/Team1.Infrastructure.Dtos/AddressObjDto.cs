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