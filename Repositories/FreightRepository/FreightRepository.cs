using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.ProfileRepository;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.FreightRepository
{
    public class FreightRepository(AppDbContext context, IProfileRepository profileRepository) : IFreightRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IProfileRepository _profileRepository = profileRepository;

        public async Task CreateFreightAsync(Freight freight, int profileId)
        {
            try
            {
                var user = await _profileRepository.GetProfileById(profileId);

                freight.ProfileId = user;

                _context.Freights.Add(freight);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating freight: {ex.Message}");
            }
        }

        public async Task<List<FreightListDTO>> GetAllFreightsAsync()
        {
            try
            {
                var freights = await _context.Freights
                    .Select(freight => new FreightListDTO
                    {
                        Id = freight.Id,
                        ProfileId = freight.ProfileId,
                        UserName = _context.Profiles
                            .Where(user => user.Id == freight.ProfileId)
                            .Select(user => user.Name)
                            .FirstOrDefault() ?? "Unknown User",
                        ProfilePicture = _context.Profiles
                            .Where(user => user.Id == freight.ProfileId)
                            .Select(user => user.ProfilePicture)
                            .FirstOrDefault() ?? "Unknown Picture",
                        LoadingAddress = freight.LoadingAddress,
                        AvailabilityDate = freight.AvailabilityDate
                    })
                    .ToListAsync();

                return freights;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all freights: {ex.Message}");
            }
        }

        public async Task<FreightInformationsDTO> GetFreightInformationByFreightIdAsync(int profileId, int freightId)
        {
            var freight = await _context.Freights
                .Where(f => f.ProfileId == profileId && f.Id == freightId)
                .Join(_context.Profiles, freight => freight.ProfileId, user => user.Id, (freight, user) => new { freight, user })
                .Join(_context.Addresses, temp => temp.user.AddressId, address => address.Id, (temp, address) => new { temp.freight, temp.user, address })
                .Join(_context.Contacts, temp => temp.user.ContactId, contact => contact.Id, (temp, contact) => new FreightInformationsDTO
                {
                    Id = temp.freight.Id,
                    ProfileId = temp.freight.ProfileId,
                    UserName = temp.user.Name,
                    StreetAndNumber = temp.address.StreetAndNumber,
                    PostalCode = temp.address.PostalCode,
                    City = temp.address.City,
                    Email = contact.Email,
                    PhoneNumber = contact.PhoneNumber,
                    LoadingAddress = temp.freight.LoadingAddress,
                    UnloadingAddress = temp.freight.UnloadingAddress,
                    TransportPrice = temp.freight.TransportPrice,
                    Status = temp.freight.Status,
                    Distance = temp.freight.Distance,
                    GoodsDescription = temp.freight.GoodsDescription,
                    GoodsType = temp.freight.GoodsType,
                    GoodsWeight = temp.freight.GoodsWeight,
                    GoodsLength = temp.freight.GoodsLength,
                    GoodsPrice = temp.freight.GoodsPrice,
                    AvailabilityDate = temp.freight.AvailabilityDate
                })
                .FirstOrDefaultAsync();

            return freight ?? throw new Exception($"Freight with ID {freightId} for user {profileId} not found.");
        }

        public async Task<List<Freight>> GetFreightsByProfileIdAsync(int profileId)
        {
            try
            {
                var freights = await _context.Freights
                    .Where(freight => freight.ProfileId == profileId) 
                    .ToListAsync(); 

                return freights;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching freights for user {profileId}: {ex.Message}");
            }
        }
    }
}
