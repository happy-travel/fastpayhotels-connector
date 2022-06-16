using CSharpFunctionalExtensions;
using HappyTravel.BaseConnector.Api.Services.Availabilities.RoomContractSetAvailabilities;
using HappyTravel.EdoContracts.Accommodations;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities.RoomContractSetAvailabilities;

public class RoomContractSetAvailabilityService : IRoomContractSetAvailabilityService
{
    public Task<Result<RoomContractSetAvailability?>> Get(string availabilityId, Guid roomContractSetId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
