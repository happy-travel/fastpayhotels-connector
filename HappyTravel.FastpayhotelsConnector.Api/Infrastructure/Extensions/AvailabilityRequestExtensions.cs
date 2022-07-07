using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

namespace HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;

public static class AvailabilityRequestExtensions
{
    public static ApiAvailabilityRequest ToApiAvailbilityRequest(this AvailabilityRequest request)
        => new()
        {            
            CheckIn = request.CheckInDate.ToString("yyyy-MM-dd"),
            CheckOut = request.CheckOutDate.ToString("yyyy-MM-dd"),
            Occupancies = request.Rooms
                .Select(r =>
                    new ApiOccupancy()
                    {
                        Adults = r.AdultsNumber,
                        Children = r.ChildrenAges.Count,
                        ChildrenAges = r.ChildrenAges
                    })
                .ToList(),
            HotelCodes = request.AccommodationIds,
            Parameters = new ApiAvailSearchParameters()
            {
                CountryOfResidence = request.Residency,
                Nationality = request.Nationality
            }
        };


    public static int GetNumberOfNights(this AvailabilityRequest request)
        => request.CheckOutDate.Date.Subtract(request.CheckInDate.Date).Days;
}
