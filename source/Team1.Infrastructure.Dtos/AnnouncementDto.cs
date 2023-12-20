using System.ComponentModel.DataAnnotations;
using Team1.Model;

namespace Team1.Infrastructure.Dtos
{
    public class AnnouncementDto : AnnouncementBase
    {
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public bool IsUpdated { get; set; }
    }
}
