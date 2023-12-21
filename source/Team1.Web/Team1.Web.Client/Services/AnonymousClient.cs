using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Team1.Infrastructure.Dtos;
using Team1.Web.Client.Helpers;

namespace Team1.Web.Client.Services;

public class AnonymousClient
{
    private readonly HttpClient _httpClient;
    private readonly ServiceResponseHandler _serviceResponseHandler;

    public AnonymousClient(HttpClient httpClient, ServiceResponseHandler serviceResponseHandler)
    {
        _httpClient = httpClient;
        _serviceResponseHandler = serviceResponseHandler;
    }

    //public async Task<List<RocketDto>> GetRockets()
    //{
    //    try
    //    {
    //        var response = await _httpClient.GetAsync("api/Rockets");
    //        return await _serviceResponseHandler.HandleJsonResponse<List<RocketDto>>(response);
    //    }
    //    catch (AccessTokenNotAvailableException exception)
    //    {
    //        exception.Redirect();
    //    }

    //    return new List<RocketDto>();
    //}

    //public async Task<List<LaunchDto>> GetLaunches(int rocketId)
    //{
    //    try
    //    {
    //        var response = await _httpClient.GetAsync($"api/Launches/{rocketId}");
    //        return await _serviceResponseHandler.HandleJsonResponse<List<LaunchDto>>(response);
    //    }
    //    catch (AccessTokenNotAvailableException exception)
    //    {
    //        exception.Redirect();
    //    }

    //    return new List<LaunchDto>();
    //}
}
