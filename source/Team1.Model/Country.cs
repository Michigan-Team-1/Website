using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team1.Model;

public class CountryBase
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.None)]
  public int CountryId { get; set; }
  [StringLength(100), Required]
  public string Name { get; set; }

  [StringLength(2), Required]
  public string Alpha2 { get; set; }
  [StringLength(3), Required]
  public string Alpha3 { get; set; }

  [StringLength(10)]
  public string? PostalCodeMask { get; set; }
  [StringLength(50)]
  public string? GoverningDistrictName { get; set; }
}

public class Country : CountryBase
{
  public virtual ICollection<GoverningDistrict> GoverningDistricts { get; set; }
}
