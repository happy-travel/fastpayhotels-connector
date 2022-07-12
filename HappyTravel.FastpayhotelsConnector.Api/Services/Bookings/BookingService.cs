using CSharpFunctionalExtensions;
using HappyTravel.BaseConnector.Api.Services.Bookings;
using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.FastpayhotelsConnector.Api.Models.Booking;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.FastpayhotelsConnector.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Booking = HappyTravel.EdoContracts.Accommodations.Booking;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Bookings;

public class BookingService : IBookingService
{
    public Task<Result<Booking, ProblemDetails>> Book(BookingRequest bookingRequest, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Cancel(string referenceCode, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Booking>> Get(string referenceCode, CancellationToken cancellationToken)
    {
        return GetBookingCode()
            .Bind(GetBookingDetails)
            .Map(MapToContract);


        async Task<Result<BookingCodeMapping>> GetBookingCode()
        {
            var bookingCodeMapping = await _context.BookingCodeMappings.FindAsync(referenceCode);

            if (bookingCodeMapping is not null)
                return bookingCodeMapping;

            return Result.Failure<BookingCodeMapping>($"Booking `{referenceCode}` not found");
        }


        async Task<Result<(BookingDetailsResponse, BookingCodeMapping)>> GetBookingDetails(BookingCodeMapping bookingCodeMapping)
        {
            var bookingDetailsRequest = new BookingDetailsRequest(bookingCodeMapping.BookingCode);

            var (isSuccess, _, bookingDetails, error) = await _client.GetBookingDetails(bookingDetailsRequest, cancellationToken);

            if (isSuccess)
                return (bookingDetails, bookingCodeMapping);

            return Result.Failure<(BookingDetailsResponse, BookingCodeMapping)> (error);
        }


        Booking MapToContract((BookingDetailsResponse, BookingCodeMapping) result)
        {
            var (response, bookingCodeMapping) = result;

            return _bookingMapper.Map(response, bookingCodeMapping);
        }
    }


    private readonly FastpayhotelsShoppingClient _client;
    private readonly FastpayhotelsContext _context;
    private readonly BookingMapper _bookingMapper;
}
