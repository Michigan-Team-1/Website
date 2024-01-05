using Microsoft.EntityFrameworkCore;
using Team1.Infrastructure.Dtos;

namespace Team1.Infrastructure.Services.Locations
{
  public class LocationsGet : BaseService
  {
    /// <summary>
    /// Gets all locations that the logged in user has access to.
    /// </summary>
    /// <param name="activeOnly">active only items</param>
    public async Task<List<LocationDto>> GetLocations(bool activeOnly)
    {
      return await (from x in db.LocationsByFilter(UserPermissionService, activeOnly)
              //join createdBy in db.Users on x.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
              //from createdBy in ljCreatedBy.DefaultIfEmpty()
              //join updatedBy in db.Users on x.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
              //from updatedBy in ljUpdatedBy.DefaultIfEmpty()
              select new LocationDto()
              {
                FAAWaiver = x.FAAWaiver,
                LocationDescription = x.LocationDescription,
                LocationId = x.LocationId,
                LocationName = x.LocationName,
                //AddressObj = new AddressObjDto()
                //{
                //  Address1 = x.AddressObj.Address1,
                //  Address2 = x.AddressObj.Address2,
                //  Address3 = x.AddressObj.Address3,
                //  City = x.AddressObj.City,
                //  CountryId = x.AddressObj.CountryId,
                //  Country = new CountryDto()
                //  {
                //    Alpha2 = x.AddressObj.Country.Alpha2,
                //    Alpha3 = x.AddressObj.Country.Alpha3,
                //    CountryId = x.AddressObj.Country.CountryId,
                //    GoverningDistrictName = x.AddressObj.Country.GoverningDistrictName,
                //    Name = x.AddressObj.Country.Name,
                //    PostalCodeMask = x.AddressObj.Country.PostalCodeMask,
                //  },
                //  GoverningDistrictId = x.AddressObj.GoverningDistrictId,
                //  PostalCode = x.AddressObj.PostalCode,
                //},
                AuditFieldsDto = new AuditFieldsDto()
                {
                  CreatedDateTime = x.AuditFields.CreatedDateTime,
                  UpdatedDateTime = x.AuditFields.UpdatedDateTime,
                  //CreatedByName = createdBy != null ? string.Concat(createdBy.FirstName, " ", createdBy.LastName) : " - ",
                  //UpdatedByName = updatedBy != null ? string.Concat(updatedBy.FirstName, " ", updatedBy.LastName) : " - "
                },
                IsActive = !x.AuditFields.InactiveDateTime.HasValue,
              }).ToListAsync();
    }

    /// <summary>
    /// Gets all locations that the logged in user has access to.
    /// </summary>
    /// <param name="activeOnly">active only items</param>
    public Task<List<Dtos.Helpers.SelectOptionDto<int>>> GetLocationsForSelection(bool activeOnly)
    {
      return (from x in db.LocationsByFilter(UserPermissionService, activeOnly)
              select new Dtos.Helpers.SelectOptionDto<int>()
              {
                Value = x.LocationId,
                Text = x.LocationName,
              }).ToListAsync();
    }

    /// <summary>
    /// Gets a location
    /// </summary>
    /// <param name="id">id to get</param>
    /// <returns>dto</returns>
    public Task<LocationDto?> GetLocation(int id)
    {
      return (from x in db.RoleRestrictedLocations(UserPermissionService)
              join createdBy in db.Users on x.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
              from createdBy in ljCreatedBy.DefaultIfEmpty()
              join updatedBy in db.Users on x.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
              from updatedBy in ljUpdatedBy.DefaultIfEmpty()
              where x.LocationId == id
              select new LocationDto()
              {
                FAAWaiver = x.FAAWaiver,
                LocationDescription = x.LocationDescription,
                LocationId = x.LocationId,
                LocationName = x.LocationName,
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
                AuditFieldsDto = new AuditFieldsDto()
                {
                  CreatedDateTime = x.AuditFields.CreatedDateTime,
                  UpdatedDateTime = x.AuditFields.UpdatedDateTime,
                  CreatedByName = createdBy != null ? string.Concat(createdBy.FirstName, " ", createdBy.LastName) : " - ",
                  UpdatedByName = updatedBy != null ? string.Concat(updatedBy.FirstName, " ", updatedBy.LastName) : " - "
                },
                IsActive = !x.AuditFields.InactiveDateTime.HasValue,
              }).SingleOrDefaultAsync();
    }
  }
}
