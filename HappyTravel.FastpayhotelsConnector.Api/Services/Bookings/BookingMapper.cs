using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.EdoContracts.Accommodations.Enums;
using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.FastpayhotelsConnector.Api.Models.Booking;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Bookings;

public static class BookingMapper
{
    public static Booking Map(ApiBookingResponse bookingResponse, List<SlimRoomOccupation> bookingRooms,
        DateTimeOffset checkInDate, DateTimeOffset checkOutDate)
    {
        var booking = bookingResponse.Result.BookingInfo;

        return new Booking(referenceCode: booking.AgencyCode,
            status: BookingStatusCodes.Confirmed,
            accommodationId: booking.HotelInfo.Code,
            supplierReferenceCode: booking.BookingCode,
            checkInDate: checkInDate,
            checkOutDate: checkOutDate,
            rooms: bookingRooms,
            bookingUpdateMode: BookingUpdateModes.Synchronous,
            specialValues: GetSpecialValues(booking.SpecialNotes));
    }


    public static Booking Map(ApiBookingDetailsResponse response, List<SlimRoomOccupation> bookingRooms,
        DateTimeOffset checkInDate, DateTimeOffset checkOutDate)
    {
        var booking = response.Booking;

        return new Booking(referenceCode: booking.CustomerCode,
            status: GetBookingStatus(),
            accommodationId:booking.HotelCode,
            supplierReferenceCode:booking.BookingCode,
            checkInDate:,
            checkOutDate:,
            rooms:bookingRooms,
            bookingUpdateMode: BookingUpdateModes.Synchronous,
            specialValues: GetSpecialValues(booking.SpecialNotes));


        BookingStatusCodes GetBookingStatus()
            => booking.BookingStatus switch
            {
                "CONFIRMED" => BookingStatusCodes.Confirmed,
                "CANCELLED" => BookingStatusCodes.Cancelled,
                _ => BookingStatusCodes.NotFound
            };
    }


    private static List<KeyValuePair<string, string>> GetSpecialValues(string specialNotes)
    {
        var specialValues = new List<KeyValuePair<string, string>>();

        if (!string.IsNullOrWhiteSpace(specialNotes))
            specialValues.Add(new("Booking information", specialNotes));

        return specialValues;
    }
}