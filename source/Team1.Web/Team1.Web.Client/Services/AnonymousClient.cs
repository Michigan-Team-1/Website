using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Team1.Infrastructure.Dtos;
using Team1.Infrastructure.Dtos.Users;
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

  public async Task<List<UserDto>?> GetBoardOfDirectors()
  {
    try
    {
      var response = await _httpClient.GetAsync("api/Users/Bod");
      return await _serviceResponseHandler.HandleJsonResponse<List<UserDto>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<UserDto>();
  }

  public async Task<List<CountryDto>?> GetCountries()
  {
    try
    {
      var response = await _httpClient.GetAsync($"api/countries");
      return await _serviceResponseHandler.HandleJsonResponse<List<CountryDto>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<CountryDto>();
  }

  public async Task<List<GoverningDistrictDto>?> GetGoverningDistricts()
  {
    try
    {
      var response = await _httpClient.GetAsync($"api/governingdistricts");
      return await _serviceResponseHandler.HandleJsonResponse<List<GoverningDistrictDto>>(response);
    }
    catch (AccessTokenNotAvailableException exception)
    {
      exception.Redirect();
    }

    return new List<GoverningDistrictDto>();
  }
}
