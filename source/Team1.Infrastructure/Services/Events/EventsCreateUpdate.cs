using Microsoft.EntityFrameworkCore;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.UserIdentity;
using Team1.Model;
using Team1.Model.OwnedTypes;

namespace Team1.Infrastructure.Services.Events;

public class EventsCreateUpdate : BaseService
{
  public EventsCreateUpdate()
  {
  }

  /// <summary>
  /// Saves a event
  /// </summary>
  /// <param name="dto">dto to save</param>
  /// <returns>updated dto object</returns>
  public async Task<BaseServiceResponse<T>> SaveEvent<T>(T dto) where T : EventDto
  {
    var response = new BaseServiceResponse<T>(dto);
    // make sure user has access to this
    if (!UserPermissionService.UserClaimModel.UserPolicies!.EventAddEditDelete)
    {
      response.Message = "You are not authorized to add/edit a event.";
      response.Status = System.Net.HttpStatusCode.Unauthorized;
      return response;
    }

    if (dto.EventLocations == null || dto.EventLocations.Count(w => !w.IsDeleted) == 0)
    {
      response.Message = "You must have at least one location for an event.";
      response.Status = System.Net.HttpStatusCode.BadRequest;
      return response;
    }

    // nothing changed, return
    if (!dto.IsUpdated)
      return response;

    var timestamp = DateTime.UtcNow;
    Team1.Model.Event? dbObj;
    var isNew = dto.EventId == 0;
    if (isNew)
    {
      dbObj = new Model.Event()
      {
        AuditFields = new AuditFields(UserPermissionService.UserClaimModel.UserId, timestamp),
      };
      db.Events.Add(dbObj);
      dto.AuditFieldsDto.SetCreated(dbObj.AuditFields, UserPermissionService.FirstLastName);
    }
    else
    {
      dbObj = await db.RoleRestrictedEvents(UserPermissionService, false).Include(i => i.EventLocations).SingleOrDefaultAsync(w => w.EventId == dto.EventId);
      if (dbObj == null)
      {
        response.Message = "You are not authorized to edit this event.";
        response.Status = System.Net.HttpStatusCode.Unauthorized;
        return response;
      }
    }

    dbObj.EventAlternateDate = dto.EventAlternateDate;
    dbObj.EventDate = dto.EventDate;
    dbObj.Name = dto.Name;

    if (dbObj.EventLocations == null)
      dbObj.EventLocations = new List<EventLocation>();
    var currentEventLocations = dbObj.EventLocations.ToList();
    foreach (var item in dto.EventLocations)
    {
      var eventLocation = currentEventLocations.FirstOrDefault(w => w.LocationId == item.LocationId);
      if (item.IsDeleted)
      {
        db.EventLocations.Remove(eventLocation);
        currentEventLocations.Remove(eventLocation);
      }
      else if (eventLocation == null)
        dbObj.EventLocations.Add(new EventLocation() { LocationId = item.LocationId });
      else
        currentEventLocations.Remove(eventLocation);
    }
    // remove any remaining event locations
    foreach (var item in currentEventLocations)
    {
      db.EventLocations.Remove(item);
    }

    dto.EventLocations = dto.EventLocations.Where(w => !w.IsDeleted).ToList();

    dbObj.AuditFields.SetActiveInactive(dto.IsActive, UserPermissionService.UserClaimModel.UserId, timestamp);

    dbObj.AuditFields.SetUpdated(UserPermissionService.UserClaimModel.UserId, timestamp);

    await db.SaveChangesAsync();

    if (isNew)
      dto.EventId = dbObj.EventId;

    // when EventId is assigned, then it knows the event location has been saved to the DB.
    dto.EventLocations.ForEach(s => s.EventId = dbObj.EventId);

    dto.IsUpdated = false;
    dto.AuditFieldsDto.SetUpdated(dbObj.AuditFields, UserPermissionService.FirstLastName);

    return response;
  }

  /// <summary>
  /// Deletes or inactivates a event (based on client needs)
  /// </summary>
  /// <param name="id">id to delete</param>
  public async Task<BaseServiceResponse<int>> DeleteEvent(int id)
  {
    var response = new BaseServiceResponse<int>(id);

    // make sure user has access to this
    if (!UserPermissionService.UserClaimModel.UserPolicies.EventAddEditDelete)
    {
      response.Message = "You are not authorized to add/edit a event.";
      response.Status = System.Net.HttpStatusCode.Unauthorized;
      return response;
    }

    // make sure logged in user has permission to the requested user
    var dbObj = await db.RoleRestrictedEvents(UserPermissionService, false).FirstOrDefaultAsync(w => w.EventId == id);
    if (dbObj == null)
    {
      response.Status = System.Net.HttpStatusCode.BadRequest;
      return response;
    }
    // delete (need to delete owned types and dependent tables)
    //db.Remove(dbObj);
    //db.Remove(dbObj.AuditFields);
    // inactivate
    dbObj.AuditFields.SetActiveInactive(false, UserPermissionService.UserClaimModel.UserId, DateTime.UtcNow);

    await db.SaveChangesAsync();

    return response;
  }
}
