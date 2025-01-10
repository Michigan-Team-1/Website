namespace Team1.Model.Constants;

public class ErrorMessages
{
  public const string RequiredField = "{0} is required.";
  public const string FVRequiredField = "{PropertyName} is required.";

  public const string StringLengthMax = "{0} must be < {1} characters.";
  public const string FVStringLengthMax = "{PropertyName} must be < {MaxLength} characters.";

  public const string StringLengthMinMax = "{0} must be > {2} and < {1} characters.";
  public const string FVStringLengthMinMax = "{PropertyName} must be > {MinLength} and < {MaxLength} characters.";

  public const string RangeMinMax = "{0} must be from {1} to {2}.";
  public const string FVRangeMinMax = "{PropertyName} must be from {MinLength} to {MaxLength}.";

  public const string FVGreaterThan = "{PropertyName} must be greater than {ComparisonValue}";

  public const string FVNoSpaces = "{PropertyName} must NOT contain any spaces.";
}
