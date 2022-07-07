namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability;

public record CachedAccommodationAvailability(string AccommodationId, List<CachedRoomContractSet> CachedRoomContractSets);
