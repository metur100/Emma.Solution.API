using Emma.Api;
using Emma.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder);
ConfigureDatabaseContext(builder);
ConfigureApiExplorerAndSwagger(builder);

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

    app.UseAuthorization();

    app.MapControllers();
}

void ConfigureDatabaseContext(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}

void ConfigureApiExplorerAndSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}
