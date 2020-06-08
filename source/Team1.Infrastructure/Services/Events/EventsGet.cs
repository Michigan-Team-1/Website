using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team1.Infrastructure.Dtos;

namespace Team1.Infrastructure.Services.Events
{
    public class EventsGet : BaseService
    {
        /// <summary>
        /// Gets all events that the logged in user has access to.
        /// </summary>
        /// <param name="activeOnly">active only items</param>
        public async Task<IEnumerable<EventDto>> GetEvents(bool activeOnly)
        {
            return await (from u in db.EventsByFilter(UserPermissionService, activeOnly)
                          select new EventDto()
                          {
                              EventAlternateDate = u.EventAlternateDate,
                              EventDate = u.EventDate,
                              EventId = u.EventId,
                              Name = u.Name,
                              IsActive = !u.AuditFields.InactiveDateTime.HasValue,
                              EventLocations = u.EventLocations.Select(s=>new EventLocationDto() { EventId = s.EventId, LocationId = s.LocationId })
                          }).ToListAsync();
        }

        /// <summary>
        /// Gets all events for dashboard
        /// </summary>
        /// <param name="activeOnly">active only items</param>
        public async Task<IEnumerable<EventDto>> GetEventsForDashboard()
        {
            var compareDate = DateTime.Today.AddDays(-2);
            return await (from u in db.EventsByFilter(UserPermissionService, true)
                          where u.EventDate >= compareDate
                          orderby u.EventDate
                          select new EventDto()
                          {
                              EventAlternateDate = u.EventAlternateDate,
                              EventDate = u.EventDate,
                              EventId = u.EventId,
                              Name = u.Name,
                              IsActive = !u.AuditFields.InactiveDateTime.HasValue,
                              Locations = u.EventLocations.Select(s=>new LocationDto()
                              {
                                  LocationId = s.LocationId,
                                  LocationName = s.Location.LocationName,
                                  FAAWaiver = s.Location.FAAWaiver,
                                  AddressObj = new AddressObjDto()
                                  {
                                      Address1 = s.Location.AddressObj.Address1,
                                      Address2 = s.Location.AddressObj.Address2,
                                      Address3 = s.Location.AddressObj.Address3,
                                      City = s.Location.AddressObj.City,
                                      CountryId = s.Location.AddressObj.CountryId,
                                      Country = new CountryDto()
                                      {
                                          Alpha3 = s.Location.AddressObj.Country.Alpha3,
                                          Name = s.Location.AddressObj.Country.Name,
                                      },
                                      GoverningDistrictName = s.Location.AddressObj.GoverningDistrict.Name,
                                      PostalCode = s.Location.AddressObj.PostalCode,
                                  },
                              })
                          }).ToListAsync();
        }

        /// <summary>
        /// Gets a event
        /// </summary>
        /// <param name="id">id to get</param>
        /// <returns>dto</returns>
        public async Task<EventDto> GetEvent(int id)
        {
            return await (from u in db.RoleRestrictedEvents(UserPermissionService)
                          where u.EventId == id
                          select new EventDto()
                          {
                              EventAlternateDate = u.EventAlternateDate,
                              EventDate = u.EventDate,
                              EventId = u.EventId,
                              Name = u.Name,
                              IsActive = !u.AuditFields.InactiveDateTime.HasValue,
                              EventLocations = u.EventLocations.Select(s => new EventLocationDto()
                              { EventId = s.EventId, LocationId = s.LocationId })
                              }).SingleOrDefaultAsync();
        }
    }
}
