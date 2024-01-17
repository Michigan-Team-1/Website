using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Team1.Model.Constants;

namespace Team1.Model
{
    public class EventBase
    {
        [Key]
        public int EventId { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = ErrorMessages.RequiredField), StringLength(Constants.FieldSizes.NameLength, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string? Name { get; set; }

        [DataType(DataType.Date)]
        [Display(Name= "Event Date")]
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public DateTime EventDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Event Alt. Date")]
        public DateTime? EventAlternateDate { get; set; }
    }

    public class Event : EventBase
    {
        public OwnedTypes.AuditFields AuditFields { get; set; }

        #region Navigation Links

        public virtual ICollection<EventLocation> EventLocations { get; set; }

        #endregion
    }
}
