using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability;
using HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

namespace HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;

public static class BookingRequestExtensions
{
    public static ApiBookingRequest ToApiBookingRequest(this BookingRequest request, CachedPrebookResult prebookResult)
    {
        var leader = request.Rooms
            .SelectMany(r => r.Passengers)
            .Single(p => p.IsLeader);

        return new ApiBookingRequest()
        {
            MessageId = prebookResult.MessageId,
            AgencyCode = request.ReferenceCode,
            Customer = new ApiCustomer()
            {
                FirstName = leader.FirstName,
                LastName = leader.LastName
            },
            Rooms = request.Rooms
                .Select((r, i) => new ApiBookingRqRoom()
                {
                    Paxes = r.Passengers
                            .Select(p => new ApiPax()
                            {
                                FirstName = p.FirstName,
                                LastName = p.LastName,
                                Age = p.Age
                            })
                            .ToList(),
                    ReservationToken = prebookResult.ReservationTokens[i]
                })
                .ToList()
        };
    }
}
