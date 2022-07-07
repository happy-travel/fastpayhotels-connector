using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability;

namespace HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;

public static class CachedRoomContractSetExtensions
{
    public static RoomContractSet ToContract(this CachedRoomContractSet roomContractSet)
        => new(id: roomContractSet.Id,
            rate: roomContractSet.Rate,
            deadline: roomContractSet.Deadline,
            rooms: roomContractSet.Rooms
                .Select(r => r.RoomContract)
                .ToList(),
            tags: roomContractSet.Tags,
            isDirectContract: roomContractSet.IsDirectContract,
            isAdvancePurchaseRate: roomContractSet.IsAdvancePurchaseRate,
            isPackageRate: roomContractSet.IsPackageRate);
}
