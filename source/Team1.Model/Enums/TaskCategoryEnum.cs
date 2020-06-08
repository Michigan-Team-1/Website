using System.ComponentModel.DataAnnotations;

namespace Team1.Model.Enums
{
    public enum TaskCategoryEnum:byte
    {
        Confirmation = 1,
        [Display(Name = "Send Email")]
        SendEmail = 2,
        [Display(Name = "Send Letter")]
        SendLetter = 3,
        Print = 4,
        [Display(Name = "Phone Call")]
        PhoneCall = 5,

        Other = 100,
    }
}
