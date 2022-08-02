using HappyTravel.EdoContracts.GeoData;
using HappyTravel.EdoContracts.GeoData.Enums;
using HappyTravel.Geography;
using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Data.Models;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Locations;

public class LocationMapper
{
    public LocationMapper(FastpayhotelsSerializer serializer)
    {
        _serializer = serializer;
    }


    /// <summary>
    /// Create <see cref="Location"/> by <see cref="Accommodation"/>.
    /// </summary>
    /// <param name="accommodation">Accommodation</param>
    /// <returns><see cref="Location"/></returns>
    public Location MapAccommodationToLocation(Accommodation accommodation)
    {
        return new Location(name: GetDefaultLocalizedName(accommodation.Name),
            locality: GetDefaultLocalizedName(accommodation.Locality ?? string.Empty),
            country: GetDefaultLocalizedName(accommodation.Country),
            coordinates: new GeoPoint(accommodation.Coordinates),
            distance: default,
            source: PredictionSources.Interior,
            type: LocationTypes.Accommodation);
    }


    /// <summary>
    /// Create <see cref="Location"/> by country and locality.
    /// </summary>
    /// <param name="country">Country</param>
    /// <param name="locality">Locality</param>
    /// <returns><see cref="Location"/></returns>
    public Location MapLocalityToLocation(string country, string locality)
    {
        return new Location(name: string.Empty,
            locality: GetDefaultLocalizedName(locality),
            country: GetDefaultLocalizedName(country),
            coordinates: default,
            distance: default,
            source: PredictionSources.Interior,
            type: LocationTypes.Location);
    }


    private string GetDefaultLocalizedName(string name)
    {
        var localizedName = new Dictionary<string, string> { { Constants.DefaultLanguageCode, name } };
        return _serializer.Serialize(localizedName);
    }


    private readonly FastpayhotelsSerializer _serializer;
}
