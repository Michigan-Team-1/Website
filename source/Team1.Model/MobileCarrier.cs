using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Team1.Model.Constants;
using Team1.Model.UserIdentity;

namespace Team1.Model
{
    public class MobileCarrierBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int MobileCarrierId { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = ErrorMessages.RequiredField), StringLength(Constants.FieldSizes.NameLength, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string MobileCarrierName { get; set; }

        [Display(Name = "Texting Email Suffix")]
        [Required(ErrorMessage = ErrorMessages.RequiredField), StringLength(Constants.FieldSizes.NameLength, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string TextingEmailSuffix { get; set; }
    }

    public class MobileCarrier : MobileCarrierBase
    {
        public virtual ICollection<User> Users { get; set; }
    }
}
