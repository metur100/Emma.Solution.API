using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Models
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public required DbSet<User> Users { get; set; }
        public required DbSet<Address> Addresses { get; set; }
        public required DbSet<Contact> Contacts { get; set; }
        public required DbSet<Vehicle> Vehicles { get; set; }
        public required DbSet<Freight> Freights { get; set; }
        public required DbSet<Offer> Offers { get; set; }
        public required DbSet<Profile> Profiles { get; set; }
        public required DbSet<Status> Statuses { get; set; }
    }
}
