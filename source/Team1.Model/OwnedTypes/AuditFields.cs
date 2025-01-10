using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team1.Model.OwnedTypes;

public class AuditFieldsBase
{
  [Column(nameof(InactiveDateTime))]
  public DateTimeOffset? InactiveDateTime { get; set; }

  //[Column(nameof(DeletedDateTime))]
  //public DateTimeOffset? DeletedDateTime { get; set; }

  [Display(Name = "Created Date")]
  [DataType(DataType.DateTime)]
  [Column(nameof(CreatedDateTime))]
  public DateTimeOffset? CreatedDateTime { get; set; }

  [Display(Name = "Updated Date")]
  [DataType(DataType.DateTime)]
  [Column(nameof(UpdatedDateTime))]
  public DateTimeOffset? UpdatedDateTime { get; set; }

  [Display(Name = "Created By Id")]
  [Column(nameof(CreatedById))]
  public int? CreatedById { get; set; }

  [Display(Name = "Updated By Id")]
  [Column(nameof(UpdatedById))]
  public int? UpdatedById { get; set; }
}

[Owned]
public class AuditFields : AuditFieldsBase
{
  public AuditFields()
  { }

  public AuditFields(int userId, DateTime timestamp)
  {
    CreatedById = userId;
    CreatedDateTime = timestamp;
    UpdatedById = userId;
    UpdatedDateTime = timestamp;
  }

  [Display(Name = "Inactivated By Id")]
  [Column(nameof(InactivatedById))]
  public int? InactivatedById { get; set; }

  //[Display(Name = "Deleted By Id")]
  //[Column(nameof(DeletedById))]
  //public int? DeletedById { get; set; }

  public bool IsActive()
  {
    return /*!DeletedDateTime.HasValue &&*/ !InactiveDateTime.HasValue;
  }

  public bool IsNotActive()
  {
    return InactiveDateTime.HasValue /*|| DeletedDateTime.HasValue*/;
  }

  public void SetUpdated(int userId, DateTime timestamp)
  {
    UpdatedById = userId;
    UpdatedDateTime = timestamp;
  }

  public void SetActiveInactive(bool isActive, int userId, DateTime timestamp)
  {
    if (!isActive && !InactiveDateTime.HasValue)
    {
      InactiveDateTime = timestamp;
      InactivatedById = userId;
    }
    else if (isActive && InactiveDateTime.HasValue)
    {
      InactiveDateTime = null;
      InactivatedById = null;
    }
  }

  public void SetDeletedUndeleted(bool isDeleted, int userId, DateTime timestamp)
  {
    //if (isDeleted && !DeletedDateTime.HasValue)
    //{
    //    DeletedDateTime = timestamp;
    //    DeletedById = userId;
    //}
    //else if (!isDeleted && DeletedDateTime.HasValue)
    //{
    //    DeletedDateTime = null;
    //    DeletedById = null;
    //}
  }
}
