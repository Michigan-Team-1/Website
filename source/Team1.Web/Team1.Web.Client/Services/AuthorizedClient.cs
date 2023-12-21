using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Helpers;
using Team1.Infrastructure.Dtos.Users;
using Team1.Web.Client.Helpers;

namespace Team1.Web.Client.Services;

public class AuthorizedClient
{
    private readonly HttpClient _httpClient;
    private readonly ServiceResponseHandler _serviceResponseHandler;

    public AuthorizedClient(HttpClient httpClient, ServiceResponseHandler serviceResponseHandler)
    {
        _httpClient = httpClient;
        _serviceResponseHandler = serviceResponseHandler;
    }

    public async Task<List<UserDto>> GetUsers()
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

    public async Task<List<RoleDto>> GetRoles()
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
}
