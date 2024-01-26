using Microsoft.EntityFrameworkCore;
using Team1.Infrastructure.Dtos;

namespace Team1.Infrastructure.Services.Events
{
  public class EventsGet : BaseService
  {
    /// <summary>
    /// Gets all events that the logged in user has access to.
    /// </summary>
    /// <param name="activeOnly">active only items</param>
    public Task<List<EventDto>> GetEvents(bool activeOnly)
    {
      return (from e in db.EventsByFilter(UserPermissionService, activeOnly)
              join createdBy in db.Users on e.AuditFields.CreatedById equals createdBy.UserId into ljCreatedBy
              from createdBy in ljCreatedBy.DefaultIfEmpty()
              join updatedBy in db.Users on e.AuditFields.UpdatedById equals updatedBy.UserId into ljUpdatedBy
              from updatedBy in ljUpdatedBy.DefaultIfEmpty()
              select new EventDto()
              {
                EventAlternateDate = e.EventAlternateDate,
                EventDate = e.EventDate,
                EventId = e.EventId,
                Name = e.Name,
                IsActive = !e.AuditFields.InactiveDateTime.HasValue,
                EventLocations = e.EventLocations.Select(s => new EventLocationDto() { EventId = s.EventId, LocationId = s.LocationId }).ToList(),
                AuditFieldsDto = new AuditFieldsDto()
                {
                  CreatedDateTime = e.AuditFields.CreatedDateTime,
                  UpdatedDateTime = e.AuditFields.UpdatedDateTime,
                  CreatedByName = createdBy != null ? string.Concat(createdBy.FirstName, " ", createdBy.LastName) : " - ",
                  UpdatedByName = updatedBy != null ? string.Concat(updatedBy.FirstName, " ", updatedBy.LastName) : " - "
                },
              }).ToListAsync();
    }

    /// <summary>
    /// Gets all events for dashboard
    /// </summary>
    /// <param name="activeOnly">active only items</param>
    public Task<List<EventDto>> GetEventsForHome()
    {
      var compareDate = DateTime.Today.AddDays(-2);
      return (from e in db.EventsByFilter(UserPermissionService, true)
              where e.EventDate >= compareDate
              orderby e.EventDate
              select new EventDto()
              {
                EventAlternateDate = e.EventAlternateDate,
                EventDate = e.EventDate,
                EventId = e.EventId,
                Name = e.Name,
                IsActive = !e.AuditFields.InactiveDateTime.HasValue,
                Locations = e.EventLocations.Select(s => new LocationDto()
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
                }).ToList()
              }).ToListAsync();
    }

    /// <summary>
    /// Gets an event
    /// </summary>
    /// <param name="id">id to get</param>
    /// <returns>dto</returns>
    public Task<EventDto?> GetEvent(int id)
    {
      return (from u in db.RoleRestrictedEvents(UserPermissionService)
              where u.EventId == id
              select new EventDto()
              {
                EventAlternateDate = u.EventAlternateDate,
                EventDate = u.EventDate,
                EventId = u.EventId,
                Name = u.Name,
                IsActive = !u.AuditFields.InactiveDateTime.HasValue,
                EventLocations = u.EventLocations.Select(s => new EventLocationDto()
                { EventId = s.EventId, LocationId = s.LocationId }).ToList()
              }).SingleOrDefaultAsync();
    }
  }
}
