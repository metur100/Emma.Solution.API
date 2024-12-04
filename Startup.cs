using Emma.Api.Handlers.ContactHandler;
using Emma.Api.Handlers.FreightHandler;
using Emma.Api.Handlers.UserHandler;
using Emma.Api.Handlers.ProfileHandler;
using Emma.Api.Handlers.VehicleHandler;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.FreightRepository;
using Emma.Api.Repositories.UserRepository;
using Emma.Api.Repositories.ProfileRepository;
using Emma.Api.Repositories.VehicleRepository;
using Emma.Api.Handlers.OfferHandler;
using Emma.Api.Repositories.OfferRepository;
using Emma.Api.Helpers.JWTHelper;

namespace Emma.Api
{
    public class Startup
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            #region Handler

            builder.Services.AddScoped<IContactHandler, ContactHandler>();
            builder.Services.AddScoped<IProfileHandler, ProfileHandler>();
            builder.Services.AddScoped<IVehicleHandler, VehicleHandler>();
            builder.Services.AddScoped<IFreightHandler, FreightHandler>();
            builder.Services.AddScoped<IUserHandler, UserHandler>();
            builder.Services.AddScoped<IOfferHandler, OfferHandler>();

            builder.Services.AddScoped<IJWTHelper, JWTHelper>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<Func<IJWTHelper>>(sp => () => sp.GetRequiredService<IJWTHelper>());

            #endregion

            #region Repository

            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<IFreightRepository, FreightRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOfferRepository, OfferRepository>();

            #endregion

        }
    }
}
