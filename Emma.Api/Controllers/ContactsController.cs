using Emma.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ContactsController(ILogger<ContactsController> logger, ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        [HttpGet]
        public IActionResult GetContacts()
        {
            var result = _applicationDbContext.Contacts.ToList();
            return Ok(result);
        }

    }
}
