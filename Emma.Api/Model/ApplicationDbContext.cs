using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Emma.Api.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    }
}
