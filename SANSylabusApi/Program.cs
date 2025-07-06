using Aspire.Hosting;
using HealthChecks.SqlServer;
using Aspire.Hosting.ApplicationModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using SylabusAPI.Data;
using SylabusAPI.Mapping;
using SylabusAPI.Services.Implementations;
using SylabusAPI.Services.Interfaces;
using SylabusAPI.Settings;
using System.Text;
using SANSylabusApi.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

/*builder.Logging
    .AddOpenTelemetry(logging =>
    {
        logging.IncludeFormattedMessage = true;
        logging.IncludeScopes = true;
    });*/

builder.AddServiceDefaults();

builder.Services.AddDbContext<SyllabusContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SylabusAPI",
        Description = "System zarz?dzania sylabusami dla uczelni wy?szej",
        TermsOfService = new Uri("https://twoja-domena.pl/terms"),
        Contact = new OpenApiContact
        {
            Name = "Zespó? Wsparcia SylabusAPI",
            Url = new Uri("https://twoja-domena.pl/contact")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Definicja schematu bezpiecze?stwa dla JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Wprowad? token w formacie: Bearer {twój_token_JWT}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPrzedmiotService, PrzedmiotService>();
builder.Services.AddScoped<ISylabusService, SylabusService>();
builder.Services.AddScoped<ISiatkaService, SiatkaService>();
builder.Services.AddScoped<IHistoriaService, HistoriaService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// 5. Authentication & JWT
var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwt.SecretKey!);

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
var isGoogleEnabled = !string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret);
builder.Services.AddSingleton(new AuthProviderStatus { IsGoogleEnabled = isGoogleEnabled });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Do autoryzacji JWT
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    // Do rzuconych wyjątków 401
        options.DefaultSignInScheme = "Cookies";                                     // Do SignInAsync
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    })
    .AddCookie("Cookies"); 

// Opcjonalnie Google, jeśli masz dane
if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    builder.Services.AddAuthentication()
        .AddGoogle("Google", options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
        });
}




builder.Services.AddAuthorization(); 

builder.Services.AddEndpointsApiExplorer(); 

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7033")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });

    options.AddPolicy("AllowGeneralLocalhost", policy =>
    {
        policy.WithOrigins("https://localhost")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });

    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7033", "https://localhost")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "sql",
        tags: new[] { "ready" }
    )
    .AddUrlGroup(new Uri("https://www.google.com"), name: "google")
    .AddUrlGroup(
        new Uri("https://frontend/health"),
        name: "frontend",
        tags: new[] { "ready" },
        configurePrimaryHttpMessageHandler: _ => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseRouting()
    app.MapOpenApi();
    //app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI v1");
        //options.SwaggerEndpoint("/swagger/v1/swagger.json", "SylabusAPI v1");
        options.DisplayRequestDuration();
    });
    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
    });
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
