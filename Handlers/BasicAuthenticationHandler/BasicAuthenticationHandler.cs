//using Azure.Core;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.Extensions.Options;
//using System.Security.Claims;
//using System.Text.Encodings.Web;
//using System.Text;

//namespace Emma.Api.Handlers.BasicAuthenticationHandler
//{
//    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
//    {
//        public BasicAuthenticationHandler(
//            IOptionsMonitor<AuthenticationSchemeOptions> options,
//            ILoggerFactory logger,
//            UrlEncoder encoder,
//            ISystemClock clock)
//            : base(options, logger, encoder, clock)
//        {
//        }

//        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
//        {
//            // Check for Authorization header
//            if (!Request.Headers.ContainsKey("Authorization"))
//            {
//                return AuthenticateResult.Fail("Unauthorized");
//            }

//            string authorizationHeader = Request.Headers["Authorization"];
//            if (string.IsNullOrEmpty(authorizationHeader))
//            {
//                return AuthenticateResult.Fail("Unauthorized");
//            }

//            if (!authorizationHeader.StartsWith("basic ", StringComparison.OrdinalIgnoreCase))
//            {
//                return AuthenticateResult.Fail("Unauthorized");
//            }

//            var token = authorizationHeader.Substring(6);
//            var credentialAsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
//            var credentials = credentialAsString.Split(":");

//            if (credentials?.Length != 2)
//            {
//                return AuthenticateResult.Fail("Unauthorized");
//            }

//            var username = credentials[0];
//            var password = credentials[1];

//            // Validate user dynamically (e.g., against a database)
//            var isValidUser = await ValidateUserAsync(username, password);
//            if (!isValidUser)
//            {
//                return AuthenticateResult.Fail("Authentication failed");
//            }

//            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, username) };
//            var identity = new ClaimsIdentity(claims, "Basic");
//            var claimsPrincipal = new ClaimsPrincipal(identity);

//            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
//        }

//        private async Task<bool> ValidateUserAsync(string username, string password)
//        {
//            // Replace this logic with a database query or service call
//            return username == "test@fake.com" && password == "subscribe";
//        }
//    }
//}
