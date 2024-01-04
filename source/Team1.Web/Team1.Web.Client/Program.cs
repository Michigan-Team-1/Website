using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Team1.Infrastructure.UserIdentity;
using Team1.Web.Client;
using Team1.Web.Client.Helpers;
using Team1.Web.Client.Services;
using Team1.Web.Common.UserIdentity.Policies;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTransient<AnonymousClient>();
builder.Services.AddTransient<AuthorizedClient>();

builder.Services.AddApiAuthorization();

builder.Services.AddAuthorizationCore(config =>
{
  config.AddPolicy(PolicyNames.AnnouncementAddEditDelete, policy => policy.Requirements.Add(new AnnouncementAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.CanApprovePicture, policy => policy.Requirements.Add(new CanApprovePictureRequirement()));
  config.AddPolicy(PolicyNames.CanImpersonate, policy => policy.Requirements.Add(new CanImpersonateRequirement()));
  config.AddPolicy(PolicyNames.EventAddEditDelete, policy => policy.Requirements.Add(new EventAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.LocationAddEditDelete, policy => policy.Requirements.Add(new LocationAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.PictureAddEditDelete, policy => policy.Requirements.Add(new PictureAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.TaskAddEditDelete, policy => policy.Requirements.Add(new TaskAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.UserAddEditDelete, policy => policy.Requirements.Add(new UserAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.UserProfileEdit, policy => policy.Requirements.Add(new UserProfileEditRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, AnnouncementAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, CanApprovePicture>();
builder.Services.AddScoped<IAuthorizationHandler, CanImpersonate>();
builder.Services.AddScoped<IAuthorizationHandler, EventAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, LocationAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, PictureAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, TaskAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, UserAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, UserProfileEdit>();

builder.Services.AddScoped<UserPermissionService>((s) =>
{
  var ups = new UserPermissionService(null);
  return ups;
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredModal();
//builder.Services.AddBlazoredLocalStorageAsSingleton();

builder.Services.AddScoped<ServiceResponseHandler>();
await builder.Build().RunAsync();
