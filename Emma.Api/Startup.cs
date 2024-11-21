using Emma.Api.Handlers.ContactHandler;
using Emma.Api.Repositories.ContactRepository;

namespace Emma.Api
{
    public class Startup
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            #region Handler

            builder.Services.AddScoped<IContactHandler, ContactHandler>();

            #endregion

            #region Repository
            builder.Services.AddScoped<IContactRepository, ContactRepository>();

            #endregion

        }
    }
}
