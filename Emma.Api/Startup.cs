using Emma.Api.Handlers.ContactHandler;
using Emma.Api.Handlers.FreightHandler;
using Emma.Api.Handlers.UserHandler;
using Emma.Api.Handlers.VehicleHandler;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.FreightRepository;
using Emma.Api.Repositories.UserRepository;
using Emma.Api.Repositories.VehicleRepository;

namespace Emma.Api
{
    public class Startup
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            #region Handler

            builder.Services.AddScoped<IContactHandler, ContactHandler>();
            builder.Services.AddScoped<IUserHandler, UserHandler>();
            builder.Services.AddScoped<IVehicleHandler, VehicleHandler>();
            builder.Services.AddScoped<IFreightHandler, FreightHandler>();

            #endregion

            #region Repository

            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<IFreightRepository, FreightRepository>();

            #endregion

        }
    }
}
