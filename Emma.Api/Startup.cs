using Emma.Api.Handlers.ContactHandler;
using Emma.Api.Handlers.UserHandler;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.UserRepository;

namespace Emma.Api
{
    public class Startup
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            #region Handler

            builder.Services.AddScoped<IContactHandler, ContactHandler>();
            builder.Services.AddScoped<IUserHandler, UserHandler>();


            #endregion

            #region Repository
            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();

            #endregion

        }
    }
}
