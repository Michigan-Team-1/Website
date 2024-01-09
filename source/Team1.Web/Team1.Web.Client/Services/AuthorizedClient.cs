using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Team1.Infrastructure.Dtos;
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

  protected virtual void SetupCookies()
  {
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

    public async Task<List<RoleDto>?> GetRoles()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Roles");
            return await _serviceResponseHandler.HandleJsonResponse<List<RoleDto>>(response);
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
        return new List<RoleDto>();
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

}
