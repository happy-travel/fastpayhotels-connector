using CSharpFunctionalExtensions;
using HappyTravel.BaseConnector.Api.Infrastructure;
using HappyTravel.BaseConnector.Api.Services.Bookings;
using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.EdoContracts.Accommodations.Enums;
using HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability;
using HappyTravel.FastpayhotelsConnector.Api.Models.Booking;
using HappyTravel.FastpayhotelsConnector.Api.Services.Caching;
using Microsoft.AspNetCore.Mvc;
using Booking = HappyTravel.EdoContracts.Accommodations.Booking;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Bookings;

public class BookingService : IBookingService
{
    public BookingService(FastpayhotelsShoppingClient client,
        AvailabilityRequestStorage requestStorage,
        PreBookResultStorage preBookResultStorage,
        BookingManager bookingManager)
    {
        _client = client;
        _requestStorage = requestStorage;
        _preBookResultStorage = preBookResultStorage;
        _bookingManager = bookingManager;
    }


    public async Task<Result<Booking, ProblemDetails>> Book(BookingRequest bookingRequest, CancellationToken cancellationToken)
    {
        return await GetCachedData()
             .Bind(Book)
             .Tap(SaveBooking)
             .Map(ToContract);


        async Task<Result<(AvailabilityRequest, CachedPrebookResult), ProblemDetails>> GetCachedData()
        {
            var availabilityRequest = await _requestStorage.Get(bookingRequest.AvailabilityId);
            if (availabilityRequest.IsFailure)
                return ProblemDetailsBuilder.CreateFailureResult<(AvailabilityRequest, CachedPrebookResult)>(availabilityRequest.Error, BookingFailureCodes.ConnectorValidationFailed);
                       
            var preBookResult = await _preBookResultStorage.Get(bookingRequest.AvailabilityId);
            if (preBookResult.IsFailure)
                return ProblemDetailsBuilder.CreateFailureResult<(AvailabilityRequest, CachedPrebookResult)>(preBookResult.Error, BookingFailureCodes.ConnectorValidationFailed);

            return (availabilityRequest.Value, preBookResult.Value);
        }


        async Task<Result<(AvailabilityRequest, ApiBookingResponse), ProblemDetails>> Book((AvailabilityRequest, CachedPrebookResult) cachedData)
        {
            var (availabilityRequest, preBookResult) = cachedData;

            var (isSuccess, _, booking, error) = await _client.Book(bookingRequest.ToApiBookingRequest(preBookResult), cancellationToken);

            if (isSuccess)
                return (availabilityRequest, booking);

            return ProblemDetailsBuilder.CreateFailureResult<(AvailabilityRequest, ApiBookingResponse)>(error, BookingFailureCodes.RequestException);
        }


        async Task SaveBooking((AvailabilityRequest, ApiBookingResponse) result)
        {
            var (availabilityRequest, booking) = result;

            await _bookingManager.Add(new Data.Models.Booking()
            {
                ReferenceCode = bookingRequest.ReferenceCode,
                BookingCode = booking.Result.BookingInfo.BookingCode,
                CheckInDate = availabilityRequest.CheckInDate,
                CheckOutDate = availabilityRequest.CheckOutDate,
                Rooms = bookingRequest.Rooms
            });
        }            


        Booking ToContract((AvailabilityRequest, ApiBookingResponse) result)
        {
            var (availabilityRequest, booking) = result;

            return BookingMapper.Map(booking, bookingRequest.Rooms, availabilityRequest.CheckInDate, availabilityRequest.CheckOutDate);
        }
    }

    public Task<Result> Cancel(string referenceCode, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Booking>> Get(string referenceCode, CancellationToken cancellationToken)
    {
        return await _bookingManager.Get(referenceCode)
            .Bind(GetBookingDetails)
            .Map(ToContract);


        async Task<Result<(Data.Models.Booking, ApiBookingDetailsResponse)>> GetBookingDetails(Data.Models.Booking booking)
        {
            var bookingDetailsRequest = new ApiBookingDetailsRequest()
            {
                BookingCode = booking.BookingCode,
                CustomerCode = booking.ReferenceCode
            };

            var (isSuccess, _, bookingDetails, error) = await _client.GetBookingDetails(bookingDetailsRequest, cancellationToken);

            if (isSuccess)
                return (booking, bookingDetails);

            return Result.Failure<(Data.Models.Booking, ApiBookingDetailsResponse)>(error);
        }


        Booking ToContract((Data.Models.Booking, ApiBookingDetailsResponse) result)
        {
            var (booking, bookingDetails) = result;

            return BookingMapper.Map(bookingDetails, booking);
        }
    }


    private readonly FastpayhotelsShoppingClient _client;
    private readonly AvailabilityRequestStorage _requestStorage;
    private readonly PreBookResultStorage _preBookResultStorage;
    private readonly BookingManager _bookingManager;
}
