using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability;

namespace HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;

public static class CachedAccommodationAvailabilityExtensions
{
    public static SlimAccommodationAvailability ToContract(this CachedAccommodationAvailability cachedAccommodationAvailability, string availabilityId)
        => new(accommodationId: cachedAccommodationAvailability.AccommodationId,
            roomContractSets: cachedAccommodationAvailability.CachedRoomContractSets
                .Select(s => s.ToContract())
                .ToList(),
            availabilityId);
}
