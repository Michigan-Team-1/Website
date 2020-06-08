using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team1.Model
{
    public class GoverningDistrictBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int GoverningDistrictId { get; set; }

        public int CountryId { get; set; }
        [StringLength(100), Required]
        public string Name { get; set; }
        [StringLength(5)]
        public string Code { get; set; }
    }

    public class GoverningDistrict : GoverningDistrictBase
    {
        [ForeignKey(nameof(CountryId))]
        public virtual Country Country { get; set; }
    }
}
