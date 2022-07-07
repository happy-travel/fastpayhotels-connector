using HappyTravel.EdoContracts.Accommodations.Internals;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability;

public record CachedRoomContract(string ReservationToken, RoomContract RoomContract);
