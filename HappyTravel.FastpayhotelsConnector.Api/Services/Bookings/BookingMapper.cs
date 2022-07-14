using HappyTravel.EdoContracts.Accommodations.Enums;
using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.EdoContracts.General;
using HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Bookings;

public static class BookingMapper
{
    public static EdoContracts.Accommodations.Booking Map(ApiBookingResponse bookingResponse, List<SlimRoomOccupation> bookingRooms,
        DateTimeOffset checkInDate, DateTimeOffset checkOutDate)
    {
        var booking = bookingResponse.Result.BookingInfo;

        return new EdoContracts.Accommodations.Booking(referenceCode: booking.AgencyCode,
            status: BookingStatusCodes.Confirmed,
            accommodationId: booking.HotelInfo.Code,
            supplierReferenceCode: booking.BookingCode,
            checkInDate: checkInDate,
            checkOutDate: checkOutDate,
            rooms: bookingRooms,
            bookingUpdateMode: BookingUpdateModes.Synchronous,
            specialValues: GetSpecialValues());


        List<KeyValuePair<string, string>> GetSpecialValues()
        {
            var specialValues = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(booking.SpecialNotes))
                specialValues.Add(new("Booking information", booking.SpecialNotes));

            return specialValues;
        }
    }
}
