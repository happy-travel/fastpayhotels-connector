using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.EdoContracts.Accommodations.Enums;
using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Common.Models;
using HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;
using HappyTravel.Geography;
using HappyTravel.MultiLanguage;
using System.Globalization;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Accommodations;

public class MultilingualAccommodationMapper
{
    public MultilingualAccommodationMapper(FastpayhotelsSerializer serializer)
    {
        _serializer = serializer;
    }


    /// <summary>
    /// Create <see cref="MultilingualAccommodation"/> by supplier hotel data.
    /// </summary>
    /// <param name="hotel">The hotel data received from the supplier</param>
    /// <returns><see cref="MultilingualAccommodation"/></returns>
    public MultilingualAccommodation Map(Data.Models.Hotel hotel)
    {
        var deserializedData = _serializer.Deserialize<HotelDetails>(hotel.Data);
        
        return new MultilingualAccommodation(
            supplierCode: hotel.Code,
            name: GetMultiLingualName(deserializedData),
            accommodationAmenities: GetMultilingualAmenities(deserializedData),
            additionalInfo: new MultiLanguage<Dictionary<string, string>>(),
            category: GetMultiLingualCategory(deserializedData),
            contacts: GetContacts(deserializedData),
            location: GetMultilingualLocationInfo(deserializedData),
            photos: GetImages(deserializedData),
            rating: GetRating(deserializedData),
            schedule: GetScheduleInfo(deserializedData),
            textualDescriptions: GetMultiLingualDescription(deserializedData),
            type: PropertyTypes.Hotels,
            isActive: hotel.IsActive,
            hotelChain: deserializedData.ChainName
        );
    }


    private MultiLanguage<string> GetMultiLingualName(HotelDetails hotelDetails)
    {
        var multilingualName = new MultiLanguage<string>();
        multilingualName.TrySetValue(Constants.DefaultLanguageCode, hotelDetails.Name);
        return multilingualName;
    }


    private MultiLanguage<List<string>> GetMultilingualAmenities(HotelDetails hotelDetails)
    {
        var multilingualAmenities = new MultiLanguage<List<string>>();
        var hotelServices = hotelDetails.Services;
        var amenities = new List<string>();

        if (hotelServices is null)
            return multilingualAmenities;

        foreach (var service in hotelServices)
        {
            amenities.Add(service.Name);
        }

        multilingualAmenities.TrySetValue(Constants.DefaultLanguageCode, amenities);

        return multilingualAmenities;
    }


    private MultiLanguage<string> GetMultiLingualCategory(HotelDetails hotelDetails)
    {
        var multilingualCategory = new MultiLanguage<string>();
        multilingualCategory.TrySetValue(Constants.DefaultLanguageCode, hotelDetails.HotelCategory.Name);
        return multilingualCategory;
    }


    private ContactInfo GetContacts(HotelDetails hotelDetails)
    {
        var hotelPhones = new List<string>();

        if (!string.IsNullOrEmpty(hotelDetails.Phone))
            hotelPhones.Add(hotelDetails.Phone);

        return new ContactInfo(
            emails: new List<string>(0),
            phones: hotelPhones,
            webSites: new List<string>(0),
            faxes: new List<string>(0)
        );
    }


    private MultilingualLocationInfo GetMultilingualLocationInfo(HotelDetails hotelDetails)
    {
        var countryCode = Constants.CountryCodes.FirstOrDefault(x => x.Key.ToLowerInvariant() == hotelDetails.Location.Country.ToLowerInvariant()).Value ?? string.Empty;

        var multilingualCountry = new MultiLanguage<string>();
        multilingualCountry.TrySetValue(Constants.DefaultLanguageCode, hotelDetails.Location.Country ?? string.Empty);

        var multilingualLocalityName = new MultiLanguage<string>();
        multilingualLocalityName.TrySetValue(Constants.DefaultLanguageCode, hotelDetails.Location.City ?? string.Empty);

        var multilingualAddress = new MultiLanguage<string>();
        multilingualAddress.TrySetValue(Constants.DefaultLanguageCode, hotelDetails.Location.Address ?? string.Empty);

        var coordinates = new GeoPoint(hotelDetails.Location.Long, hotelDetails.Location.Lat);

        var locationDescriptionCode = GetLocationDescriptionCode(hotelDetails.HotelType);

        return new MultilingualLocationInfo(
            countryCode: countryCode,
            country: multilingualCountry,
            locality: multilingualLocalityName,
            localityZone: null,
            address: multilingualAddress,
            coordinates: coordinates,
            locationDescriptionCode: locationDescriptionCode,
            pointsOfInterests: new List<PoiInfo>());


        LocationDescriptionCodes GetLocationDescriptionCode(RootType hotelType)
        {
            if (hotelType is null)
                return LocationDescriptionCodes.Unspecified;

            switch (hotelType.Name)
            {
                case "City":
                    return LocationDescriptionCodes.City;
                case "Boutique":
                    return LocationDescriptionCodes.Boutique;
                case "Mountain":
                    return LocationDescriptionCodes.Mountains;
                case "Airport":
                    return LocationDescriptionCodes.Airport;
                default:
                    return LocationDescriptionCodes.Unspecified;
            }
        }
    }


    static List<ImageInfo> GetImages(HotelDetails hotelDetails)
    {
        return hotelDetails.Images
            .Select(img => new ImageInfo(img.Url, GetImageCaption(img)))
            .ToList();


        string GetImageCaption(Image image)
        {
            var captions = image.Captions;

            if (captions is null)
            {
                return string.Empty;
            }
            else
            {
                var caption = captions.FirstOrDefault(c => c.Lang == Constants.DefaultLanguageCode);
                string captionName = caption is not null
                    ? caption.Content
                    : string.Empty;
                return captionName;
            }
        }
    }


    private static AccommodationRatings GetRating(HotelDetails hotelDetails)
        => hotelDetails.HotelCategory.Code switch
        {
            "0" => AccommodationRatings.NotRated,
            "1" => AccommodationRatings.OneStar,
            "2" => AccommodationRatings.TwoStars,
            "3" => AccommodationRatings.ThreeStars,
            "4" => AccommodationRatings.FourStars,
            "5" => AccommodationRatings.FiveStars,
            _ => AccommodationRatings.NotRated
        };

    
    private ScheduleInfo GetScheduleInfo(HotelDetails hotelDetails)
    {
        return new ScheduleInfo(
            checkInTime: hotelDetails.CheckinHour,
            checkOutTime: hotelDetails.CheckoutHour
        );
    }


    private List<MultilingualTextualDescription> GetMultiLingualDescription(HotelDetails hotelDetails)
    {
        var multilingualTextualDescription = new List<MultilingualTextualDescription>();

        if (hotelDetails.Descriptions is not null)
        {
            var multilingualDescription = new MultiLanguage<string>();

            foreach (var description in hotelDetails.Descriptions)
            {
                if (Constants.Languages.Contains(description.Language) && !string.IsNullOrEmpty(description.Content))
                    multilingualDescription.TrySetValue(description.Language, description.Content);
            }

            multilingualTextualDescription.Add(new MultilingualTextualDescription(TextualDescriptionTypes.General, multilingualDescription));
        }

        if (hotelDetails.Rooms is not null)
        {
            foreach (var room in hotelDetails.Rooms)
            {
                if (room.Descriptions is not null)
                {
                    var multilingualDescription = new MultiLanguage<string>();

                    foreach (var description in room.Descriptions)
                    {
                        string langugage = description.Language ?? Constants.DefaultLanguageCode;

                        if ((Constants.Languages.Contains(langugage) || description.Language is null) && !string.IsNullOrEmpty(description.Content))
                        {
                            multilingualDescription.TrySetValue(langugage, description.Content);
                            multilingualTextualDescription.Add(new MultilingualTextualDescription(TextualDescriptionTypes.Room, multilingualDescription));
                        }
                    }
                }
            }
        }

        return multilingualTextualDescription;
    }


    private readonly FastpayhotelsSerializer _serializer;
}