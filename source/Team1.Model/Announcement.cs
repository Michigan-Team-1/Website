using System;
using System.ComponentModel.DataAnnotations;
using Team1.Model.Constants;

namespace Team1.Model
{
    public class AnnouncementBase
    {
        [Key]
        public int AnnouncementId { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = ErrorMessages.RequiredField), StringLength(Constants.FieldSizes.NameLength, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string Title { get; set; }

        [Display(Name = "Body")]
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public string Body { get; set; }

        [Display(Name ="Start Showing On Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public DateTime StartShowingOnDate { get; set; }
    }

    public class Announcement : AnnouncementBase
    {
        public OwnedTypes.AuditFields AuditFields { get; set; }
    }
}
