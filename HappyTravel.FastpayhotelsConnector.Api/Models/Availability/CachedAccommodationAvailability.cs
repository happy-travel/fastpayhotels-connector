namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability;

public record CachedAccommodationAvailability(string AccommodationId, string MessageId, List<CachedRoomContractSet> CachedRoomContractSets);
