using HappyTravel.BaseConnector.Api.Services.Locations;
using HappyTravel.EdoContracts.GeoData;
using HappyTravel.EdoContracts.GeoData.Enums;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Locations;

public class LocationService : ILocationService
{
    public Task<List<Location>> Get(DateTimeOffset modified, LocationTypes locationType, int skip, int top, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
