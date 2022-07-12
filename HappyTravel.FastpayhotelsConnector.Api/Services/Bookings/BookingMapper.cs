using HappyTravel.EdoContracts.Accommodations.Enums;
using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.EdoContracts.General.Enums;
using HappyTravel.FastpayhotelsConnector.Api.Models.Booking;
using HappyTravel.FastpayhotelsConnector.Data.Models;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Bookings;

public class BookingMapper
{
    public EdoContracts.Accommodations.Booking Map(BookingDetailsResponse response, BookingCodeMapping bookingCodeMapping)
    {
        var booking = response.Booking;

        return new EdoContracts.Accommodations.Booking(referenceCode: booking.BookingCode,
            status: GetBookingStatus(booking.BookingStatus),
            accommodationId: booking.HotelCode,
            supplierReferenceCode: booking.CustomerCode,
            checkInDate: booking.CheckIn.ToUniversalTime(),
            checkOutDate: booking.CheckOut.ToUniversalTime(),
            rooms: GetRooms(),
            bookingUpdateMode: BookingUpdateModes.Synchronous,
            specialValues: new List<KeyValuePair<string, string>>());


        BookingStatusCodes GetBookingStatus(string bookingStatus)
            => bookingStatus switch
            {
                "CONFIRMED" => BookingStatusCodes.Confirmed,
                "CANCELLED" => BookingStatusCodes.Cancelled,
                _ => BookingStatusCodes.NotFound
            };
        


        List<SlimRoomOccupation> GetRooms()
        {
            return bookingCodeMapping.BookingRooms.Select(r =>
                new SlimRoomOccupation(
                    type: RoomTypes.NotSpecified,
                    passengers: r.Paxes.Select(p =>
                        new EdoContracts.General.Pax(title: GetPassengerTitle(p.Title),
                            lastName: p.LastName,
                            firstName: p.FirstName))
                        .ToList(),
                    supplierRoomReferenceCode: r.RoomCode))
                .ToList();


            PassengerTitles GetPassengerTitle(string prefix)
                => prefix switch
                {
                    "Mr." => PassengerTitles.Mr,
                    "Mrs." => PassengerTitles.Mrs,
                    "Ms." => PassengerTitles.Ms,
                    _ => PassengerTitles.Unspecified
                };
        }
    }
}