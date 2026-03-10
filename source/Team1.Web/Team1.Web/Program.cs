using Blazored.Modal;
using Blazored.Toast;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Team1.Entities;
using Team1.Infrastructure.Services.Announcements;
using Team1.Infrastructure.Services.Events;
using Team1.Infrastructure.Services.Locations;
using Team1.Infrastructure.Services.Pictures;
using Team1.Infrastructure.Services.Users;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.UserIdentity;
using Team1.Web.Client.Helpers;
using Team1.Web.Client.Layout;
using Team1.Web.Client.Services;
using Team1.Web.Common.UserIdentity;
using Team1.Web.Common.UserIdentity.Policies;
using Team1.Web.Components;
using Team1.Web.Components.Account;
using Team1.Infrastructure.Dtos;
using Services.FileManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddControllers().AddJsonOptions((options) =>
{
  options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
  options.JsonSerializerOptions.Converters.Add(new TextJsonSerializer.DBNullConverter());
});

builder.Services.AddLocalization();
builder.Services.AddOptions();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
      options.DefaultScheme = IdentityConstants.ApplicationScheme;
      options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddIdentityCore<User>(options =>
{
  options.Lockout.AllowedForNewUsers = true;
  options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 10, 0);
  options.Lockout.MaxFailedAccessAttempts = 3;

  options.Password.RequireDigit = false;
  options.Password.RequiredLength = Team1.Model.Constants.FieldSizes.PasswordMinimumLength;
  options.Password.RequiredUniqueChars = 0;
  options.Password.RequireLowercase = true;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase = true;

  options.User.RequireUniqueEmail = true;

  options.SignIn.RequireConfirmedEmail = true;
}).AddRoles<Role>().AddUserStore<Team1.Entities.UserIdentity.UserStore>().AddRoleStore<Team1.Entities.UserIdentity.RoleStore>()
.AddSignInManager<CustomSignInManager>().AddDefaultTokenProviders();

builder.Services.AddAuthorization(config =>
{
  config.AddPolicy(PolicyNames.AnnouncementAddEditDelete, policy => policy.Requirements.Add(new AnnouncementAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.CanApprovePicture, policy => policy.Requirements.Add(new CanApprovePictureRequirement()));
  config.AddPolicy(PolicyNames.CanImpersonate, policy => policy.Requirements.Add(new CanImpersonateRequirement()));
  config.AddPolicy(PolicyNames.EventAddEditDelete, policy => policy.Requirements.Add(new EventAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.LocationAddEditDelete, policy => policy.Requirements.Add(new LocationAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.PictureAddEditDelete, policy => policy.Requirements.Add(new PictureAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.UserAddEditDelete, policy => policy.Requirements.Add(new UserAddEditDeleteRequirement()));
  config.AddPolicy(PolicyNames.UserProfileEdit, policy => policy.Requirements.Add(new UserProfileEditRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, AnnouncementAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, CanApprovePicture>();
builder.Services.AddScoped<IAuthorizationHandler, CanImpersonate>();
builder.Services.AddScoped<IAuthorizationHandler, EventAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, LocationAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, PictureAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, UserAddEditDelete>();
builder.Services.AddScoped<IAuthorizationHandler, UserProfileEdit>();

builder.Services.Configure<Services.Email.MailSettings>(builder.Configuration.GetSection(nameof(Services.Email.MailSettings)));
builder.Services.AddScoped<IEmailSender<User>, CustomEmailSender>();
builder.Services.AddScoped<Services.Email.IEmailer, Services.Email.Emailer>();

builder.Services.AddTransient<Services.FileManager.IFileManager, Services.FileManager.LocalFileManager>();
builder.Services.Configure<FileManagerSettings>(builder.Configuration.GetSection(nameof(FileManagerSettings)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LocationDtoValidator>();

Team1.Web.Common.FluentValidationHelpers.SetupDisplayNameResolver();

// must be transient
builder.Services.AddTransient<UserPermissionService>((s) =>
{
  var httpContext = s.GetService<IHttpContextAccessor>();
  string? ipAddress = null;
  if (httpContext?.HttpContext?.Connection?.RemoteIpAddress != null)
  {
    if (httpContext.HttpContext.Connection.RemoteIpAddress.IsIPv4MappedToIPv6)
      ipAddress = httpContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    else
      ipAddress = httpContext.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
  }
  var ups = new UserPermissionService(ipAddress);
  ups.Setup(new UserClaimBuilder(httpContext!.HttpContext?.User));
  return ups;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ILoggingContext, DataContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<AnnouncementsCreateUpdate>();
builder.Services.AddTransient<AnnouncementsGet>();
builder.Services.AddTransient<EventsCreateUpdate>();
builder.Services.AddTransient<EventsGet>();
builder.Services.AddTransient<LocationsCreateUpdate>();
builder.Services.AddTransient<LocationsGet>();
builder.Services.AddTransient<PicturesCreateUpdate>();
builder.Services.AddTransient<PicturesGet>();

builder.Services.AddTransient<UsersCreateUpdate>();
builder.Services.AddTransient<UsersGet>();

builder.Services.AddTransient<Team1.Infrastructure.Services.Templates.Emails.EmailsCreate>();
builder.Services.AddTransient<Team1.Infrastructure.Services.Addresses.AddressesGet>();
builder.Services.AddTransient<Team1.Infrastructure.Services.Addresses.AddressCreateUpdate>();
builder.Services.AddTransient<Team1.Infrastructure.Services.MobileCarriers.MobileCarriersGet>();
builder.Services.AddTransient<Team1.Infrastructure.Services.Countries.CountriesGet>();
builder.Services.AddTransient<Team1.Infrastructure.Services.GoverningDistricts.GoverningDistrictsGet>();
builder.Services.AddTransient<Team1.Infrastructure.Services.Logs.SystemLogsCreate>();
builder.Services.AddTransient<Team1.Infrastructure.Services.Logs.APILogsCreateUpdate>();

builder.Services.AddHeaderPropagation(options =>
{
  options.Headers.Add("Cookie", context =>
  {
    KeyValuePair<string,string>? accessToken = context.HttpContext.Request.Cookies.FirstOrDefault(w => w.Key == ".AspNetCore.Identity.Application");
    return accessToken != null ? new StringValues($"{accessToken.Value.Key}={accessToken.Value.Value}") : new StringValues();
  });
});

// Supply HttpClient instances that include access tokens when making requests to the server project
var url = builder.Configuration.GetValue<string>("ApiUrl") ?? throw new InvalidOperationException("Missing API Url");
builder.Services.AddHttpClient("Auth", client =>
{
  client.BaseAddress = new Uri(url);
}).AddHeaderPropagation(options =>
{
  options.Headers.Add("Cookie");
});

builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Auth"));
builder.Services.AddScoped((s) => new AuthorizedClient(s.GetRequiredService<IHttpClientFactory>().CreateClient("Auth"), s.GetRequiredService<ServiceResponseHandler>()));

builder.Services.AddScoped<AnonymousClient>();
builder.Services.AddScoped<AuthorizedClient>();

builder.Services.AddScoped<ServiceResponseHandler>();

builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredModal();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseWebAssemblyDebugging();
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseHeaderPropagation();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(LoginDisplay).Assembly);

app.MapControllers();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
