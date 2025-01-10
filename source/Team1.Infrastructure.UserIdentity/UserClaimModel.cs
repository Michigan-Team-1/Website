using System.Security.Claims;
using Team1.Model.SerializedObjects;
using Team1.Model.UserIdentity;

namespace Team1.Infrastructure.UserIdentity;

public class UserClaimModel
{
  #region Claim Types

  public const string RoleDataClaimType = nameof(RoleData);
  public const string IPAddressClaimType = nameof(IpAddress);
  public const string OriginalSuffix = "Original";

  #endregion

  public bool IsAuthenticated { get { return UserId > 0; } }

  public int UserId { get; set; }
  public int UserIdOriginal { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string Email { get; set; }

  public string IpAddress { get; set; }

  public int UtcOffset { get; set; }

  // roles
  public bool IsAdmin { get; set; }

  public RoleData? RoleData { get; set; }

  // policies
  public bool HasAdminPolicy 
  {
    get
    {
      if (UserPolicies == null)
        return false;

      return UserPolicies.AnnouncementAddEditDelete || UserPolicies.EventAddEditDelete || UserPolicies.LocationAddEditDelete;
    }
  }
  public UserPolicies? UserPolicies { get; set; }

  protected void SetupPolicies()
  {
    UserPolicies = new UserPolicies();
    UserPolicies.CanImpersonate = IsAdmin;
    UserPolicies.UserAddEditDelete = IsAdmin;
    UserPolicies.UserProfileEdit = IsAuthenticated;// everyone can

    UserPolicies.AnnouncementAddEditDelete = IsAdmin;
    UserPolicies.CanApprovePicture = IsAdmin;
    UserPolicies.EventAddEditDelete = IsAdmin;
    UserPolicies.LocationAddEditDelete = IsAdmin;
    UserPolicies.PictureAddEditDelete = IsAuthenticated; // everyone can
  }

  /// <summary>
  /// Simplified claims for the client side.  
  /// </summary>
  /// <returns></returns>
  public List<Claim> GenerateClaimsFromUserClaimModel()
  {
    var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.NameIdentifier, UserId.ToString(), ClaimValueTypes.String),
        new Claim(nameof(User.UserId) + OriginalSuffix, UserIdOriginal.ToString(), ClaimValueTypes.Integer),
        new Claim(ClaimTypes.GivenName, FirstName, ClaimValueTypes.String),
        new Claim(ClaimTypes.Surname, LastName, ClaimValueTypes.String),
        new Claim(ClaimTypes.Email, Email, ClaimValueTypes.String),
        new Claim(ClaimTypes.Name, Email, ClaimValueTypes.String),
        new Claim(ClaimTypes.Role, IsAdmin ? "Admin" : "", ClaimValueTypes.String)
    };

    return claims;
  }
}

public class UserPolicies
{
  public UserPolicies() { }
  public UserPolicies(UserPolicies userPolicies)
  {
    var properties = typeof(UserPolicies).GetProperties();
    foreach (var item in properties)
    {
      item.SetValue(this, item.GetValue(userPolicies));
    }
  }

  public bool AnnouncementAddEditDelete { get; set; }
  public bool CanImpersonate { get; set; }
  public bool CanApprovePicture { get; set; }
  public bool EventAddEditDelete { get; set; }
  public bool LocationAddEditDelete { get; set; }
  public bool PictureAddEditDelete { get; set; }
  public bool UserAddEditDelete { get; set; }
  public bool UserProfileEdit { get; set; }
}
