using System.ComponentModel.DataAnnotations;

namespace Team1.Model.Enums
{
    public enum GalleryTypeEnum : byte
    {
        Public = 1,
        [Display(Name = "My Gallery")]
        MyGallery = 2,
        [Display(Name = "Admin Mode")]
        AdminMode = 3,
    }
}
