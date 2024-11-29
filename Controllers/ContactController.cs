using Emma.Api.Handlers.ContactHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ContactController(IContactHandler contactHandler) : ControllerBase
    {
        private readonly IContactHandler _contactHandler = contactHandler;

        [HttpGet]
        public IActionResult GetAllContacts()
        {
            var result = _contactHandler.GetAllContacts();
            return Ok(result);
        }
    }
}
