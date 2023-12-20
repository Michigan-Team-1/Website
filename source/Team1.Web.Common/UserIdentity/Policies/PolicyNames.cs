namespace Team1.Web.Common.UserIdentity.Policies
{
  /// <summary>
  /// Names of all policies within the system
  /// </summary>
  public static class PolicyNames
  {
    public const string AnnouncementAddEditDelete = nameof(Policies.AnnouncementAddEditDelete);
    public const string CanApprovePicture = nameof(Policies.CanApprovePicture);
    public const string CanImpersonate = nameof(Policies.CanImpersonate);
    public const string EventAddEditDelete = nameof(Policies.TaskAddEditDelete);
    public const string LocationAddEditDelete = nameof(Policies.TaskAddEditDelete);
    public const string PictureAddEditDelete = nameof(Policies.PictureAddEditDelete);
    public const string TaskAddEditDelete = nameof(Policies.TaskAddEditDelete);
    public const string UserAddEditDelete = nameof(Policies.UserAddEditDelete);
    public const string UserProfileEdit = nameof(Policies.UserProfileEdit);
  }
}
