using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team1.Model.OwnedTypes;

[Owned]
public class DocumentObj
{
  /// <summary>
  /// File Name on disk or Embed Html
  /// </summary>
  [Column(nameof(DocumentFilename))]
  [Display(Name = "Filename On Disk")]
  [Required(ErrorMessage = Constants.ErrorMessages.RequiredField)]
  [StringLength(Constants.FieldSizes.DocumentFilenameLength, ErrorMessage = Constants.ErrorMessages.StringLengthMax)]
  public string DocumentFilename { get; set; } = default!;

  /// <summary>
  /// Display name for the file or Embed Html
  /// </summary>
  [Column(nameof(DocumentDisplayName))]
  [Display(Name = "Filename")]
  [Required(ErrorMessage = Constants.ErrorMessages.RequiredField)]
  [StringLength(Constants.FieldSizes.DocumentDisplayNameLength, ErrorMessage = Constants.ErrorMessages.StringLengthMax)]
  public string DocumentDisplayName { get; set; } = default!;

  /// <summary>
  /// mime type for uploaded file
  /// </summary>
  /// <remarks>If embed html, mime type is "Text/Html"</remarks>
  [Column(nameof(MimeType))]
  [Display(Name = "Mime Type")]
  [Required(ErrorMessage = Constants.ErrorMessages.RequiredField)]
  [StringLength(Constants.FieldSizes.NameLength, ErrorMessage = Constants.ErrorMessages.StringLengthMax)]
  public string MimeType { get; set; } = default!;
}
