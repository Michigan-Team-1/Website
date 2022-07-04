using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Team1.Entities;
using Team1.Infrastructure;
using Team1.Infrastructure.Services.Users;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.UserIdentity;
using Team1.Web.Site.Infrastructure.UserIdentity;
using Team1.Web.Site.Infrastructure.UserIdentity.Policies;
//using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using Team1.Infrastructure.Services.Tasks;
using Team1.Infrastructure.Services.Events;
using Team1.Infrastructure.Services.Locations;
using Team1.Infrastructure.Services.Announcements;
using Team1.Infrastructure.Services.Pictures;
using Microsoft.Extensions.Hosting;

namespace Team1.Web.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static TokenValidationParameters TokenValidationParameters { get; private set; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSwaggerGen(c =>
            //{
            //    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            //    {
            //        Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
            //        In = ParameterLocation.Header,
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.ApiKey
            //    });

            //    c.OperationFilter<SecurityRequirementsOperationFilter>();

            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Title = "Spud.Template API",
            //        Version = "v1",
            //        Description = "Spud.Template API Docs",
            //        Contact = new OpenApiContact() { Name = "Spud Software", Url = new Uri("https://www.spudsoftware.com/") }
            //    });

            //    // uncomment if documentation isn't working to help figure out why it isn't working
            //    // this ignores any *public* method that doesn't have a httpget, httppost, etc. attribute on it.
            //    //c.DocInclusionPredicate((docName, apiDesc) => 
            //    //{
            //    //    if (apiDesc.HttpMethod == null) return false;
            //    //    return true;
            //    //});

            //    // Set the comments path for the Swagger JSON and UI.
            //    var basePath = AppContext.BaseDirectory;
            //    var xmlPath = Path.Combine(basePath, "APIDocumentation.xml");
            //    c.IncludeXmlComments(xmlPath);
            //});

            services.AddDataProtection();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<AppSettings>();

            services.Configure<Services.Email.MailSettings>(Configuration.GetSection(nameof(Services.Email.MailSettings)));
            services.Configure<Services.FileManager.FileManagerSettings>(Configuration.GetSection(nameof(Services.FileManager.FileManagerSettings)));
            services.Configure<IpWhiteListSettings>(Configuration.GetSection(nameof(IpWhiteListSettings)));

            services.AddOptions();
            services.AddTransient<Services.Email.IEmailer, Services.Email.Emailer>();
            services.AddTransient<Services.FileManager.IFileManager, Services.FileManager.LocalFileManager>();

            services.AddLogging();

            services.AddLocalization();

            services.AddHttpsRedirection(o =>
            {
                o.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                o.HttpsPort = 443;
            });

            var appSettings = new AppSettings(Configuration);
            AuthenticationAndAuthorization(services, appSettings);

            services.AddControllers((options) => { }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddJsonOptions((options) =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            var supportedCultures = appSettings.SupportedCultures.Cultures.Select(s => new CultureInfo(s)).ToList();
            services.Configure<RequestLocalizationOptions>(opts =>
            {
                opts.DefaultRequestCulture = new RequestCulture(appSettings.SupportedCultures.DefaultCulture);
                // Formatting numbers, dates, etc.
                opts.SupportedCultures = supportedCultures;
                // UI strings that we have localized.
                opts.SupportedUICultures = supportedCultures;
            });

            // middleware
            services.AddTransient<Infrastructure.ActionFilters.DenyIfIpAddressChangedFilter>();
            services.AddTransient<Infrastructure.ActionFilters.IpWhiteListFilter>();
            services.AddTransient<Infrastructure.ActionFilters.LogRequestResponseFilter>();

            // developer created class DI ***************************************
            DeveloperAddedServices(services);
        }

        /// <summary>
        ///  This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Spud.Template API");
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandling();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseHsts();

            // require https on QA (staging) and production
            if (env.IsStaging() || env.IsProduction())
                app.UseHttpsRedirection();

            // turn on if routing help is needed.
            //app.Use((context, next) =>
            //{
            //    var endpointFeature = context.Features[typeof(IEndpointFeature)] as IEndpointFeature;
            //    var endpoint = endpointFeature?.Endpoint;

            //    //note: endpoint will be null, if there was no resolved route
            //    if (endpoint != null)
            //    {
            //        var routePattern = (endpoint as RouteEndpoint)?.RoutePattern?.RawText;

            //        Console.WriteLine("Name: " + endpoint.DisplayName);
            //        Console.WriteLine($"Route Pattern: {routePattern}");
            //        Console.WriteLine("Metadata Types: " + string.Join(", ", endpoint.Metadata));
            //    }
            //    return next();
            //});

            app.UseRouting();

            // must be between UseRouting and UseEndpoints
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        /// <summary>
        /// All authentication and authorization setup goes here.
        /// </summary>
        private void AuthenticationAndAuthorization(IServiceCollection services, AppSettings appSettings)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(0, 10, 0);
                options.Lockout.MaxFailedAccessAttempts = 3;

                options.Password.RequireDigit = false;
                options.Password.RequiredLength = Model.Constants.FieldSizes.PasswordMinimumLength;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = true;
            }).AddUserStore<Team1.Entities.UserIdentity.UserStore>().AddRoleStore<Team1.Entities.UserIdentity.RoleStore>()
                        .AddDefaultTokenProviders().AddUserManager<UserManager<User>>().AddSignInManager<SignInManager<User>>();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JWTSettings.Key)),

                ValidateIssuer = true,
                ValidIssuer = appSettings.JWTSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = appSettings.JWTSettings.Issuer,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;

                config.TokenValidationParameters = TokenValidationParameters;
            });

            services.AddAuthorization(config =>
            {
                config.AddPolicy(PolicyNames.AnnouncementAddEditDelete, policy => policy.Requirements.Add(new AnnouncementAddEditDelete()));
                config.AddPolicy(PolicyNames.CanApprovePicture, policy => policy.Requirements.Add(new CanApprovePicture()));
                config.AddPolicy(PolicyNames.CanImpersonate, policy => policy.Requirements.Add(new CanImpersonate()));
                config.AddPolicy(PolicyNames.EventAddEditDelete, policy => policy.Requirements.Add(new EventAddEditDelete()));
                config.AddPolicy(PolicyNames.LocationAddEditDelete, policy => policy.Requirements.Add(new LocationAddEditDelete()));
                config.AddPolicy(PolicyNames.PictureAddEditDelete, policy => policy.Requirements.Add(new PictureAddEditDelete()));
                config.AddPolicy(PolicyNames.TaskAddEditDelete, policy => policy.Requirements.Add(new TaskAddEditDelete()));
                config.AddPolicy(PolicyNames.UserAddEditDelete, policy => policy.Requirements.Add(new UserAddEditDelete()));
                config.AddPolicy(PolicyNames.UserProfileEdit, policy => policy.Requirements.Add(new UserProfileEdit()));
            });
        }

        /// <summary>
        /// Services that the system uses that are added
        /// </summary>
        private void DeveloperAddedServices(IServiceCollection services)
        {
            services.AddTransient<UserPermissionService>((s) =>
            {
                var httpContext = s.GetService<IHttpContextAccessor>();
                string ipAddress;
                if (httpContext.HttpContext.Connection.RemoteIpAddress.IsIPv4MappedToIPv6)
                    ipAddress = httpContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                else
                    ipAddress = httpContext.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();
                var ups = new UserPermissionService(ipAddress);
                ups.Setup(new UserClaimBuilder(httpContext.HttpContext.User));
                return ups;
            });

            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ILoggingContext, DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<AnnouncementsCreateUpdate>();
            services.AddTransient<AnnouncementsGet>();
            services.AddTransient<EventsCreateUpdate>();
            services.AddTransient<EventsGet>();
            services.AddTransient<LocationsCreateUpdate>();
            services.AddTransient<LocationsGet>();
            services.AddTransient<PicturesCreateUpdate>();
            services.AddTransient<PicturesGet>();
            services.AddTransient<TasksCreateUpdate>();
            services.AddTransient<TasksGet>();

            services.AddTransient<UserRefreshTokensGet>();
            services.AddTransient<UserRefreshTokensCreateUpdate>();
            services.AddTransient<UsersCreateUpdate>();
            services.AddTransient<UsersGet>();

            services.Configure<JWTSettings>(Configuration.GetSection(nameof(JWTSettings)));

            services.AddTransient<Team1.Infrastructure.Services.Templates.Emails.EmailsCreate>();
            services.AddTransient<Team1.Infrastructure.Services.Addresses.AddressesGet>();
            services.AddTransient<Team1.Infrastructure.Services.Addresses.AddressCreateUpdate>();
            services.AddTransient<Team1.Infrastructure.Services.MobileCarriers.MobileCarriersGet>();
            services.AddTransient<Team1.Infrastructure.Services.Countries.CountriesGet>();
            services.AddTransient<Team1.Infrastructure.Services.GoverningDistricts.GoverningDistrictsGet>();
            services.AddTransient<Team1.Infrastructure.Services.Logs.SystemLogsCreate>();
            services.AddTransient<Team1.Infrastructure.Services.Logs.APILogsCreateUpdate>();
        }
    }
}
