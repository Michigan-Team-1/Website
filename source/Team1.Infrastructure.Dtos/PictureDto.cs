using FluentValidation;
using System.ComponentModel.DataAnnotations;
using Team1.Model;

namespace Team1.Infrastructure.Dtos;

public class PictureDto : PictureBase
{
  public const string YouTubeLinkFormat = "https://youtube.com/embed/{videoId}";

  public bool IsUpdated { get; set; }

  [Display(Name = "Is Active")]
  public bool IsActive { get; set; }
  [Display(Name = "Is Approved")]
  public bool IsApproved { get; set; }

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
        && item.IsApproved == IsApproved && item.IsVideoLink == IsVideoLink
        && item.Document.DocumentFilename.IfNullThenEmptyString() == Document.DocumentFilename.IfNullThenEmptyString() && item.Document.DocumentDisplayName.IfNullThenEmptyString() == Document.DocumentDisplayName.IfNullThenEmptyString()
        && item.Document.MimeType.IfNullThenEmptyString() == Document.MimeType.IfNullThenEmptyString();
    }

    return false;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }

  public string GetPictureUrl()
  {
    return $"api/Pictures/{PictureId}/viewing?_={AuditFieldsDto?.UpdatedDateTime?.Ticks}";
  }
}

public class PictureDtoValidator : AbstractValidator<PictureDto>
{
  public PictureDtoValidator()
  {
    When(w => w.IsVideoLink, () =>
    {
      RuleFor(x => x.Document.DocumentFilename).NotEmpty().WithMessage("YouTube Link (Url) required.");
      RuleFor(x => x.Document.DocumentFilename).Must(x => x.ToLower().Contains("youtube.com/embed/")).WithMessage($"YouTube Link must be in this format: {PictureDto.YouTubeLinkFormat}");
      RuleFor(x => x.Document.DocumentFilename).Must(x => x.ToLower().StartsWith("https://")).WithMessage($"YouTube Link must be in this format: {PictureDto.YouTubeLinkFormat}");
    });
  }
}