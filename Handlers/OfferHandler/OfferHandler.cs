using Emma.Api.Helpers.EmailServiceHelper;
using Emma.Api.Helpers.EnrichOfferHelper;
using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Models.Enum;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.FreightRepository;
using Emma.Api.Repositories.OfferRepository;
using Emma.Api.Repositories.ProfileRepository;
using Emma.Api.Repositories.StatusRepository;
using Emma.Api.Repositories.VehicleRepository;

namespace Emma.Api.Handlers.OfferHandler
{
    public class OfferHandler(IOfferRepository repository, 
        IStatusRepository statusRepository,
        IProfileRepository profileRepository,
        IVehicleRepository vehicleRepository,
        IFreightRepository freightRepository,
        IEnrichOfferHelper enrichOfferHelper,
        IJWTHelper jWTHelper,
        IEmailServiceHelper emailServiceHelper,
        IContactRepository contactRepository) : IOfferHandler
    {
        private readonly IOfferRepository _offerRepository = repository;
        private readonly IStatusRepository _statusRepository = statusRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IFreightRepository _freightRepository = freightRepository;
        private readonly IEnrichOfferHelper _enrichOfferHelper = enrichOfferHelper;
        private readonly IJWTHelper _jWTHelper = jWTHelper;
        private readonly IEmailServiceHelper _emailServiceHelper = emailServiceHelper;
        private readonly IContactRepository _contactRepository = contactRepository;

        public async Task CreateOfferAsync(Offer offer, int userId)
        {
            if (offer == null)
                throw new ArgumentNullException(nameof(offer), "Offer cannot be null.");

            if (userId <= 0)
                throw new ArgumentException("User ID must be a positive integer.", nameof(userId));

            try
            {
                await _offerRepository.CreateOfferAsync(offer, userId);

                if (offer.Id <= 0)
                    throw new InvalidOperationException("Offer creation failed. The Offer ID is invalid.");

                var initialStatus = new Status
                {
                    OfferId = offer.Id,
                    StatusType = (int)StatusType.Created,
                    CreatedAt = DateTime.UtcNow
                };
                await _statusRepository.CreateStatusAsync(initialStatus);

                var offerDetails = await GetOfferDetailsByIdAsync(offer.Id); 
                if (offerDetails == null)
                    throw new InvalidOperationException("Failed to retrieve offer details.");

                var contactId = await _profileRepository.GetContactIdByProfileId(offer.OfferFromUserProfileId)
                    ?? throw new InvalidOperationException("ContactId for the specified profile ID was not found.");

                var contactDetails = await _contactRepository.GetContactByIdAsync(contactId);

                var emailSubject = "Nova ponuda / New offer";
                var emailBody = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #1abc9c;'>Poštovani {offerDetails.OfferForUser},</h2>
                    <p>Primili ste novu ponudu od korisnika <strong>{offerDetails.OfferFromUser}</strong>.</p>
                    
                    <h3 style='color: #34495e;'>Detalji ponude:</h3>
                    <ul>
                        <li><strong>Prvobitna cijena transporta:</strong> {offerDetails.TransportPrice}</li>
                        <li><strong>Predložena nova cijena transporta:</strong> {offerDetails.NewTransportPrice}</li>
                        <li><strong>Komentar:</strong> {offerDetails.Comment}</li>
                    </ul>
                    
                    <h3 style='color: #34495e;'>Informacije o vozilu:</h3>
                    <ul>
                        <li><strong>Marka vozila:</strong> {offerDetails.Brand}</li>
                        <li><strong>Opis vozila:</strong> {offerDetails.Description}</li>
                        <li><strong>Nosivost vozila:</strong> {offerDetails.PayloadCapacity}</li>
                        <li><strong>Vrsta karoserije:</strong> {offerDetails.BodyType}</li>
                        <li><strong>Volumen teretnog prostora:</strong> {offerDetails.CargoSpaceVolume}</li>
                        <li><strong>Težina vozila:</strong> {offerDetails.Weight}</li>
                        <li><strong>Dužina vozila:</strong> {offerDetails.Length}</li>
                    </ul>
                    
                    <h3 style='color: #34495e;'>Informacije o robi:</h3>
                    <ul>
                        <li><strong>Opis robe:</strong> {offerDetails.GoodsDescription}</li>
                        <li><strong>Cijena robe:</strong> {offerDetails.GoodsPrice}</li>
                        <li><strong>Težina robe:</strong> {offerDetails.GoodsWeight}</li>
                        <li><strong>Dužina robe:</strong> {offerDetails.GoodsLength}</li>
                    </ul>
                    
                    <h3 style='color: #34495e;'>Informacije o transportu:</h3>
                    <ul>
                        <li><strong>Adresa utovara:</strong> {offerDetails.LoadingAddress}</li>
                        <li><strong>Adresa istovara:</strong> {offerDetails.UnloadingAddress}</li>
                        <li><strong>Udaljenost:</strong> {offerDetails.Distance}</li>
                    </ul>

                    <h3 style='color: #34495e;'>Kontakt podaci:</h3>
                    <ul>
                        <li><strong>Email:</strong> {contactDetails?.Email}</li>
                        <li><strong>Broj telefona:</strong> {contactDetails?.PhoneNumber}</li>
                    </ul>
                    
                    <p>Za više informacija i upravljanje ponudama, prijavite se na svoj nalog.</p>
                    <p>Lijep pozdrav,<br>Vaš Emma Logistics tim</p>

                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>

                    <h2 style='color: #1abc9c;'><em>Dear {offerDetails.OfferForUser},</em></h2>
                    <p><em>You have received a new offer from user <strong>{offerDetails.OfferFromUser}</strong>.</em></p>
                    
                    <h3 style='color: #34495e;'><em>Offer Details:</em></h3>
                    <ul>
                        <li><em><strong>Initial Transport Price:</strong> {offerDetails.TransportPrice}</em></li>
                        <li><em><strong>Proposed New Transport Price:</strong> {offerDetails.NewTransportPrice}</em></li>
                        <li><em><strong>Comment:</strong> {offerDetails.Comment}</em></li>
                    </ul>
                    
                    <h3 style='color: #34495e;'><em>Vehicle Information:</em></h3>
                    <ul>
                        <li><em><strong>Vehicle Brand:</strong> {offerDetails.Brand}</em></li>
                        <li><em><strong>Vehicle Description:</strong> {offerDetails.Description}</em></li>
                        <li><em><strong>Payload Capacity:</strong> {offerDetails.PayloadCapacity}</em></li>
                        <li><em><strong>Body Type:</strong> {offerDetails.BodyType}</em></li>
                        <li><em><strong>Cargo Space Volume:</strong> {offerDetails.CargoSpaceVolume}</em></li>
                        <li><em><strong>Vehicle Weight:</strong> {offerDetails.Weight}</em></li>
                        <li><em><strong>Vehicle Length:</strong> {offerDetails.Length}</em></li>
                    </ul>
                    
                    <h3 style='color: #34495e;'><em>Goods Information:</em></h3>
                    <ul>
                        <li><em><strong>Goods Description:</strong> {offerDetails.GoodsDescription}</em></li>
                        <li><em><strong>Goods Price:</strong> {offerDetails.GoodsPrice}</em></li>
                        <li><em><strong>Goods Weight:</strong> {offerDetails.GoodsWeight}</em></li>
                        <li><em><strong>Goods Length:</strong> {offerDetails.GoodsLength}</em></li>
                    </ul>
                    
                    <h3 style='color: #34495e;'><em>Transport Information:</em></h3>
                    <ul>
                        <li><em><strong>Loading Address:</strong> {offerDetails.LoadingAddress}</em></li>
                        <li><em><strong>Unloading Address:</strong> {offerDetails.UnloadingAddress}</em></li>
                        <li><em><strong>Distance:</strong> {offerDetails.Distance}</em></li>
                    </ul>

                    <h3 style='color: #34495e;'>Contact details:</h3>
                    <ul>
                        <li><strong>E-mail:</strong> {contactDetails?.Email}</li>
                        <li><strong>Phone number:</strong> {contactDetails?.PhoneNumber}</li>
                    </ul>
                    
                    <p><em>For more information and to manage offers, please log in to your account.</em></p>
                    <p><em>Best regards,<br>Your Emma Logistics Team</em></p>
                </div>";

                var email = await _profileRepository.GetEmailByProfileId(offer.OfferForUserProfileId) 
                    ?? throw new InvalidOperationException("Email for the specified profile ID was not found.");

                _emailServiceHelper.SendEmail(
                    new List<string> { email },
                    emailSubject,
                    emailBody
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while creating the offer.", ex);
            }
        }

        public async Task AddCounterOffer(CounterOfferDTO counterOfferDTO)
        {
            if (counterOfferDTO == null)
                throw new ArgumentNullException(nameof(counterOfferDTO));

            try
            {
                var offer = await _offerRepository.GetOfferByIdAsync(counterOfferDTO.OfferId);
                if (offer == null)
                    throw new InvalidOperationException("Offer not found.");

                var loggedInUserProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(_jWTHelper.GetUserId());

                if (loggedInUserProfileIds.Contains(offer.OfferFromUserProfileId))
                {
                    counterOfferDTO.CounterOfferProfileId = offer.OfferFromUserProfileId;
                }
                else if (loggedInUserProfileIds.Contains(offer.OfferForUserProfileId))
                {
                    counterOfferDTO.CounterOfferProfileId = offer.OfferForUserProfileId;
                }
                else
                {
                    throw new InvalidOperationException("User does not have permission to make a counteroffer.");
                }

                offer.CounterOfferPrice = counterOfferDTO.CounterOfferPrice;
                offer.CounterOfferProfileId = counterOfferDTO.CounterOfferProfileId;
                await _offerRepository.UpdateOfferAsync(offer);

                var initialStatus = new Status
                {
                    OfferId = counterOfferDTO.OfferId,
                    StatusType = (int)StatusType.CounterOffer,
                    CreatedAt = DateTime.UtcNow
                };
                await _statusRepository.CreateStatusAsync(initialStatus);

                var recipientProfileId = offer.OfferFromUserProfileId == counterOfferDTO.CounterOfferProfileId
                    ? offer.OfferForUserProfileId
                    : offer.OfferFromUserProfileId;

                var email = await _profileRepository.GetEmailByProfileId(recipientProfileId)
                    ?? throw new InvalidOperationException("Email for the specified profile ID was not found.");

                var nameOfRecipient = await _profileRepository.GetProfileNameByProfileIdAsync(counterOfferDTO.CounterOfferProfileId);

                var offerDetails = await GetOfferDetailsByIdAsync(offer.Id);

                var emailBody = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #1abc9c;'>Poštovani,</h2>
                    <p>Primili ste novu protuponudu od korisnika <strong>{nameOfRecipient}</strong>.</p>
                    <h3>Detalji protuponude:</h3>
                    <ul>
                        <li><strong>Prvobitna cijena transporta:</strong> {offerDetails?.TransportPrice}</li>
                        <li><strong>Predložena nova protuponudna cijena transporta:</strong> {counterOfferDTO.CounterOfferPrice}</li>
                    </ul>
                    <p>Za više informacija i upravljanje ponudama, prijavite se na svoj nalog.</p>
                    <p>Lijep pozdrav,<br>Vaš Emma Logistics tim</p>

                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>

                    <h2 style='color: #1abc9c;'><em>Dear Sir/Madam,</em></h2>
                    <p><em>You have received a new counteroffer from user <strong>{nameOfRecipient}</strong>.</em></p>
                    <h3><em>Counteroffer Details:</em></h3>
                    <ul>
                        <li><em><strong>Original Transport Price:</strong> {offerDetails?.TransportPrice}</em></li>
                        <li><em><strong>Proposed New Counteroffer Transport Price:</strong> {counterOfferDTO.CounterOfferPrice}</em></li>
                    </ul>
                    <p><em>For more information and to manage offers, please log in to your account.</em></p>
                    <p><em>Best regards,<br>Your Emma Logistics Team</em></p>
                </div>";

                _emailServiceHelper.SendEmail(new List<string> { email }, "Nova protuponuda / New counteroffer", emailBody);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while adding the counteroffer.", ex);
            }
        }

        public async Task AddDeclineOfferAsync(int offerId)
        {
            if (offerId <= 0)
                throw new ArgumentException("Invalid offer ID.", nameof(offerId));

            try
            {
                var offer = await _offerRepository.GetOfferByIdAsync(offerId);
                if (offer == null)
                    throw new InvalidOperationException("Offer not found.");

                var declineStatus = new Status
                {
                    OfferId = offerId,
                    StatusType = (int)StatusType.Declined,
                    CreatedAt = DateTime.UtcNow
                };
                await _statusRepository.CreateStatusAsync(declineStatus);

                var offerDetails = await GetOfferDetailsByIdAsync(offer.Id);
                if (offerDetails == null)
                    throw new InvalidOperationException("Failed to retrieve offer details.");

                var recipientProfileId = offer.OfferFromUserProfileId == _jWTHelper.GetUserId()
                    ? offer.OfferForUserProfileId
                    : offer.OfferFromUserProfileId;

                var email = await _profileRepository.GetEmailByProfileId(recipientProfileId)
                    ?? throw new InvalidOperationException("Email for the specified profile ID was not found.");

                var nameOfRecipient = await _profileRepository.GetProfileNameByProfileIdAsync(recipientProfileId);

                var contactId1 = await _profileRepository.GetContactIdByProfileId(offer.OfferFromUserProfileId)
                    ?? throw new InvalidOperationException("ContactId for the specified profile ID was not found.");

                var contactId2 = await _profileRepository.GetContactIdByProfileId(offer.OfferForUserProfileId)
                    ?? throw new InvalidOperationException("ContactId for the specified profile ID was not found.");

                var contactDetails1 = await _contactRepository.GetContactByIdAsync(contactId1);
                var contactDetails2 = await _contactRepository.GetContactByIdAsync(contactId2);

                var emailSubject = "Ponuda odbijena / Offer rejected";
                var emailBody = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #e74c3c;'>Poštovani {nameOfRecipient},</h2>
                    <p>Nažalost, vaša ponuda je odbijena.</p>
                    
                    <h3 style='color: #34495e;'>Detalji ponude:</h3>
                    <ul>
                        <li><strong>Prvobitna cijena transporta:</strong> {offerDetails.TransportPrice}</li>
                        <li><strong>Predložena nova cijena transporta:</strong> {offerDetails.NewTransportPrice}</li>
                        <li><strong>Protuponudna cijena transporta:</strong> {offerDetails.CounterOfferTransportPrice}</li>
                        <li><strong>Komentar:</strong> {offerDetails.Comment}</li>
                    </ul>
                    
                    <h3 style='color: #34495e;'>Informacije o vozilu:</h3>
                    <ul>
                        <li><strong>Marka vozila:</strong> {offerDetails.Brand}</li>
                        <li><strong>Opis vozila:</strong> {offerDetails.Description}</li>
                        <li><strong>Nosivost vozila:</strong> {offerDetails.PayloadCapacity}</li>
                        <li><strong>Vrsta karoserije:</strong> {offerDetails.BodyType}</li>
                        <li><strong>Volumen teretnog prostora:</strong> {offerDetails.CargoSpaceVolume}</li>
                        <li><strong>Težina vozila:</strong> {offerDetails.Weight}</li>
                        <li><strong>Dužina vozila:</strong> {offerDetails.Length}</li>
                    </ul>
                    
                    <h3 style='color: #34495e;'>Informacije o robi:</h3>
                    <ul>
                        <li><strong>Opis robe:</strong> {offerDetails.GoodsDescription}</li>
                        <li><strong>Cijena robe:</strong> {offerDetails.GoodsPrice}</li>
                        <li><strong>Težina robe:</strong> {offerDetails.GoodsWeight}</li>
                        <li><strong>Dužina robe:</strong> {offerDetails.GoodsLength}</li>
                    </ul>
                    
                    <h3 style='color: #34495e;'>Informacije o transportu:</h3>
                    <ul>
                        <li><strong>Adresa utovara:</strong> {offerDetails.LoadingAddress}</li>
                        <li><strong>Adresa istovara:</strong> {offerDetails.UnloadingAddress}</li>
                        <li><strong>Udaljenost:</strong> {offerDetails.Distance}</li>
                    </ul>

                   <h3 style='color: #34495e;'>Kontakt podaci:</h3>
                    <ul>
                        <li><strong>Email:</strong> {contactDetails1?.Email}</li>
                        <li><strong>Broj telefona:</strong> {contactDetails1?.PhoneNumber}</li>
                    </ul>

                    <ul>
                        <li><strong>Email:</strong> {contactDetails2?.Email}</li>
                        <li><strong>Broj telefona:</strong> {contactDetails2?.PhoneNumber}</li>
                    </ul>
                    
                    <p>Za više informacija i upravljanje ponudama, prijavite se na svoj nalog.</p>
                    <p>Lijep pozdrav,<br>Vaš Emma Logistics tim</p>

                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>

                    <h2 style='color: #e74c3c;'><em>Dear {nameOfRecipient},</em></h2>
                    <p><em>Unfortunately, your offer has been declined.</em></p>
                    
                    <h3 style='color: #34495e;'><em>Offer Details:</em></h3>
                    <ul>
                        <li><em><strong>Original Transport Price:</strong> {offerDetails.TransportPrice}</em></li>
                        <li><em><strong>Proposed New Transport Price:</strong> {offerDetails.NewTransportPrice}</em></li>
                        <li><em><strong>Counteroffer Transport Price:</strong> {offerDetails.CounterOfferTransportPrice}</em></li>
                        <li><em><strong>Comment:</strong> {offerDetails.Comment}</em></li>
                    </ul>
                    
                    <h3 style='color: #34495e;'><em>Vehicle Information:</em></h3>
                    <ul>
                        <li><em><strong>Vehicle Brand:</strong> {offerDetails.Brand}</em></li>
                        <li><em><strong>Vehicle Description:</strong> {offerDetails.Description}</em></li>
                        <li><em><strong>Payload Capacity:</strong> {offerDetails.PayloadCapacity}</em></li>
                        <li><em><strong>Body Type:</strong> {offerDetails.BodyType}</em></li>
                        <li><em><strong>Cargo Space Volume:</strong> {offerDetails.CargoSpaceVolume}</em></li>
                        <li><em><strong>Vehicle Weight:</strong> {offerDetails.Weight}</em></li>
                        <li><em><strong>Vehicle Length:</strong> {offerDetails.Length}</em></li>
                    </ul>
                    
                    <h3 style='color: #34495e;'><em>Goods Information:</em></h3>
                    <ul>
                        <li><em><strong>Goods Description:</strong> {offerDetails.GoodsDescription}</em></li>
                        <li><em><strong>Goods Price:</strong> {offerDetails.GoodsPrice}</em></li>
                        <li><em><strong>Goods Weight:</strong> {offerDetails.GoodsWeight}</em></li>
                        <li><em><strong>Goods Length:</strong> {offerDetails.GoodsLength}</em></li>
                    </ul>
                    
                    <h3 style='color: #34495e;'><em>Transport Information:</em></h3>
                    <ul>
                        <li><em><strong>Loading Address:</strong> {offerDetails.LoadingAddress}</em></li>
                        <li><em><strong>Unloading Address:</strong> {offerDetails.UnloadingAddress}</em></li>
                        <li><em><strong>Distance:</strong> {offerDetails.Distance}</em></li>
                    </ul>

                    <h3 style='color: #34495e;'>Contact details:</h3>
                    <ul>
                        <li><strong>E-mail:</strong> {contactDetails1?.Email}</li>
                        <li><strong>Phone number:</strong> {contactDetails1?.PhoneNumber}</li>
                    </ul>

                    <ul>
                        <li><strong>Email:</strong> {contactDetails2?.Email}</li>
                        <li><strong>Broj telefona:</strong> {contactDetails2?.PhoneNumber}</li>
                    </ul>
                    
                    <p><em>For more information and to manage offers, please log in to your account.</em></p>
                    <p><em>Best regards,<br>Your Emma Logistics Team</em></p>
                </div>";

                _emailServiceHelper.SendEmail(new List<string> { email }, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while declining the offer with ID {offerId}.", ex);
            }
        }

        public async Task AddAcceptOfferAsync(int offerId)
        {
            if (offerId <= 0)
                throw new ArgumentException("Invalid offer ID.", nameof(offerId));

            try
            {
                var offer = await _offerRepository.GetOfferByIdAsync(offerId);
                if (offer == null)
                    throw new InvalidOperationException("Offer not found.");

                var acceptStatus = new Status
                {
                    OfferId = offerId,
                    StatusType = (int)StatusType.PreperingForTransit,
                    CreatedAt = DateTime.UtcNow
                };
                await _statusRepository.CreateStatusAsync(acceptStatus);

                var offerDetails = await GetOfferDetailsByIdAsync(offer.Id);
                if (offerDetails == null)
                    throw new InvalidOperationException("Failed to retrieve offer details.");

                var recipientProfileId = offer.OfferFromUserProfileId == _jWTHelper.GetUserId()
                    ? offer.OfferForUserProfileId
                    : offer.OfferFromUserProfileId;

                var email = await _profileRepository.GetEmailByProfileId(recipientProfileId)
                    ?? throw new InvalidOperationException("Email for the specified profile ID was not found.");

                var nameOfRecipient = await _profileRepository.GetProfileNameByProfileIdAsync(recipientProfileId);

                var contactId1 = await _profileRepository.GetContactIdByProfileId(offer.OfferFromUserProfileId)
                    ?? throw new InvalidOperationException("ContactId for the specified profile ID was not found.");

                var contactId2 = await _profileRepository.GetContactIdByProfileId(offer.OfferForUserProfileId)
                    ?? throw new InvalidOperationException("ContactId for the specified profile ID was not found.");

                var contactDetails1 = await _contactRepository.GetContactByIdAsync(contactId1);
                var contactDetails2 = await _contactRepository.GetContactByIdAsync(contactId2);

                var emailSubject = "Ponuda prihvaćena / Offer accepted";
                var emailBody = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #1abc9c;'>Poštovani {nameOfRecipient},</h2>
                    <p>Čestitamo! Vaša ponuda je prihvaćena.</p>
                    
                    <h3 style='color: #34495e;'>Detalji ponude:</h3>
                    <ul>
                        <li><strong>Prvobitna cijena transporta:</strong> {offerDetails.TransportPrice}</li>
                        <li><strong>Prihvaćena cijena transporta:</strong> {offerDetails.NewTransportPrice}</li>
                        <li><strong>Komentar:</strong> {offerDetails.Comment}</li>
                    </ul>
                    
                    <h3 style='color: #34495e;'>Informacije o vozilu:</h3>
                    <ul>
                        <li><strong>Marka vozila:</strong> {offerDetails.Brand}</li>
                        <li><strong>Opis vozila:</strong> {offerDetails.Description}</li>
                        <li><strong>Nosivost vozila:</strong> {offerDetails.PayloadCapacity}</li>
                        <li><strong>Vrsta karoserije:</strong> {offerDetails.BodyType}</li>
                        <li><strong>Volumen teretnog prostora:</strong> {offerDetails.CargoSpaceVolume}</li>
                        <li><strong>Težina vozila:</strong> {offerDetails.Weight}</li>
                        <li><strong>Dužina vozila:</strong> {offerDetails.Length}</li>
                    </ul>
                    
                    <h3 style='color: #34495e;'>Informacije o robi:</h3>
                    <ul>
                        <li><strong>Opis robe:</strong> {offerDetails.GoodsDescription}</li>
                        <li><strong>Cijena robe:</strong> {offerDetails.GoodsPrice}</li>
                        <li><strong>Težina robe:</strong> {offerDetails.GoodsWeight}</li>
                        <li><strong>Dužina robe:</strong> {offerDetails.GoodsLength}</li>
                    </ul>
                    
                    <h3 style='color: #34495e;'>Informacije o transportu:</h3>
                    <ul>
                        <li><strong>Adresa utovara:</strong> {offerDetails.LoadingAddress}</li>
                        <li><strong>Adresa istovara:</strong> {offerDetails.UnloadingAddress}</li>
                        <li><strong>Udaljenost:</strong> {offerDetails.Distance}</li>
                    </ul>

                    <h3 style='color: #34495e;'>Kontakt podaci:</h3>
                    <ul>
                        <li><strong>Email:</strong> {contactDetails1?.Email}</li>
                        <li><strong>Broj telefona:</strong> {contactDetails1?.PhoneNumber}</li>
                    </ul>

                    <ul>
                        <li><strong>Email:</strong> {contactDetails2?.Email}</li>
                        <li><strong>Broj telefona:</strong> {contactDetails2?.PhoneNumber}</li>
                    </ul>
                    
                    <p>Za više informacija i daljnje korake, prijavite se na svoj nalog.</p>
                    <p>Lijep pozdrav,<br>Vaš Emma Logistics tim</p>
                
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                
                    <h2 style='color: #1abc9c;'><em>Dear {nameOfRecipient},</em></h2>
                    <p><em>Congratulations! Your offer has been accepted.</em></p>
                    
                    <h3 style='color: #34495e;'><em>Offer Details:</em></h3>
                    <ul>
                        <li><em><strong>Original Transport Price:</strong> {offerDetails.TransportPrice}</em></li>
                        <li><em><strong>Accepted Transport Price:</strong> {offerDetails.NewTransportPrice}</em></li>
                        <li><em><strong>Comment:</strong> {offerDetails.Comment}</em></li>
                    </ul>
                    
                    <h3 style='color: #34495e;'><em>Vehicle Information:</em></h3>
                    <ul>
                        <li><em><strong>Vehicle Brand:</strong> {offerDetails.Brand}</em></li>
                        <li><em><strong>Vehicle Description:</strong> {offerDetails.Description}</em></li>
                        <li><em><strong>Payload Capacity:</strong> {offerDetails.PayloadCapacity}</em></li>
                        <li><em><strong>Body Type:</strong> {offerDetails.BodyType}</em></li>
                        <li><em><strong>Cargo Space Volume:</strong> {offerDetails.CargoSpaceVolume}</em></li>
                        <li><em><strong>Vehicle Weight:</strong> {offerDetails.Weight}</em></li>
                        <li><em><strong>Vehicle Length:</strong> {offerDetails.Length}</em></li>
                    </ul>
                    
                    <h3 style='color: #34495e;'><em>Goods Information:</em></h3>
                    <ul>
                        <li><em><strong>Goods Description:</strong> {offerDetails.GoodsDescription}</em></li>
                        <li><em><strong>Goods Price:</strong> {offerDetails.GoodsPrice}</em></li>
                        <li><em><strong>Goods Weight:</strong> {offerDetails.GoodsWeight}</em></li>
                        <li><em><strong>Goods Length:</strong> {offerDetails.GoodsLength}</em></li>
                    </ul>
                    
                    <h3 style='color: #34495e;'><em>Transport Information:</em></h3>
                    <ul>
                        <li><em><strong>Loading Address:</strong> {offerDetails.LoadingAddress}</em></li>
                        <li><em><strong>Unloading Address:</strong> {offerDetails.UnloadingAddress}</em></li>
                        <li><em><strong>Distance:</strong> {offerDetails.Distance}</em></li>
                    </ul>

                    <h3 style='color: #34495e;'>Contact details:</h3>
                    <ul>
                        <li><strong>E-mail:</strong> {contactDetails1?.Email}</li>
                        <li><strong>Phone number:</strong> {contactDetails1?.PhoneNumber}</li>
                    </ul>

                    <ul>
                        <li><strong>Email:</strong> {contactDetails2?.Email}</li>
                        <li><strong>Broj telefona:</strong> {contactDetails2?.PhoneNumber}</li>
                    </ul>
                    
                    <p><em>For more information and next steps, please log in to your account.</em></p>
                    <p><em>Best regards,<br>Your Emma Logistics Team</em></p>
                </div>";

                _emailServiceHelper.SendEmail(new List<string> { email }, emailSubject, emailBody);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while declining the offer with ID {offerId}.", ex);
            }
        }

        public async Task<OfferDetailsDTO?> GetOfferDetailsByIdAsync(int offerId)
        {
            if (offerId <= 0)
                throw new ArgumentException("Invalid offer ID.");

            var offer = await _offerRepository.GetOfferByIdAsync(offerId);
            if (offer == null)
                return null;

            string offerForStuff = string.Empty;
            if (offer.VehicleId == offer.OfferForStuffId)
            {
                var vehicleForStuff = await _vehicleRepository.GetVehicleByIdAsync(offer.OfferForStuffId);
                offerForStuff = vehicleForStuff != null
                    ? $"{vehicleForStuff.Brand} {vehicleForStuff.BodyType}"
                    : "Unknown Vehicle";
            }
            else if (offer.FreightId == offer.OfferForStuffId)
            {
                var freightForStuff = await _freightRepository.GetFreightByIdAsync(offer.OfferForStuffId);
                offerForStuff = freightForStuff != null
                    ? $"{freightForStuff.GoodsType} - {freightForStuff.GoodsDescription}"
                    : "Unknown Freight";
            }

            string offerFromStuff = string.Empty;
            if (offer.VehicleId == offer.OfferFromStuffId)
            {
                var vehicleFromStuff = await _vehicleRepository.GetVehicleByIdAsync(offer.OfferFromStuffId);
                offerFromStuff = vehicleFromStuff != null
                    ? $"{vehicleFromStuff.Brand} {vehicleFromStuff.BodyType}"
                    : "Unknown Vehicle";
            }
            else if (offer.FreightId == offer.OfferFromStuffId)
            {
                var freightFromStuff = await _freightRepository.GetFreightByIdAsync(offer.OfferFromStuffId);
                offerFromStuff = freightFromStuff != null
                    ? $"{freightFromStuff.GoodsType} - {freightFromStuff.GoodsDescription}"
                    : "Unknown Freight";
            }

            var offerFromUser = await _profileRepository.GetProfileNameByProfileIdAsync(offer.OfferFromUserProfileId)
                                ?? "Unknown User";
            var offerForUser = await _profileRepository.GetProfileNameByProfileIdAsync(offer.OfferForUserProfileId)
                                ?? "Unknown User";

            var transporter = await _profileRepository.GetProfileNameByProfileIdAsync(offer.TransporterProfileId)
                              ?? "Unknown Transporter";

            var freightForTransport = await _freightRepository.GetFreightByIdAsync(offer.FreightIdForTransport);
            var freightForTransportName = freightForTransport != null
                ? $"{freightForTransport.LoadingAddress} -> {freightForTransport.UnloadingAddress}"
                : "Unknown Freight";

            var offerVehicle = await _vehicleRepository.GetVehicleByIdAsync(offer.VehicleId);
            var offerFreight = await _freightRepository.GetFreightByIdAsync(offer.FreightId);

            if (offerVehicle == null || offerFreight == null)
                throw new InvalidOperationException("Offer data is incomplete.");

            var loggedInUserProfileIds = await _profileRepository.GetProfileNamesByUserIdAsync(_jWTHelper.GetUserId()); 

            string profileToRateName = "Nepoznat korisnik!"; 

            if (loggedInUserProfileIds.Contains(offerFromUser))
            {
                profileToRateName = offerForUser; 
            }
            else if (loggedInUserProfileIds.Contains(offerForUser))
            {
                profileToRateName = offerFromUser; 
            }

            return new OfferDetailsDTO
            {
                Id = offer.Id,
                OfferForStuff = offerForStuff,
                OfferFromUser = offerFromUser,
                OfferForUser = offerForUser,
                OfferFromStuff = offerFromStuff,
                Transporter = transporter,
                FreightForTransport = freightForTransportName,
                OfferForUserProfileId = offer.OfferForUserProfileId,
                OfferFromUserProfileId = offer.OfferFromUserProfileId,
                ProfileToRateName = profileToRateName,
                NewTransportPrice = offer.NewTransportPrice,
                CounterOfferTransportPrice = offer.CounterOfferPrice,
                Comment = offer.Comment,
                Brand = offerVehicle.Brand,
                PlateNumber = offerVehicle.PlateNumber,
                Description = offerVehicle.Description,
                BodyType = offerVehicle.BodyType,
                PayloadCapacity = offerVehicle.PayloadCapacity,
                CargoSpaceVolume = offerVehicle.CargoSpaceVolume,
                Length = offerVehicle.Length,
                Weight = offerVehicle.Weight,
                LoadingAddress = offerFreight.LoadingAddress,
                UnloadingAddress = offerFreight.UnloadingAddress,
                TransportPrice = offerFreight.TransportPrice,
                Distance = offerFreight.Distance,
                GoodsDescription = offerFreight.GoodsDescription,
                GoodsType = offerFreight.GoodsType,
                GoodsWeight = offerFreight.GoodsWeight,
                GoodsLength = offerFreight.GoodsLength,
                GoodsPrice = offerFreight.GoodsPrice,
                AvailabilityDate = offerFreight.AvailabilityDate
            };
        }

        public async Task<IEnumerable<OfferListDTO>> GetReceivedOffersAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("The user ID must be a positive integer.", nameof(userId));

            try
            {
                var profileIds = await _profileRepository.GetUserProfileIdsByUserIdAsync(userId);
                if (profileIds == null || !profileIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var vehicleIds = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(profileIds);
                var freightIds = await _freightRepository.GetFreightIdsByProfileIdsAsync(profileIds);

                var latestStatuses = await _statusRepository.GetLatestStatusesAsync();
                if (latestStatuses == null || !latestStatuses.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var validOfferIds = latestStatuses
                    .Where(ls => ls.LatestStatusType == (int)StatusType.Created || ls.LatestStatusType == (int)StatusType.CounterOffer)
                    .Select(ls => ls.OfferId)
                    .ToList();

                if (!validOfferIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var allOffers = await _offerRepository.GetOffersByIdsAsync(validOfferIds);
                if (allOffers == null || !allOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var filteredOffers = allOffers
                    .Where(o =>
                        (latestStatuses.Any(ls => ls.OfferId == o.Id && ls.LatestStatusType == (int)StatusType.Created) &&
                         !profileIds.Contains(o.OfferFromUserProfileId) &&
                         (vehicleIds.Contains(o.VehicleId) || freightIds.Contains(o.FreightId))) ||

                        (latestStatuses.Any(ls => ls.OfferId == o.Id && ls.LatestStatusType == (int)StatusType.CounterOffer) &&
                         o.CounterOfferProfileId.HasValue &&
                         !profileIds.Contains(o.CounterOfferProfileId.Value) &&
                         (vehicleIds.Contains(o.VehicleId) || freightIds.Contains(o.FreightId)))
                    )
                    .ToList();

                if (!filteredOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var enrichedOffers = new List<OfferListDTO>();
                foreach (var offer in filteredOffers)
                {
                    var enrichedOffer = await _enrichOfferHelper.EnrichOfferAsync(offer);
                    enrichedOffers.Add(enrichedOffer);
                }

                return enrichedOffers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while fetching received offers.", ex);
            }
        }

        public async Task<IEnumerable<OfferListDTO>> GetSentOffersAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            try
            {
                var userProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
                if (userProfileIds == null || !userProfileIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var latestStatuses = await _statusRepository.GetLatestStatusesAsync();
                if (latestStatuses == null || !latestStatuses.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var validOfferIds = latestStatuses
                    .Where(ls => ls.LatestStatusType == (int)StatusType.Created || ls.LatestStatusType == (int)StatusType.CounterOffer)
                    .Select(ls => ls.OfferId)
                    .ToList();

                if (!validOfferIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var allValidOffers = await _offerRepository.GetOffersByIdsAsync(validOfferIds);
                if (allValidOffers == null || !allValidOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var sentOffers = allValidOffers
                    .Where(o =>
                        (latestStatuses.Any(ls => ls.LatestStatusType == (int)StatusType.Created) &&
                        userProfileIds.Contains(o.OfferFromUserProfileId)) ||

                        (latestStatuses.Any(ls => ls.OfferId == o.Id && ls.LatestStatusType == (int)StatusType.CounterOffer) &&
                         o.CounterOfferProfileId.HasValue && userProfileIds.Contains(o.CounterOfferProfileId.Value))
                    )
                    .ToList();

                if (!sentOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var enrichedOffers = new List<OfferListDTO>();
                foreach (var offer in sentOffers)
                {
                    var enrichedOffer = await _enrichOfferHelper.EnrichOfferAsync(offer);
                    enrichedOffers.Add(enrichedOffer);
                }

                return enrichedOffers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving sent offers.", ex);
            }
        }

        public async Task<IEnumerable<OfferListDTO>> GetTransitOffersAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            try
            {
                var userProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
                if (userProfileIds == null || !userProfileIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var userVehicles = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(userProfileIds);
                var userFreights = await _freightRepository.GetFreightIdsByProfileIdsAsync(userProfileIds);

                var allOffers = await _offerRepository.GetOffersAsync();
                if (allOffers == null || !allOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var transitOfferIds = new List<int>();
                foreach (var offer in allOffers)
                {
                    if (await _statusRepository.HasStatusGreaterOrEqualAsync(offer.Id, (int)StatusType.PreperingForTransit))
                    {
                        transitOfferIds.Add(offer.Id);
                    }
                }

                if (!transitOfferIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var transitOffers = allOffers
                    .Where(o =>
                        (userProfileIds.Contains(o.OfferFromUserProfileId) && transitOfferIds.Contains(o.Id)) ||
                        (!userProfileIds.Contains(o.OfferFromUserProfileId) &&
                         (userVehicles.Contains(o.VehicleId) || userFreights.Contains(o.FreightId)) &&
                         transitOfferIds.Contains(o.Id)))
                    .ToList();

                if (!transitOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var enrichedOffers = new List<OfferListDTO>();
                foreach (var offer in transitOffers)
                {
                    var enrichedOffer = await _enrichOfferHelper.EnrichOfferAsync(offer);
                    enrichedOffers.Add(enrichedOffer);
                }

                return enrichedOffers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while fetching transit offers.", ex);
            }
        }

        public async Task<IEnumerable<OfferListDTO>> GetDeclinedOffersAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            try
            {
                var userProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
                if (userProfileIds == null || !userProfileIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var userVehicles = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(userProfileIds);
                var userFreights = await _freightRepository.GetFreightIdsByProfileIdsAsync(userProfileIds);

                var latestStatuses = await _statusRepository.GetLatestStatusesAsync();
                if (latestStatuses == null || !latestStatuses.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var declinedOfferIds = latestStatuses
                    .Where(ls => ls.LatestStatusType == (int)StatusType.Declined)
                    .Select(ls => ls.OfferId)
                    .ToList();

                if (!declinedOfferIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var allOffers = await _offerRepository.GetOffersByIdsAsync(declinedOfferIds);
                if (allOffers == null || !allOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var enrichedDeclinedOffers = new List<OfferListDTO>();

                foreach (var offer in allOffers)
                {
                    if (IsOfferRelevant(offer, userProfileIds, userVehicles, userFreights, declinedOfferIds))
                    {
                        var enrichedOffer = await _enrichOfferHelper.EnrichOfferAsync(offer);
                        enrichedDeclinedOffers.Add(enrichedOffer);
                    }
                }

                return enrichedDeclinedOffers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving declined offers.", ex);
            }
        }

        public async Task<int> GetAllOffersCountAsync()
        {
            try
            {
                return await _offerRepository.GetOffersCountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all offers: {ex.Message}");
            }
        }

        public async Task<int> GetAllTransportsCountAsync()
        {
            try
            {
                return await _offerRepository.GetTransportsCountAsync(StatusType.PreperingForTransit);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all offers: {ex.Message}");
            }
        }

        public async Task<int> GetAllDeliveredOffersCountAsync()
        {
            try
            {
                return await _offerRepository.GetDeliveredCountAsync(StatusType.Arrived);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all offers: {ex.Message}");
            }
        }

        private bool IsOfferRelevant(
            Offer offer,
            IEnumerable<int> userProfileIds,
            IEnumerable<int> userVehicles,
            IEnumerable<int> userFreights,
            IEnumerable<int> declinedOfferIds)
        {
            return (userProfileIds.Contains(offer.OfferFromUserProfileId) && declinedOfferIds.Contains(offer.Id)) ||
                   (!userProfileIds.Contains(offer.OfferFromUserProfileId) &&
                    (userVehicles.Contains(offer.VehicleId) || userFreights.Contains(offer.FreightId)) &&
                    declinedOfferIds.Contains(offer.Id));
        }

        public async Task<IEnumerable<OfferListDTO>> SearchReceivedOffersAsync(int userId, string searchTerm)
        {
            if (userId <= 0)
                throw new ArgumentException("The user ID must be a positive integer.", nameof(userId));

            try
            {
                var profileIds = await _profileRepository.GetUserProfileIdsByUserIdAsync(userId);
                if (profileIds == null || !profileIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var vehicleIds = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(profileIds);
                var freightIds = await _freightRepository.GetFreightIdsByProfileIdsAsync(profileIds);

                var latestStatuses = await _statusRepository.GetLatestStatusesAsync();
                if (latestStatuses == null || !latestStatuses.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var validOfferIds = latestStatuses
                    .Where(ls => ls.LatestStatusType == (int)StatusType.Created || ls.LatestStatusType == (int)StatusType.CounterOffer)
                    .Select(ls => ls.OfferId)
                    .ToList();

                if (!validOfferIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var allOffers = await _offerRepository.GetOffersByIdsAsync(validOfferIds);
                if (allOffers == null || !allOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var filteredOffers = allOffers
                    .Where(o =>
                        (latestStatuses.Any(ls => ls.OfferId == o.Id && ls.LatestStatusType == (int)StatusType.Created) &&
                         !profileIds.Contains(o.OfferFromUserProfileId) &&
                         (vehicleIds.Contains(o.VehicleId) || freightIds.Contains(o.FreightId))) ||

                        (latestStatuses.Any(ls => ls.OfferId == o.Id && ls.LatestStatusType == (int)StatusType.CounterOffer) &&
                         o.CounterOfferProfileId.HasValue &&
                         !profileIds.Contains(o.CounterOfferProfileId.Value) &&
                         (vehicleIds.Contains(o.VehicleId) || freightIds.Contains(o.FreightId)))
                    )
                    .ToList();

                if (!filteredOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var enrichedOffers = new List<OfferListDTO>();
                foreach (var offer in filteredOffers)
                {
                    var enrichedOffer = await _enrichOfferHelper.EnrichOfferAsync(offer);
                    enrichedOffers.Add(enrichedOffer);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var searchLower = searchTerm.ToLower();
                    enrichedOffers = enrichedOffers
                        .Where(o =>
                            (!string.IsNullOrEmpty(o.OfferForStuff) && o.OfferForStuff.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferForUser) && o.OfferForUser.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferFromStuff) && o.OfferFromStuff.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferFromUser) && o.OfferFromUser.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.Transporter) && o.Transporter.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.FreightForTransport) && o.FreightForTransport.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.TransportPrice) && o.TransportPrice.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.NewTransportPrice) && o.NewTransportPrice.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.TransportPrice) && o.TransportPrice.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.Comment) && o.Comment.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.CounterOfferPrice) && o.CounterOfferPrice.ToLower().Contains(searchLower))
                        )
                        .ToList();
                }

                return enrichedOffers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while fetching and searching received offers.", ex);
            }
        }

        public async Task<IEnumerable<OfferListDTO>> SearchSentOffersAsync(int userId, string searchTerm)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            try
            {
                var userProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
                if (userProfileIds == null || !userProfileIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var latestStatuses = await _statusRepository.GetLatestStatusesAsync();
                if (latestStatuses == null || !latestStatuses.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var validOfferIds = latestStatuses
                    .Where(ls => ls.LatestStatusType == (int)StatusType.Created || ls.LatestStatusType == (int)StatusType.CounterOffer)
                    .Select(ls => ls.OfferId)
                    .ToList();

                if (!validOfferIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var allValidOffers = await _offerRepository.GetOffersByIdsAsync(validOfferIds);
                if (allValidOffers == null || !allValidOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var sentOffers = allValidOffers
                    .Where(o =>
                        (latestStatuses.Any(ls => ls.LatestStatusType == (int)StatusType.Created) &&
                         userProfileIds.Contains(o.OfferFromUserProfileId)) ||

                        (latestStatuses.Any(ls => ls.OfferId == o.Id && ls.LatestStatusType == (int)StatusType.CounterOffer) &&
                         o.CounterOfferProfileId.HasValue && userProfileIds.Contains(o.CounterOfferProfileId.Value))
                    )
                    .ToList();

                if (!sentOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                // Enrich offers
                var enrichedOffers = new List<OfferListDTO>();
                foreach (var offer in sentOffers)
                {
                    var enrichedOffer = await _enrichOfferHelper.EnrichOfferAsync(offer);
                    enrichedOffers.Add(enrichedOffer);
                }

                // Apply search filter if searchTerm is provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var searchLower = searchTerm.ToLower();
                    enrichedOffers = enrichedOffers
                        .Where(o =>
                            (!string.IsNullOrEmpty(o.OfferForStuff) && o.OfferForStuff.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferForUser) && o.OfferForUser.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferFromStuff) && o.OfferFromStuff.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferFromUser) && o.OfferFromUser.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.Transporter) && o.Transporter.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.TransportPrice) && o.TransportPrice.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.FreightForTransport) && o.FreightForTransport.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.NewTransportPrice) && o.NewTransportPrice.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.Comment) && o.Comment.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.CounterOfferPrice) && o.CounterOfferPrice.ToLower().Contains(searchLower))
                        )
                        .ToList();
                }

                return enrichedOffers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving and searching sent offers.", ex);
            }
        }

        public async Task<IEnumerable<OfferListDTO>> SearchDeclinedOffersAsync(int userId, string searchTerm)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            try
            {
                var userProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
                if (userProfileIds == null || !userProfileIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var userVehicles = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(userProfileIds);
                var userFreights = await _freightRepository.GetFreightIdsByProfileIdsAsync(userProfileIds);

                var latestStatuses = await _statusRepository.GetLatestStatusesAsync();
                if (latestStatuses == null || !latestStatuses.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var declinedOfferIds = latestStatuses
                    .Where(ls => ls.LatestStatusType == (int)StatusType.Declined)
                    .Select(ls => ls.OfferId)
                    .ToList();

                if (!declinedOfferIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var allOffers = await _offerRepository.GetOffersByIdsAsync(declinedOfferIds);
                if (allOffers == null || !allOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var enrichedDeclinedOffers = new List<OfferListDTO>();
                foreach (var offer in allOffers)
                {
                    if (IsOfferRelevant(offer, userProfileIds, userVehicles, userFreights, declinedOfferIds))
                    {
                        var enrichedOffer = await _enrichOfferHelper.EnrichOfferAsync(offer);
                        enrichedDeclinedOffers.Add(enrichedOffer);
                    }
                }

                // Apply search filter if searchTerm is provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var searchLower = searchTerm.ToLower();
                    enrichedDeclinedOffers = enrichedDeclinedOffers
                        .Where(o =>
                            (!string.IsNullOrEmpty(o.OfferForStuff) && o.OfferForStuff.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferForUser) && o.OfferForUser.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferFromStuff) && o.OfferFromStuff.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferFromUser) && o.OfferFromUser.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.Transporter) && o.Transporter.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.TransportPrice) && o.TransportPrice.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.FreightForTransport) && o.FreightForTransport.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.NewTransportPrice) && o.NewTransportPrice.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.Comment) && o.Comment.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.CounterOfferPrice) && o.CounterOfferPrice.ToLower().Contains(searchLower))
                        )
                        .ToList();
                }

                return enrichedDeclinedOffers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving and searching declined offers.", ex);
            }
        }

        public async Task<IEnumerable<OfferListDTO>> SearchTransitOffersAsync(int userId, string searchTerm)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            try
            {
                var userProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
                if (userProfileIds == null || !userProfileIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var userVehicles = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(userProfileIds);
                var userFreights = await _freightRepository.GetFreightIdsByProfileIdsAsync(userProfileIds);

                var allOffers = await _offerRepository.GetOffersAsync();
                if (allOffers == null || !allOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var transitOfferIds = new List<int>();
                foreach (var offer in allOffers)
                {
                    if (await _statusRepository.HasStatusGreaterOrEqualAsync(offer.Id, (int)StatusType.PreperingForTransit))
                    {
                        transitOfferIds.Add(offer.Id);
                    }
                }

                if (!transitOfferIds.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var transitOffers = allOffers
                    .Where(o =>
                        (userProfileIds.Contains(o.OfferFromUserProfileId) && transitOfferIds.Contains(o.Id)) ||
                        (!userProfileIds.Contains(o.OfferFromUserProfileId) &&
                         (userVehicles.Contains(o.VehicleId) || userFreights.Contains(o.FreightId)) &&
                         transitOfferIds.Contains(o.Id)))
                    .ToList();

                if (!transitOffers.Any())
                    return Enumerable.Empty<OfferListDTO>();

                var enrichedOffers = new List<OfferListDTO>();
                foreach (var offer in transitOffers)
                {
                    var enrichedOffer = await _enrichOfferHelper.EnrichOfferAsync(offer);
                    enrichedOffers.Add(enrichedOffer);
                }

                // Apply search filter if searchTerm is provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var searchLower = searchTerm.ToLower();
                    enrichedOffers = enrichedOffers
                        .Where(o =>
                            (!string.IsNullOrEmpty(o.OfferForStuff) && o.OfferForStuff.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferForUser) && o.OfferForUser.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferFromStuff) && o.OfferFromStuff.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.OfferFromUser) && o.OfferFromUser.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.Transporter) && o.Transporter.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.TransportPrice) && o.TransportPrice.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.FreightForTransport) && o.FreightForTransport.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.NewTransportPrice) && o.NewTransportPrice.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.Comment) && o.Comment.ToLower().Contains(searchLower)) ||
                            (!string.IsNullOrEmpty(o.CounterOfferPrice) && o.CounterOfferPrice.ToLower().Contains(searchLower))
                        )
                        .ToList();
                }

                return enrichedOffers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while fetching and searching transit offers.", ex);
            }
        }

    }
}
