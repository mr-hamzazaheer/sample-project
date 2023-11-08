global using Asp.Versioning;
global using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.Mobile.Configuration;
using Repository.Portal.Configuration;
using Serilog;
using Shared.Repository;
using Shared.Repository.Interface;
using System.IO.Compression;
using System.Text;
//LgAAAB_@_LCAAAAAAAAApzqDQyKfHRNLY1NbENyM80VFVRtrQ0NzYztLKwsDQ2MHIwNo6Njo830DY00tXVBgAGvJ4yLgAAAA_!__!_

var builder = WebApplication.CreateBuilder(args);


IConfiguration _configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                             .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                             .Build();

Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
.WriteTo.File($"{builder.Environment.WebRootPath}" + @"\\logs\\log.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();


#region DbContext

builder.Services.AddDbContext<PracticeContext>(item =>
   item.UseSqlServer(_configuration.GetConnectionString("Default"), x =>
   {
       x.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
   }));

builder.Services.AddIdentity<AppUser, AppRole>()
        .AddEntityFrameworkStores<PracticeContext>()
        .AddDefaultTokenProviders()
        .AddUserStore<UserStore<AppUser, AppRole, PracticeContext, Guid>>()
        .AddRoleStore<RoleStore<AppRole, PracticeContext, Guid>>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
});
#endregion

#region Metadata

AppSettings _settings = new();
builder.Configuration.GetSection("Settings").Bind(_settings, c => c.BindNonPublicProperties = true);
Static.Settings = _settings;
#endregion

#region Services

builder.Services.AddLogging(configuration => configuration.ClearProviders());

builder.Services.AddScoped<Response>();
builder.Services.AddScoped<IShared, SharedRepository>();
builder.Services.AddPortalScope();
builder.Services.AddMobileScope();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
      "CorsPolicy",
      builder => builder.WithOrigins(_settings.CorsUrl)
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials());
});

#region GzipCompression
builder.Services.Configure<GzipCompressionProviderOptions>
                   (options => options.Level = CompressionLevel.Optimal);
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = new[] { "text/plain",
                                "text/css",
                                "application/javascript",
                                "text/html",
                                "application/xml",
                                "text/xml",
                                "application/json",
                                "text/json",
                                "image/svg+xml",
                               "application/atom+xml",
                               "image/png",
                               "image/jpg","text/x-component","text/javascript",
                               "application/x-javascript","application/manifest+json",
                                "application/vnd.api+json","application/xhtml+xml",
                                "application/rss+xml","application/vnd.ms-fontobject",
                                "application/x-font-ttf","application/x-font-opentype",
                                "application/x-font-truetype","image/x-icon",
                                "image/vnd.microsoft.icon","font/ttf",
                                "font/eot","font/otf","font/opentype",".json","json"
             };
    options.Providers.Add<GzipCompressionProvider>();
});
#endregion

#region Swagger

builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1, 0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
    x.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("x-version"),
        new MediaTypeApiVersionReader("ver"));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("mobile", new OpenApiInfo { Title = "Mobile API", Version = "v1" });
    c.SwaggerDoc("portal", new OpenApiInfo { Title = "Portal API", Version = "v1" });

    c.DocInclusionPredicate((documentName, apiDescription) =>
    {
        return ((ApiExplorerSettingsAttribute)apiDescription.ActionDescriptor.EndpointMetadata.First(x => x.GetType().Equals(typeof(ApiExplorerSettingsAttribute))))
        .GroupName == documentName;

    });
    c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
    {
        Description = "OAuth2",
        Name = "auth",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
        {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
                {
                Type = ReferenceType.SecurityScheme,
                Id = "OAuth2"
                },
                Scheme = "oauth2",
                Name = "auth",
                In = ParameterLocation.Header,

            },
            new List<string>()
            }
        });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
        {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
                {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,

            },
            new List<string>()
            }
        });

});
#endregion

#region JwtAuth

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = Static.Settings.Jwt.Issuer,
        ValidAudience = Static.Settings.Jwt.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(Static.Settings.Jwt.Key)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});
#endregion

builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Host.UseSerilog();
#endregion

#region App

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger().UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/mobile/swagger.json", "Mobile API");
    c.SwaggerEndpoint("/swagger/portal/swagger.json", "Portal API");
});
app.UseMiddleware(typeof(OAuth));
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
#endregion