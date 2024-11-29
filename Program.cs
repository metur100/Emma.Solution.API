using Emma.Api;
using Emma.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder);
ConfigureDatabaseContext(builder);
ConfigureApiExplorerAndSwagger(builder);
ConfigureBasicAuthentication(builder);

var app = builder.Build();
ConfigureMiddleware(app);

app.Run();

void ConfigureMiddleware(WebApplication app)
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    app.UseHttpsRedirection();

    app.UseAuthentication(); 

    app.UseAuthorization();

    app.MapControllers();
}

void ConfigureDatabaseContext(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration["ConnectionStrings"];
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
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

        // Add JWT authentication scheme
        setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. \nEnter your token in the text input below.\n\nExample: Bearer {your_token}",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey, // Use ApiKey to specify headers
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        // Set the security requirements
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
                Array.Empty<string>() // No specific scopes required
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("vIgkS94GZw9Pxuoeoefiwlp4vOmj5fQa"))
            };
        });

        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
 }
