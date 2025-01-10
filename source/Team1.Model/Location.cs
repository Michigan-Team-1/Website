using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Team1.Model.Constants;

namespace Team1.Model
{
    public class LocationBase
    {
        [Key]
        public int LocationId { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = ErrorMessages.RequiredField), StringLength(Constants.FieldSizes.NameLength, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string LocationName { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = ErrorMessages.RequiredField), StringLength(Constants.FieldSizes.DescriptionLength, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string LocationDescription { get; set; }

        [Display(Name= "FAA Waiver")]
        [StringLength(Constants.FieldSizes.NameLength, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string? FAAWaiver { get; set; }
    }

    public class Location : LocationBase
    {
        public OwnedTypes.AuditFields AuditFields { get; set; }

        public OwnedTypes.AddressObj AddressObj { get; set; }

        #region Navigation Links

        public virtual ICollection<EventLocation> EventLocations { get; set; }

        #endregion
    }
}
