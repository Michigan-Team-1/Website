using System.ComponentModel.DataAnnotations;

namespace Team1.Model.Enums
{
    public enum MemberTypeEnum : byte
    {
        Prefect = 1,
        [Display(Name = "Vice Prefect")]
        VicePrefect = 2,
        Secretary = 3,
        Treasurer = 4,
    }
}
