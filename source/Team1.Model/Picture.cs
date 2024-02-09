using System.ComponentModel.DataAnnotations;

namespace Team1.Model;

public class PictureBase
  {
      [Key]
      public int PictureId { get; set; }

      public int OwnerUserId { get; set; }

      public int? ApprovedByUserId { get; set; }
      
      public DateTime? ApprovedDateTime { get; set; }

      [StringLength(Constants.FieldSizes.DescriptionLength, ErrorMessage = Constants.ErrorMessages.StringLengthMax)]
      public string? Description { get; set; }
  }

  public class Picture : PictureBase
  {
      public OwnedTypes.AuditFields AuditFields { get; set; }

      public OwnedTypes.DocumentObj DocumentObj { get; set; }

      #region Navigation Links

      public virtual UserIdentity.User OwnerUser { get; set; }

      #endregion
  }
