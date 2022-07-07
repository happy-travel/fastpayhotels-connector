using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.EdoContracts.General;

namespace HappyTravel.FastpayhotelsConnector.Api.Models.Availability;

public record CachedRoomContractSet(Guid Id,
    in Rate Rate,
    Deadline Deadline,
    List<CachedRoomContract> Rooms,
    List<string> Tags,
    bool IsDirectContract,
    bool IsAdvancePurchaseRate,
    bool IsPackageRate);
