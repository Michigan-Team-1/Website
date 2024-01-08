using Microsoft.AspNetCore.Http;
using Team1.Web.Client.Helpers;
using Team1.Web.Client.Services;

namespace Team1.Web.Services;

public class ServerAuthorizedClient : AuthorizedClient
{
  private readonly IHttpContextAccessor httpContext;

  public ServerAuthorizedClient(HttpClient httpClient, ServiceResponseHandler serviceResponseHandler, IHttpContextAccessor httpContext): base (httpClient, serviceResponseHandler)
    {
    this.httpContext = httpContext;
    }

  protected override void SetupCookies()
  {
   
  }

}
