using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Team1.Model;

namespace Team1.Infrastructure.Dtos;

public class PictureDto : PictureBase
{
  public bool IsUpdated { get; set; }

  [Display(Name = "Is Active")]
  public bool IsActive { get; set; }
  [Display(Name = "Is Approved")]
  public bool IsApproved { get; set; }

  [Display(Name = "Image or Video Embed")]
  public string? Upload { get; set; }

  [Display(Name = "Picture Upload")]
  public DocumentObjDto Document { get; set; } = default!;

  public AuditFieldsDto AuditFieldsDto { get; set; } = default!;

  public override bool Equals(object? obj)
  {
    if (obj == null)
      return false;

    if (obj is PictureDto item)
    {
      return item.Description.IfNullThenEmptyString() == Description.IfNullThenEmptyString() && item.IsActive == IsActive && item.PictureId == PictureId
        && item.IsApproved == IsApproved && item.IsActive == IsActive
        && item.Document.DocumentFilename.IfNullThenEmptyString() == Document.DocumentFilename.IfNullThenEmptyString() && item.Document.DocumentDisplayName.IfNullThenEmptyString() == Document.DocumentDisplayName.IfNullThenEmptyString()
        && item.Document.MimeType.IfNullThenEmptyString() == Document.MimeType.IfNullThenEmptyString();
    }

    return false;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}

public class PictureDtoValidator : AbstractValidator<PictureDto>
{
  public PictureDtoValidator()
  {
    When(w => w.PictureId == 0, () =>
    {
      RuleFor(x => x.Upload).NotEmpty().WithMessage("An image or embed html text must be added.");
    });
  }
}