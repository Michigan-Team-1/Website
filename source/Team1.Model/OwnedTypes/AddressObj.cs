using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Team1.Model.Constants;

namespace Team1.Model.OwnedTypes;

public class AddressObjBase
{
  [Display(Name = "Address Line 1")]
  [StringLength(FieldSizes.AddressFieldLengths, ErrorMessage = ErrorMessages.StringLengthMax), Required(ErrorMessage = ErrorMessages.RequiredField)]
  [Column(nameof(Address1))]
  public string? Address1 { get; set; }

  [Display(Name = "Address Line 2")]
  [StringLength(FieldSizes.AddressFieldLengths, ErrorMessage = ErrorMessages.StringLengthMax)]
  [Column(nameof(Address2))]
  public string? Address2 { get; set; }

  [Display(Name = "Address Line 3")]
  [StringLength(FieldSizes.AddressFieldLengths, ErrorMessage = ErrorMessages.StringLengthMax)]
  [Column(nameof(Address3))]
  public string? Address3 { get; set; }

  [Display(Name = "City"), Required(ErrorMessage = ErrorMessages.RequiredField)]
  [StringLength(FieldSizes.AddressFieldLengths, ErrorMessage = ErrorMessages.StringLengthMax)]
  [Column(nameof(City))]
  public string? City { get; set; }

  [Display(Name = "Governing District"), Required(ErrorMessage = ErrorMessages.RequiredField)]
  [Column(nameof(GoverningDistrictId))]
  public int? GoverningDistrictId { get; set; }

  [Display(Name = "Country")]
  [Column(nameof(CountryId)), Required(ErrorMessage = ErrorMessages.RequiredField)]
  public int? CountryId { get; set; }

  // inputmask handles length
  [Display(Name = "Postal Code"), Required(ErrorMessage = ErrorMessages.RequiredField)]
  [StringLength(12)]
  [Column(nameof(PostalCode))]
  public string? PostalCode { get; set; }

  public void Update(AddressObjBase item)
  {
    Address1 = item.Address1;
    Address2 = item.Address2;
    Address3 = item.Address3;
    City = item.City;
    GoverningDistrictId = item.GoverningDistrictId;
    CountryId = item.CountryId;
    PostalCode = item.PostalCode;
  }
}

//[Owned]
public class AddressObj : AddressObjBase
{
  #region Navigation Links

  [ForeignKey(nameof(CountryId))]
  public virtual Country Country { get; set; }
  [ForeignKey(nameof(GoverningDistrictId))]
  public virtual GoverningDistrict GoverningDistrict { get; set; }

  #endregion
}
