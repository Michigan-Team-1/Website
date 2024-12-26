using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Helpers;
using Team1.Infrastructure.Dtos.Users;
using Team1.Web.Client.Helpers;

namespace Team1.Web.Client.Services;

public class AuthorizedClient
{
  protected readonly HttpClient _httpClient;
  protected readonly ServiceResponseHandler _serviceResponseHandler;

  public AuthorizedClient(HttpClient httpClient, ServiceResponseHandler serviceResponseHandler)
  {
    _httpClient = httpClient;
    _serviceResponseHandler = serviceResponseHandler;
  }

  public async Task<List<AnnouncementDto>?> GetAnnouncements()
  {
    try
    {
      var response = await _httpClient.GetAsync("api/Announcements");
      return await _serviceResponseHandler.HandleJsonResponse<List<AnnouncementDto>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<AnnouncementDto>();
  }

  public async Task<AnnouncementDto?> SaveAnnouncement(AnnouncementDto dto)
  {
    try
    {
      Task<HttpResponseMessage>? saveTask;
      var content = _serviceResponseHandler.BuildJsonContent(dto);
      if (dto.AnnouncementId == 0)
        saveTask = _httpClient.PostAsync("api/Announcements", content);
      else
        saveTask = _httpClient.PutAsync("api/Announcements", content);

      var response = await saveTask;
      return await _serviceResponseHandler.HandleJsonResponse<AnnouncementDto>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new AnnouncementDto();
  }

  public async Task<List<RoleDto>?> GetRoles()
  {
    try
    {
      var response = await _httpClient.GetAsync("api/roles");
      return await _serviceResponseHandler.HandleJsonResponse<List<RoleDto>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<RoleDto>();
  }

  public async Task<List<UserDto>?> GetUsers()
  {
    try
    {
      var response = await _httpClient.GetAsync("api/Users");
      return await _serviceResponseHandler.HandleJsonResponse<List<UserDto>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<UserDto>();
  }
  
  public async Task<UserDto?> GetUser(int userId)
  {
    try
    {
      var url = $"api/Users/{userId}";
      var response = await _httpClient.GetAsync(url);
      return await _serviceResponseHandler.HandleJsonResponse<UserDto>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new UserDto();
  }

  public async Task<UserDto?> SaveUser(UserDto dto)
  {
    try
    {
      Task<HttpResponseMessage>? saveTask;
      var content = _serviceResponseHandler.BuildJsonContent(dto);
      if (dto.UserId == 0)
        saveTask = _httpClient.PostAsync("api/Users", content);
      else
        saveTask = _httpClient.PutAsync("api/Users", content);

      var response = await saveTask;
      return await _serviceResponseHandler.HandleJsonResponse<UserDto>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return null;
  }

  public async Task<UserDto?> GetUserProfile()
  {
    try
    {
      var response = await _httpClient.GetAsync("api/manage");
      return await _serviceResponseHandler.HandleJsonResponse<UserDto>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new UserDto();
  }

  public async Task<UserDto?> SaveUserProfile(UserDto dto)
  {
    try
    {
      var content = _serviceResponseHandler.BuildJsonContent(dto);
      var response = await _httpClient.PutAsync("api/manage", content);
      return await _serviceResponseHandler.HandleJsonResponse<UserDto>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return null;
  }

  public async Task<List<EventDto>?> GetEvents()
  {
    try
    {
      var response = await _httpClient.GetAsync("api/Events");
      return await _serviceResponseHandler.HandleJsonResponse<List<EventDto>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<EventDto>();
  }

  public async Task<EventDto?> SaveEvent(EventDto dto)
  {
    try
    {
      Task<HttpResponseMessage>? saveTask;
      var content = _serviceResponseHandler.BuildJsonContent(dto);
      if (dto.EventId == 0)
        saveTask = _httpClient.PostAsync("api/Events", content);
      else
        saveTask = _httpClient.PutAsync("api/Events", content);

      var response = await saveTask;
      return await _serviceResponseHandler.HandleJsonResponse<EventDto>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new EventDto();
  }

  public async Task<List<SelectOptionDto<int>>?> GetLocationsForSelection()
  {
    try
    {
      var response = await _httpClient.GetAsync("api/Locations/ForSelection");
      return await _serviceResponseHandler.HandleJsonResponse<List<SelectOptionDto<int>>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<SelectOptionDto<int>>();
  }

  public async Task<List<LocationDto>?> GetLocations()
  {
    try
    {
      var response = await _httpClient.GetAsync("api/Locations");
      return await _serviceResponseHandler.HandleJsonResponse<List<LocationDto>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<LocationDto>();
  }


  public async Task<LocationDto?> SaveLocation(LocationDto dto)
  {
    try
    {
      Task<HttpResponseMessage>? saveTask;
      var content = _serviceResponseHandler.BuildJsonContent(dto);
      if (dto.LocationId == 0)
        saveTask = _httpClient.PostAsync("api/Locations", content);
      else
        saveTask = _httpClient.PutAsync("api/Locations", content);

      var response = await saveTask;
      return await _serviceResponseHandler.HandleJsonResponse<LocationDto>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new LocationDto();
  }

  public async Task<List<PictureDto>?> GetMyMedia()
  {
    try
    {
      var response = await _httpClient.GetAsync("api/pictures/mine");
      return await _serviceResponseHandler.HandleJsonResponse<List<PictureDto>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<PictureDto>();
  }

  public async Task<List<PictureDto>?> GetAdminMedia()
  {
    try
    {
      var response = await _httpClient.GetAsync("api/pictures/admin");
      return await _serviceResponseHandler.HandleJsonResponse<List<PictureDto>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<PictureDto>();
  }

  public async Task<PictureDto?> SaveMedia(PictureDto dto, Stream? file)
  {
    try
    {
      var content = new MultipartFormDataContent();
      content.Add(_serviceResponseHandler.BuildJsonContent(dto), "json", "json.json");
      if (file is not null)
      {
        content.Add(new StreamContent(file), "file", "file.name");
      }
      Task<HttpResponseMessage>? saveTask;
      if (dto.PictureId == 0)
        saveTask = _httpClient.PostAsync("api/pictures", content);
      else
        saveTask = _httpClient.PutAsync("api/pictures", content);

      var response = await saveTask;
      return await _serviceResponseHandler.HandleJsonResponse<PictureDto>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new PictureDto();
  }
}
