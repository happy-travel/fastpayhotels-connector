using CSharpFunctionalExtensions;
using HappyTravel.BaseConnector.Api.Services.Availabilities.RoomContractSetAvailabilities;
using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;
using HappyTravel.FastpayhotelsConnector.Api.Services.Caching;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities.RoomContractSetAvailabilities;

public class RoomContractSetAvailabilityService : IRoomContractSetAvailabilityService
{
    public RoomContractSetAvailabilityService(FastpayhotelsShoppingClient client,
        AvailabilityRequestStorage requestStorage,
        AvailabilitySearchResultStorage availabilitySearchResultStorage,
        PreBookResultStorage preBookResultStorage,
        AvailabilitySearchMapper availabilitySearchMapper)
    {
        _client = client;
        _requestStorage = requestStorage;
        _availabilitySearchResultStorage = availabilitySearchResultStorage;
        _preBookResultStorage = preBookResultStorage;
        _availabilitySearchMapper = availabilitySearchMapper;
    }


    public async Task<Result<RoomContractSetAvailability?>> Get(string availabilityId, Guid roomContractSetId, CancellationToken cancellationToken)
    {
        return await GetRequest()
            .Bind(GetSearchResult)
            .Bind(PreBook)
            .Map(ToContracts);


        Task<Result<AvailabilityRequest>> GetRequest()
            => _requestStorage.Get(availabilityId);


        async Task<Result<(AvailabilityRequest, CachedAccommodationAvailability)>> GetSearchResult(AvailabilityRequest availabilityRequest)
        {
            var (isSuccess, _, searchResult, error) = await _availabilitySearchResultStorage.GetByRoomContractSetId(availabilityId, roomContractSetId);

            if (isSuccess)
                return (availabilityRequest, searchResult);

            return Result.Failure<(AvailabilityRequest, CachedAccommodationAvailability)>(error);
        }


        async Task<Result<(AvailabilityRequest, string, CachedRoomContractSet)>> PreBook((AvailabilityRequest, CachedAccommodationAvailability) result)
        {
            var (request, accommodationAvailability) = result;

            var cachedRoomContractSet = accommodationAvailability.CachedRoomContractSets.Single(r => r.Id == roomContractSetId);

            var rooms = cachedRoomContractSet.Rooms
                .Select(r => r.ReservationToken)
                .GroupBy(r => r)
                .Select(g => new ApiPreBookRqRoom()
                    {
                        AvailToken = g.Key,
                        Quantity = g.Count()
                    })
                .ToList();

            var preBookRequest = new ApiPreBookRequest()
            {
                MessageId = accommodationAvailability.MessageId,
                Rooms = rooms
            };

            var (isSuccess, _, preBookResponse, error) = await _client.PreBook(preBookRequest, cancellationToken);

            if (isSuccess)
            {
                await _preBookResultStorage.Set(availabilityId, new CachedPrebookResult(preBookResponse.MessageId, preBookResponse.Result.ReservationTokens));

                return (request, accommodationAvailability.AccommodationId, cachedRoomContractSet);
            }                

            return Result.Failure<(AvailabilityRequest, string, CachedRoomContractSet)>(error);
        }


        RoomContractSetAvailability? ToContracts((AvailabilityRequest, string, CachedRoomContractSet) result)
        {
            var (request, accommodationId, cachedRoomContractSet) = result;

            return new RoomContractSetAvailability(availabilityId: availabilityId,
                    accommodationId: accommodationId,
                    checkInDate: request.CheckInDate,
                    checkOutDate: request.CheckOutDate,
                    numberOfNights: request.GetNumberOfNights(),
                    roomContractSet: cachedRoomContractSet.ToContract(),
                    creditCardRequirement: GetCreditCardRequirement());


            CreditCardRequirement GetCreditCardRequirement()
            {
                var activationDate = cachedRoomContractSet.Deadline.Date;

                return new CreditCardRequirement(activationDate: activationDate.Value.ToUniversalTime(),
                    dueDate: request.CheckOutDate);
            }
        }
    }


    private readonly FastpayhotelsShoppingClient _client;
    private readonly AvailabilityRequestStorage _requestStorage;
    private readonly AvailabilitySearchResultStorage _availabilitySearchResultStorage;
    private readonly PreBookResultStorage _preBookResultStorage;
    private readonly AvailabilitySearchMapper _availabilitySearchMapper;
}
