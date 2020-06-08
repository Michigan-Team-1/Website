using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;

namespace Team1.Infrastructure.Services.Locations
{
    public class LocationsGet : BaseService
    {
        /// <summary>
        /// Gets all locations that the logged in user has access to.
        /// </summary>
        /// <param name="activeOnly">active only items</param>
        public async Task<IEnumerable<LocationDto>> GetLocations(bool activeOnly)
        {
            return await (from x in db.LocationsByFilter(UserPermissionService, activeOnly)
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
                              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                          }).ToListAsync();
        }

        /// <summary>
        /// Gets all locations that the logged in user has access to.
        /// </summary>
        /// <param name="activeOnly">active only items</param>
        public async Task<IEnumerable<Dtos.Helpers.SelectOptionDto<int>>> GetLocationsForSelection(bool activeOnly)
        {
            return await (from x in db.LocationsByFilter(UserPermissionService, activeOnly)
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
        public async Task<LocationDto> GetLocation(int id)
        {
            return await (from x in db.RoleRestrictedLocations(UserPermissionService)
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
                              IsActive = !x.AuditFields.InactiveDateTime.HasValue,
                          }).SingleOrDefaultAsync();
        }
    }
}
