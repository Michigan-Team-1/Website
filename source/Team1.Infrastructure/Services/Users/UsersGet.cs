using Microsoft.EntityFrameworkCore;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Users;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.Enums;

namespace Team1.Infrastructure.Services.Users;

public class UsersGet : BaseService
{
  /// <summary>
  /// Gets all users that the logged in user has access to.
  /// </summary>
  /// <param name="activeOnly">active only items</param>
  public Task<List<UserDto>> GetUsers(bool activeOnly)
  {
    var query = db.UsersByFilter(UserPermissionService, activeOnly);

    if (UserPermissionService.UserPolicies!.UserAddEditDelete)
    {
      return (from u in query
              join createdBy in db.Users on u.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
              from createdBy in ljCreatedBy.DefaultIfEmpty()
              join updatedBy in db.Users on u.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
              from updatedBy in ljUpdatedBy.DefaultIfEmpty()
              select new UserDto()
              {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                IsActive = !u.AuditFields.InactiveDateTime.HasValue,
                IsLoginEnabled = u.IsLoginEnabled,
                CertificationLevel = u.CertificationLevel,
                PaidThroughYear = u.PaidThroughYear,
                UserMemberTypes = u.UserMemberTypes.Select(s => new UserMemberTypeDto() { MemberTypeId = s.MemberTypeId, UserId = s.UserId }).ToList(),
                Roles = u.UserRoles.Select(r => new RoleDto()
                {
                  RoleId = r.RoleId,
                  Level = r.Role.Level,
                  Name = r.Role.Name,
                  Type = r.Role.Type,
                }).ToList(),
                AuditFieldsDto = new AuditFieldsDto()
                {
                  CreatedDateTime = u.AuditFields.CreatedDateTime,
                  UpdatedDateTime = u.AuditFields.UpdatedDateTime,
                  CreatedByName = createdBy != null ? string.Concat(createdBy.FirstName, " ", createdBy.LastName) : " - ",
                  UpdatedByName = updatedBy != null ? string.Concat(updatedBy.FirstName, " ", updatedBy.LastName) : " - "
                },
              }).ToListAsync();
    }

    if (UserPermissionService.UserClaimModel!.IsAuthenticated)
      return query.Select(u => new UserDto()
      {
        UserId = u.UserId,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        IsActive = !u.AuditFields.InactiveDateTime.HasValue,
        CertificationLevel = u.CertificationLevel,
        UserMemberTypes = u.UserMemberTypes.Select(s => new UserMemberTypeDto() { MemberTypeId = s.MemberTypeId, UserId = s.UserId }).ToList(),
      }).ToListAsync();

    return query.Select(u => new UserDto()
    {
      UserId = u.UserId,
      FirstName = u.FirstName,
      LastName = u.LastName,
      IsActive = !u.AuditFields.InactiveDateTime.HasValue,
      CertificationLevel = u.CertificationLevel,
      UserMemberTypes = u.UserMemberTypes.Select(s => new UserMemberTypeDto() { MemberTypeId = s.MemberTypeId, UserId = s.UserId }).ToList(),
    }).ToListAsync();
  }

  /// <summary>
  /// Gets Board of Directors
  /// </summary>
  public Task<List<UserDto>> GetBoardOfDirectors()
  {
    var userMemberTypes = new List<MemberTypeEnum>() { MemberTypeEnum.Prefect, MemberTypeEnum.Secretary, MemberTypeEnum.Treasurer, MemberTypeEnum.VicePrefect };
    return (from u in db.UsersByFilter(UserPermissionService, true)
            where !u.AuditFields.InactiveDateTime.HasValue && u.UserMemberTypes.Any(s => userMemberTypes.Contains(s.MemberTypeId))
            orderby u.UserMemberTypes.Select(s => s.MemberTypeId).FirstOrDefault()
            select new UserDto()
            {
              UserId = u.UserId,
              FirstName = u.FirstName,
              LastName = u.LastName,
              Email = u.Email,
              PhoneNumber = u.PhoneNumber,
              CertificationLevel = u.CertificationLevel,
              TripoliNumber = u.TripoliNumber,
              NarNumber = u.NarNumber,
              UserMemberTypes = u.UserMemberTypes.Select(s => new UserMemberTypeDto() { MemberTypeId = s.MemberTypeId, UserId = s.UserId }).ToList(),
            }).ToListAsync();
  }

  /// <summary>
  /// Gets all General roles the logged in user has access to
  /// </summary>
  public Task<List<RoleDto>> GetGrantableRoles()
  {
    return (from r in db.RoleRestrictedRoles(UserPermissionService, true)
            orderby r.Level
            select new RoleDto()
            {
              RoleId = r.RoleId,
              Level = r.Level,
              Name = r.Name,
              Type = r.Type
            }).ToListAsync();
  }

  /// <summary>
  /// Gets a user
  /// </summary>
  /// <param name="userId">userId to get</param>
  /// <returns>userDto</returns>
  public Task<UserDto?> GetUser(int userId)
  {
    return (from u in db.RoleRestrictedUsers(UserPermissionService)
            join createdBy in db.Users on u.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
            from createdBy in ljCreatedBy.DefaultIfEmpty()
            join updatedBy in db.Users on u.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
            from updatedBy in ljUpdatedBy.DefaultIfEmpty()
            where u.UserId == userId
            select new UserDto()
            {
              UserId = u.UserId,
              FirstName = u.FirstName,
              LastName = u.LastName,
              Email = u.Email,
              PhoneNumber = u.PhoneNumber,
              BirthDate = u.BirthDate,
              IsActive = !u.AuditFields.InactiveDateTime.HasValue,
              IsLoginEnabled = u.IsLoginEnabled,
              CertificationLevel = u.CertificationLevel,
              TripoliNumber = u.TripoliNumber,
              NarNumber = u.NarNumber,
              PaidThroughYear = u.PaidThroughYear,
              MobileCarrierId = u.MobileCarrierId,
              EmailConfirmed = u.EmailConfirmed,
              UserMemberTypes = u.UserMemberTypes.Select(s => new UserMemberTypeDto() { MemberTypeId = s.MemberTypeId, UserId = s.UserId }).ToList(),
              UserRoles = u.UserRoles.Select(r => new UserRoleDto()
              {
                RoleId = r.RoleId,
                UserId = r.UserId
              }).ToList(),
              Addresses = u.Addresses.Where(w => !w.AuditFields.InactiveDateTime.HasValue).Select(x => new AddressDto()
              {
                AddressId = x.AddressId,
                IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                AddressObj = new AddressObjDto()
                {
                  Address1 = x.AddressObj.Address1,
                  Address2 = x.AddressObj.Address2,
                  Address3 = x.AddressObj.Address3,
                  City = x.AddressObj.City,
                  CountryId = x.AddressObj.CountryId,
                  GoverningDistrictId = x.AddressObj.GoverningDistrictId,
                  PostalCode = x.AddressObj.PostalCode,
                },
                UserId = x.UserId
              }).ToList(),
              AuditFieldsDto = new AuditFieldsDto()
              {
                CreatedDateTime = u.AuditFields.CreatedDateTime,
                UpdatedDateTime = u.AuditFields.UpdatedDateTime,
                CreatedByName = createdBy != null ? string.Concat(createdBy.FirstName, " ", createdBy.LastName) : " - ",
                UpdatedByName = updatedBy != null ? string.Concat(updatedBy.FirstName, " ", updatedBy.LastName) : " - "
              },
            }).SingleOrDefaultAsync();
  }

  /// <summary>
  /// Gets a user profile
  /// </summary>
  /// <param name="userId">userId to get</param>
  /// <returns>userDto</returns>
  public Task<UserDto?> GetUserProfile()
  {
    return (from u in db.RoleRestrictedUsers(UserPermissionService)
            join createdBy in db.Users on u.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
            from createdBy in ljCreatedBy.DefaultIfEmpty()
            join updatedBy in db.Users on u.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
            from updatedBy in ljUpdatedBy.DefaultIfEmpty()
            where u.UserId == UserPermissionService.UserClaimModel!.UserId
            select new UserDto()
            {
              UserId = u.UserId,
              FirstName = u.FirstName,
              LastName = u.LastName,
              Email = u.Email,
              PhoneNumber = u.PhoneNumber,
              BirthDate = u.BirthDate,
              TwoFactorEnabled = u.TwoFactorEnabled,
              CertificationLevel = u.CertificationLevel,
              TripoliNumber = u.TripoliNumber,
              NarNumber = u.NarNumber,
              MobileCarrierId = u.MobileCarrierId,
              PaidThroughYear = u.PaidThroughYear,
              Addresses = u.Addresses.Where(w => !w.AuditFields.InactiveDateTime.HasValue).Select(x => new AddressDto()
              {
                AddressId = x.AddressId,
                IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                AddressObj = new AddressObjDto()
                {
                  Address1 = x.AddressObj.Address1,
                  Address2 = x.AddressObj.Address2,
                  Address3 = x.AddressObj.Address3,
                  City = x.AddressObj.City,
                  CountryId = x.AddressObj.CountryId,
                  Country = new CountryDto()
                  {
                    Alpha2 = x.AddressObj.Country.Alpha2,
                    Alpha3 = x.AddressObj.Country.Alpha3,
                    CountryId = x.AddressObj.Country.CountryId,
                    GoverningDistrictName = x.AddressObj.Country.GoverningDistrictName,
                    Name = x.AddressObj.Country.Name,
                    PostalCodeMask = x.AddressObj.Country.PostalCodeMask,
                  },
                  GoverningDistrictId = x.AddressObj.GoverningDistrictId,
                  PostalCode = x.AddressObj.PostalCode,
                },
                UserId = x.UserId
              }).ToList(),
              AuditFieldsDto = new AuditFieldsDto()
              {
                CreatedDateTime = u.AuditFields.CreatedDateTime,
                UpdatedDateTime = u.AuditFields.UpdatedDateTime,
                CreatedByName = createdBy != null ? string.Concat(createdBy.FirstName, " ", createdBy.LastName) : " - ",
                UpdatedByName = updatedBy != null ? string.Concat(updatedBy.FirstName, " ", updatedBy.LastName) : " - "
              },
            }).SingleOrDefaultAsync();
  }

  /// <summary>
  /// Gets all user roles
  /// </summary>
  /// <param name="userId">user id to get roles for</param>
  /// <param name="ups">UserPermissionService</param>
  /// <param name="withRoleData">returns role data with the dto if true</param>
  public Task<List<RoleDto>> GetUserRoles(int userId, UserPermissionService ups, bool withRoleData = false)
  {
    return (from u in db.Users
            join ur in db.UserRoles on u.UserId equals ur.UserId
            join r in db.RoleRestrictedRoles(UserPermissionService, false) on ur.RoleId equals r.RoleId
            where !u.AuditFields.InactiveDateTime.HasValue && u.UserId == userId
            orderby r.Level
            select new RoleDto()
            {
              RoleId = r.RoleId,
              Level = r.Level,
              Name = r.Name,
              Type = r.Type,
              Data = withRoleData ? r.Data : null
            }).ToListAsync();
  }


    public Task<UserDto?> GetSecretary()
    {
        return (from u in db.UsersByFilter(UserPermissionService, true)
                where !u.AuditFields.InactiveDateTime.HasValue
                   && u.UserMemberTypes.Any(m => m.MemberTypeId == MemberTypeEnum.Secretary)
                select new UserDto()
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,

                    Addresses = u.Addresses
                        .Where(a => !a.AuditFields.InactiveDateTime.HasValue)
                        .Select(a => new AddressDto()
                        {
                            AddressId = a.AddressId,
                            UserId = a.UserId,
                            AddressObj = new AddressObjDto()
                            {
                                Address1 = a.AddressObj.Address1,
                                Address2 = a.AddressObj.Address2,
                                Address3 = a.AddressObj.Address3,
                                City = a.AddressObj.City,
                                GoverningDistrictId = a.AddressObj.GoverningDistrictId,
                                PostalCode = a.AddressObj.PostalCode,
                                CountryId = a.AddressObj.CountryId,

                                GoverningDistrictName = a.AddressObj.GoverningDistrict != null
                                ? a.AddressObj.GoverningDistrict.Name
                                : null,
                            }
                        }).ToList()
                }).SingleOrDefaultAsync();
    }


}
