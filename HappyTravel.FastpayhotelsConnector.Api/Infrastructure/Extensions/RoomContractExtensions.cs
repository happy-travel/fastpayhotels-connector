using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability;

namespace HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;

public static class RoomContractExtensions
{
    public static CachedRoomContract ToContract(this RoomContract roomContract, string reservationToken)
        => new(reservationToken, roomContract);
}
