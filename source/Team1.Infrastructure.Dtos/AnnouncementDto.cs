using System.ComponentModel.DataAnnotations;
using Team1.Model;

namespace Team1.Infrastructure.Dtos;

public class AnnouncementDto : AnnouncementBase
{
  [Display(Name = "Is Active")]
  public bool IsActive { get; set; }
  public bool IsUpdated { get; set; }

  public AuditFieldsDto AuditFieldsDto { get; set; } = default!;
  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;

    if (obj is AnnouncementDto item)
    {
      return item.Title.IfNullThenEmptyString() == Title.IfNullThenEmptyString() && item.IsActive == IsActive
        && item.StartShowingOnDate == StartShowingOnDate && item.Body.IfNullThenEmptyString() == Body.IfNullThenEmptyString()
        && item.AnnouncementId == AnnouncementId;
    }

    return false;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}
