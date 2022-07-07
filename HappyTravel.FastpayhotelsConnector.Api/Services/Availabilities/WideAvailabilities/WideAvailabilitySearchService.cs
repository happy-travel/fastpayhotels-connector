using CSharpFunctionalExtensions;
using HappyTravel.BaseConnector.Api.Services.Availabilities.WideAvailabilities;
using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;
using HappyTravel.FastpayhotelsConnector.Api.Services.Caching;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.FastpayhotelsConnector.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities.WideAvailabilities;

public class WideAvailabilitySearchService : IWideAvailabilitySearchService
{
    public WideAvailabilitySearchService(FastpayhotelsContext context,
        FastpayhotelsShoppingClient client,
        AvailabilitySearchMapper availabilitySearchMapper,
        AvailabilityRequestStorage requestStorage)
    {
        _context = context;
        _client = client;
        _availabilitySearchMapper = availabilitySearchMapper;
        _requestStorage = requestStorage;
    }


    public async Task<Result<Availability>> Get(AvailabilityRequest request, string languageCode, CancellationToken cancellationToken)
    {
        //var accommodations = await GetAccommodations();
        //var accommodations = new Dictionary<string, Accommodation>() { { "HUS-8669QPHM+HR-E00", new Accommodation() } };
        var accommodations = new Dictionary<string, Accommodation>() { { "HUS-76RX5V5X+29-E00", new Accommodation() } };

        if (!accommodations.Any())
            return new Availability(availabilityId: string.Empty, 
                numberOfNights: request.GetNumberOfNights(),
                checkInDate: request.CheckInDate, 
                checkOutDate:  request.CheckOutDate,
                expiredAfter: DateTimeOffset.MinValue,
                results: new List<SlimAccommodationAvailability>(0));

        return await GetHotelResults()
            .Map(ToContracts)
            .Tap(SaveRequest);


        async Task<Dictionary<string, Accommodation>> GetAccommodations()
        {
            var accommodationCodes = request.AccommodationIds.ToHashSet();
            return await _context.Accommodations
                .Where(acc => accommodationCodes.Contains(acc.Code))
                .ToDictionaryAsync(acc => acc.Code, acc => acc);
        }


        Task<Result<ApiAvailabilityResponse>> GetHotelResults()
            => _client.GetAvailability(request.ToApiAvailbilityRequest(), cancellationToken);


        Task<Availability> ToContracts(ApiAvailabilityResponse response)
            => _availabilitySearchMapper.MapToAvailability(request, response);


        Task SaveRequest(Availability availability)
             => _requestStorage.Set(availability.AvailabilityId, request);
    }


    private readonly FastpayhotelsContext _context;
    private readonly FastpayhotelsShoppingClient _client;
    private readonly AvailabilitySearchMapper _availabilitySearchMapper;
    private readonly AvailabilityRequestStorage _requestStorage;
}
