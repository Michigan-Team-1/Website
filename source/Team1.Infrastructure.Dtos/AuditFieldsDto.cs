using Team1.Model.OwnedTypes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Team1.Infrastructure.Dtos;

public class AuditFieldsDto : AuditFieldsBase
{
    [Display(Name = "Created By")]
    public string CreatedByName { get; set; }

    private DateOnly? createdDate;

    [Display(Name = "Created Date")]
    [JsonIgnore]
    public DateOnly? CreatedDate
    {
        get
        {
            if (!createdDate.HasValue && CreatedDateTime.HasValue)
                createdDate = DateOnly.FromDateTime(CreatedDateTime.Value.ToLocalTime().Date);

            return createdDate;
        }
    }

  private DateOnly? updatedDate;
  [Display(Name = "Updated Date")]
  [JsonIgnore]
  public DateOnly? UpdatedDate
  {
    get
    {
      if (!updatedDate.HasValue && UpdatedDateTime.HasValue)
        updatedDate = DateOnly.FromDateTime(UpdatedDateTime.Value.ToLocalTime().Date);

      return updatedDate;
    }
  }

  [Display(Name = "Updated By")]
    public string UpdatedByName { get; set; }

    public void SetCreated(AuditFields auditFields, string name)
    {
        CreatedByName = name;
        CreatedDateTime = auditFields.CreatedDateTime;
    }

    public void SetUpdated(AuditFields auditFields, string name)
    {
        UpdatedByName = name;
        UpdatedDateTime = auditFields.UpdatedDateTime;
    }
}
