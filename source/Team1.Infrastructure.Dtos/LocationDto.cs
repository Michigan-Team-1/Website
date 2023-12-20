using System.ComponentModel.DataAnnotations;
using Team1.Model;

namespace Team1.Infrastructure.Dtos
{
    public class LocationDto : LocationBase
    {
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public bool IsUpdated { get; set; }

        [Display(Name ="Address")]
        [Required(ErrorMessage = "Address is required.")]
        public AddressObjDto AddressObj { get; set; }
    }
}
