using Emma.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactsController(ApplicationDbContext applicationDbContext) : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        [HttpGet]
        public IActionResult GetContacts()
        {
            var result = _applicationDbContext.Contacts.ToList();
            return Ok(result);
        }
    }
}
