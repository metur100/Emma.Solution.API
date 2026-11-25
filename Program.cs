using Emma.Api;
using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Windows Event Log is not writable on shared hosting - log to console/debug only.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
Startup.ConfigureServices(builder);
ConfigureDatabaseContext(builder);
ConfigureApiExplorerAndSwagger(builder);
ConfigureBasicAuthentication(builder);
ConfigureCors(builder);

var app = builder.Build();
ApplyDatabaseMigrations(app);
ConfigureMiddleware(app);

app.Run();

void ConfigureMiddleware(WebApplication app)
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var root = exception;
        while (root?.InnerException != null) root = root.InnerException;

        var isClientError = exception is InvalidOperationException or ArgumentException or KeyNotFoundException
                            || root is InvalidOperationException or ArgumentException or KeyNotFoundException;

        context.Response.StatusCode = isClientError ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { message = root?.Message ?? "Unexpected error." });
    }));

    // The frontend calls the API under /api/... (same host, no route clashes with the SPA).
    // Old routes without the prefix keep working.
    app.UsePathBase("/api");

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseCors("frontend");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Single page app (built React frontend in wwwroot) - every unknown route serves index.html.
    app.MapFallbackToFile("index.html");
}

void ConfigureCors(WebApplicationBuilder builder)
{
    var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ??
    [
        "http://localhost:5173",
        "https://emma-logistics-git-master-metur100s-projects.vercel.app",
        "https://emma-logistics-i8069jgoe-metur100s-projects.vercel.app"
    ];

    builder.Services.AddCors(options => options.AddPolicy("frontend", policy => policy
        .WithOrigins(origins)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()));
}

void ConfigureDatabaseContext(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    if (string.IsNullOrWhiteSpace(connectionString))
        connectionString = builder.Configuration["ConnectionString"];
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(3)));
}

void ApplyDatabaseMigrations(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
        logger.LogInformation("Database migrations applied.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Applying database migrations failed.");
    }
}

void ConfigureApiExplorerAndSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(setup =>
    {
        setup.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Emma.API",
            Version = "v1"
        });

        setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. \nEnter your token in the text input below.\n\nExample: Bearer {your_token}",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        setup.AddSecurityRequirement(new OpenApiSecurityRequirement
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
}

void ConfigureBasicAuthentication(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "EmmaLogistics",
            ValidAudience = "EmmaLogisticsUser",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTHelper.GetSigningKey(builder.Configuration)))
        };
    });

    builder.Services.AddAuthorization();
    builder.Services.AddControllers();
}
