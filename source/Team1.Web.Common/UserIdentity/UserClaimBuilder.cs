using System.Security.Claims;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Users;
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
    #region Claim Types

    public const string UserIdClaimType = nameof(UserId);
    public const string EmailClaimType = nameof(Email);
    public const string FirstNameClaimType = nameof(FirstName);
    public const string LastNameClaimType = nameof(LastName);
    public const string RoleDataClaimType = nameof(RoleData);
    public const string IPAddressClaimType = nameof(IpAddress);
    public const string OriginalSuffix = "Original";

    #endregion

    private ClaimsPrincipal _userPrincipal;
    private IEnumerable<Claim> _claims;

    /// <summary>
    /// Constructs the UserClaimBuilder using the claimsPrincipal
    /// </summary>
    /// <param name="claimsUser">System.Security.Claims.ClaimsPrincipal</param>
    public UserClaimBuilder(ClaimsPrincipal claimsUser)
    {
      _userPrincipal = claimsUser;
      if (claimsUser == null)
        return;

      _claims = _userPrincipal.Claims;
      Email = UserClaimString(ClaimTypes.Email);

      UserId = UserClaimInt(ClaimTypes.NameIdentifier);
      UserIdOriginal = UserClaimInt(nameof(User.UserId) + OriginalSuffix);
      FirstName = UserClaimString(ClaimTypes.GivenName);
      LastName = UserClaimString(ClaimTypes.Surname);

      var roleDataString = UserClaimString(RoleDataClaimType);
      if (!string.IsNullOrEmpty(roleDataString))
        RoleData = roleDataString.DeserializeJsonNet<RoleData>();

      // roles
      IsAdmin = _userPrincipal.IsInRole(Role.Admin);

      // policies
      SetupPolicies();
    }

    /// <summary>
    /// Generates claims for the passed in user.
    /// 
    /// Can setup impersonation if optional user is passed in.
    /// </summary>
    /// <param name="user">user to impersonate</param>
    /// <param name="roles">user's roles</param>
    /// <param name="ipAddress">user's ip Address</param>
    /// <param name="userToImpersonate">user to impersonate</param>
    /// <returns>claims list</returns>
    public static List<Claim> GenerateClaims(User user, IList<RoleDto> roles, string ipAddress, User userToImpersonate = null)
    {
      var claimsBasedOnUser = userToImpersonate ?? user;
      var role = roles.OrderBy(o => o.Level).FirstOrDefault();
      var timestamp = DateTime.UtcNow;
      var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Iat, timestamp.ToString(), ClaimValueTypes.DateTime),
                new Claim(JwtRegisteredClaimNames.Iat + OriginalSuffix, timestamp.ToString(), ClaimValueTypes.DateTime),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String),
                new Claim(JwtRegisteredClaimNames.Sid, user.UserId.ToString(), ClaimValueTypes.Integer),
                new Claim(UserIdClaimType, claimsBasedOnUser.UserId.ToString(), ClaimValueTypes.Integer),
                new Claim(JwtRegisteredClaimNames.Sub, claimsBasedOnUser.Email, ClaimValueTypes.String),
                new Claim(JwtRegisteredClaimNames.GivenName, claimsBasedOnUser.FirstName, ClaimValueTypes.String),
                new Claim(JwtRegisteredClaimNames.FamilyName, claimsBasedOnUser.LastName, ClaimValueTypes.String),

                new Claim(IPAddressClaimType, ipAddress, ClaimValueTypes.Integer),
            };

      if (role != null && role.Data != null)
        claims.Add(new Claim(RoleDataClaimType, role.Data, ClaimValueTypes.String));

      if (roles != null && roles.Count > 0)
      {
        foreach (var item in roles)
        {
          claims.Add(new Claim(ClaimTypes.Role, item.Name));
        }
      }

      return claims;
    }

    /// <summary>
    /// Impersonate an user by setting claims accordingly
    /// </summary>
    /// <param name="claims">current list of claims</param>
    /// <param name="userDto">user to impersonate</param>
    /// <returns>claims list</returns>
    public static List<Claim> ImpersonateUser(List<Claim> claims, UserDto userDto)
    {
      var userIdClaim = claims.FirstOrDefault(w => w.Type == UserIdClaimType);
      var emailClaim = claims.FirstOrDefault(w => w.Type == JwtRegisteredClaimNames.Sub);
      var firstNameClaim = claims.FirstOrDefault(w => w.Type == JwtRegisteredClaimNames.GivenName);
      var lastNameClaim = claims.FirstOrDefault(w => w.Type == JwtRegisteredClaimNames.FamilyName);

      if (userIdClaim != null)
        claims.Remove(userIdClaim);
      if (emailClaim != null)
        claims.Remove(emailClaim);
      if (firstNameClaim != null)
        claims.Remove(firstNameClaim);
      if (lastNameClaim != null)
        claims.Remove(lastNameClaim);

      claims.Add(new Claim(UserIdClaimType, userDto.UserId.ToString(), ClaimValueTypes.Integer));
      claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userDto.Email, ClaimValueTypes.String));
      claims.Add(new Claim(JwtRegisteredClaimNames.GivenName, userDto.FirstName, ClaimValueTypes.String));
      claims.Add(new Claim(JwtRegisteredClaimNames.FamilyName, userDto.LastName, ClaimValueTypes.String));

      return claims;
    }

    #region helpers

    private string UserClaimString(string claimToFind)
    {
      var claim = _claims.FirstOrDefault(w => w.Type == claimToFind);
      if (claim == null)
        return "";
      if (claim.ValueType != ClaimValueTypes.String)
        throw new InvalidCastException($"User claim '{claimToFind}' is not of type String.");

      return claim.Value;
    }

    private int UserClaimInt(string claimToFind)
    {
      var claim = _claims.FirstOrDefault(w => w.Type == claimToFind);
      if (claim == null)
        return -1;
      if (claim.ValueType != ClaimValueTypes.Integer32 && claim.ValueType != ClaimValueTypes.Integer)
        throw new InvalidCastException($"User claim '{claimToFind}' is not of type int.");

      return int.Parse(claim.Value);
    }

    private bool UserClaimBoolean(string claimToFind)
    {
      var claim = _claims.FirstOrDefault(w => w.Type == claimToFind);
      if (claim == null)
        return false;
      if (claim.ValueType != ClaimValueTypes.Boolean)
        throw new InvalidCastException($"User claim '{claimToFind}' is not of type boolean.");
      return bool.Parse(claim.Value);
    }

    #endregion
  }
}

