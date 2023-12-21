using System.Security.Claims;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.SerializedObjects;
using Team1.Model.UserIdentity;

namespace Team1.Web.Common.UserIdentity
{
  /// <summary>
  /// Builds the user claim model
  /// </summary>
  public class UserClaimBuilder : UserClaimModel
  {
    private ClaimsPrincipal? _userPrincipal;
    private IEnumerable<Claim>? _claims;

    /// <summary>
    /// Constructs the UserClaimBuilder using the claimsPrincipal
    /// </summary>
    /// <param name="claimsUser">System.Security.Claims.ClaimsPrincipal</param>
    public UserClaimBuilder(ClaimsPrincipal? claimsUser)
    {
      if (claimsUser == null)
        return;

      _userPrincipal = claimsUser;
      _claims = _userPrincipal.Claims;
      Email = UserClaimString(ClaimTypes.Email);

      UserId = UserClaimInt(ClaimTypes.NameIdentifier);
      UserIdOriginal = UserClaimInt(nameof(User.UserId) + OriginalSuffix);
      FirstName = UserClaimString(ClaimTypes.GivenName);
      LastName = UserClaimString(ClaimTypes.Surname);

      var roleDataString = UserClaimString(RoleDataClaimType);
      if (!string.IsNullOrEmpty(roleDataString))
        RoleData = roleDataString.DeserializeJson<RoleData>();

      // roles
      IsAdmin = _userPrincipal.IsInRole(Role.Admin);

      // policies
      SetupPolicies();
    }

    /// <summary>
  /// Generates additional claims for the passed in user for the SERVER
  /// These claims the ones that identity server DOES NOT already create
  /// 
  /// Can setup impersonation if optional user is passed in.
  /// </summary>
  /// <param name="user">user to impersonate</param>
  /// <param name="roles">user's roles</param>
  /// <param name="ipAddress">user's ip Address</param>
  /// <param name="userToImpersonate">user to impersonate</param>
  /// <returns>claims list</returns>
  public static List<Claim> GenerateClaimsServer(User user, IList<Role>? roles, string? ipAddress, User? userToImpersonate = null)
  {
    var claimsBasedOnUser = userToImpersonate ?? user;
    var role = roles?.OrderBy(o => o.Level).FirstOrDefault();
    var claims = GenerateClaimsClient(user, userToImpersonate);

    if (ipAddress != null)
      claims.Add(new Claim(IPAddressClaimType, ipAddress, ClaimValueTypes.String));

    if (role != null && role.Data != null)
      claims.Add(new Claim(RoleDataClaimType, role.Data, ClaimValueTypes.String));

    return claims;
  }

  /// <summary>
  /// Generates additional claims for the passed in user for the CLIENT
  /// These claims the ones that identity server DOES NOT already create
  /// 
  /// Can setup impersonation if optional user is passed in.
  /// </summary>
  /// <param name="user">user to impersonate</param>
  /// <param name="userToImpersonate">user to impersonate</param>
  /// <returns>claims list</returns>
  public static List<Claim> GenerateClaimsClient(User user, User? userToImpersonate = null)
  {
    var claimsBasedOnUser = userToImpersonate ?? user;
    var claims = new List<Claim>()
          {
              new Claim(nameof(User.UserId) + OriginalSuffix, user.UserId.ToString(), ClaimValueTypes.Integer),
              new Claim(ClaimTypes.GivenName, claimsBasedOnUser.FirstName, ClaimValueTypes.String),
              new Claim(ClaimTypes.Surname, claimsBasedOnUser.LastName, ClaimValueTypes.String),
          };

    return claims;
  }

    #region helpers

    private string UserClaimString(string claimToFind)
    {
      var claim = _claims?.FirstOrDefault(w => w.Type == claimToFind);
      if (claim == null)
        return "";
      if (claim.ValueType != ClaimValueTypes.String)
        throw new InvalidCastException($"User claim '{claimToFind}' is not of type String.");

      return claim.Value;
    }

    private int UserClaimInt(string claimToFind)
    {
      var claim = _claims?.FirstOrDefault(w => w.Type == claimToFind);
      if (claim == null)
        return -1;
      if (claim.ValueType != ClaimValueTypes.Integer32 && claim.ValueType != ClaimValueTypes.Integer)
        throw new InvalidCastException($"User claim '{claimToFind}' is not of type int.");

      return int.Parse(claim.Value);
    }

    private bool UserClaimBoolean(string claimToFind)
    {
      var claim = _claims?.FirstOrDefault(w => w.Type == claimToFind);
      if (claim == null)
        return false;
      if (claim.ValueType != ClaimValueTypes.Boolean)
        throw new InvalidCastException($"User claim '{claimToFind}' is not of type boolean.");
      return bool.Parse(claim.Value);
    }

    #endregion
  }
}

