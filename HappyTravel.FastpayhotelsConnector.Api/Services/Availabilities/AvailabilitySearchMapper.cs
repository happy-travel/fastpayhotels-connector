using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.EdoContracts.Accommodations.Enums;
using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.EdoContracts.General;
using HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;
using HappyTravel.FastpayhotelsConnector.Api.Services.Caching;
using HappyTravel.Money.Enums;
using HappyTravel.Money.Models;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities;

public class AvailabilitySearchMapper
{
    public AvailabilitySearchMapper(AvailabilitySearchResultStorage availabilitySearchResultStorage,
        TimezoneService timezoneService)
    {
        _availabilitySearchResultStorage = availabilitySearchResultStorage;
        _timezoneService = timezoneService;
    }


    public async Task<Availability> MapToAvailability(AvailabilityRequest availabilityRequest, ApiAvailabilityResponse response)
    {
        var numberOfNights = availabilityRequest.GetNumberOfNights();

        var availabilityId = Guid.NewGuid().ToString();

        if (!response.HotelAvails.Any())
            return new Availability(availabilityId: availabilityId,
                numberOfNights: numberOfNights,
                checkInDate: availabilityRequest.CheckInDate,
                checkOutDate: availabilityRequest.CheckOutDate,
                expiredAfter: DateTimeOffset.MinValue,
                new List<SlimAccommodationAvailability>(0));

        var slimAccommodationAvailabilities = new List<SlimAccommodationAvailability>();
        var cachedAccommodationAvailabilities = new List<CachedAccommodationAvailability>();

        foreach (var hotelAvail in response.HotelAvails)
        {
            //var hotelTimezone = await _timezoneService.GetTimezone(hotelAvail.HotelInfo.Code);
            var hotelTimezone = TimeSpan.FromHours(5); // For testing

            var cachedAccommodationAvailability = new CachedAccommodationAvailability(
                AccommodationId: hotelAvail.HotelInfo.Code,
                MessageId: response.MessageId.ToString(),
                CachedRoomContractSets: MapRooms(hotelAvail, availabilityRequest, numberOfNights, hotelTimezone));

            slimAccommodationAvailabilities.Add(cachedAccommodationAvailability.ToContract(availabilityId));
            cachedAccommodationAvailabilities.Add(cachedAccommodationAvailability);
        }

        await _availabilitySearchResultStorage.Set(availabilityId, cachedAccommodationAvailabilities);

        return new Availability(availabilityId: availabilityId,
            numberOfNights: numberOfNights,
            checkInDate: availabilityRequest.CheckInDate,
            checkOutDate: availabilityRequest.CheckOutDate,
            expiredAfter: DateTimeOffset.MinValue,
            results: slimAccommodationAvailabilities);
    }


    private List<CachedRoomContractSet> MapRooms(ApiHotelAvail hotelAvail, AvailabilityRequest availabilityRequest, int numberOfNights,  TimeSpan hotelTimezone)
        => availabilityRequest.Rooms.Count == 1
            ? GetForSingleRoom(hotelAvail, availabilityRequest, numberOfNights, hotelTimezone)
            : GetForMultipleRooms(hotelAvail, availabilityRequest, numberOfNights, hotelTimezone);    


    private List<CachedRoomContractSet> GetForSingleRoom(ApiHotelAvail hotelAvail, AvailabilityRequest availabilityRequest, int numberOfNights, TimeSpan hotelTimezone)
    {
        var roomOccupationRequest = availabilityRequest.Rooms.Single();
        var adultsNumber = roomOccupationRequest.AdultsNumber; // Because AvailRoomRate.Occupancy is null for single room request
        var childrenAges = roomOccupationRequest.ChildrenAges; // Because AvailRoomRate.Occupancy is null for single room request

        return hotelAvail.AvailRoomRates
            .Select(r =>
            {
                var id = Guid.NewGuid();
                return CreateRoomContractSet(id,
                    new List<CachedRoomContract>()
                    {
                            CreateRoomContract(r, availabilityRequest.CheckInDate, adultsNumber, childrenAges, numberOfNights, hotelTimezone)
                    });
            })
           .ToList();
    }
       


    private List<CachedRoomContractSet> GetForMultipleRooms(ApiHotelAvail hotelAvail, AvailabilityRequest availabilityRequest, int numberOfNights, TimeSpan hotelTimezone)
    {
        var cachedRoomContractSets = new List<CachedRoomContractSet>();

        var roomGroups = GetRoomGroups();

        if (roomGroups is null)
            return new();

        if (EvaluateRoomContractSetsCount() <= Constants.RoomContractSetsMaxCount)
            return GetAllRoomContractSets();
        else
            return GetRoomContractSetsSelection();


        Dictionary<int, List<ApiAvailRoomRate>>? GetRoomGroups()
        {
            var roomGroups = new Dictionary<int, List<ApiAvailRoomRate>>();
            
            var roomOccupationGroups = availabilityRequest.Rooms
                .Select(r => RoomOccupationToString(r))
                .GroupBy(d => d)
                .ToList();

            var roomIndex = 0;            
            foreach (var roomOccupations in roomOccupationGroups)
            {
                var equalRoomOccupationsCount = roomOccupations.Count();

                var roomRateGroup = new List<ApiAvailRoomRate>();
                // If all the requested rooms have the same guests, null will be returned in the Occupancy property.
                if (roomOccupationGroups.Count == 1 && hotelAvail.AvailRoomRates.All(r => r.Occupancy is null)) 
                    roomRateGroup = hotelAvail.AvailRoomRates;
                else  
                    roomRateGroup = hotelAvail.AvailRoomRates
                        .Where(r => ApiOccupancyToString(r.Occupancy) == roomOccupations.Key)
                        .ToList();
                if (!roomRateGroup.Any())
                    return default;
                
                var preparedRoomRates = new List<ApiAvailRoomRate>();
                foreach (var roomRate in roomRateGroup)
                {
                    var clonedRoomsCount = equalRoomOccupationsCount <= roomRate.Inventory
                    ? equalRoomOccupationsCount
                    : roomRate.Inventory;

                    for (int i = 0; i < clonedRoomsCount; i++)
                    {
                        preparedRoomRates.Add(roomRate);
                    }
                }

                if (preparedRoomRates.Count < equalRoomOccupationsCount)
                    return default;

                var currentRoomIndex = roomIndex;
                for (int i = 0; i < equalRoomOccupationsCount; i++)
                {   
                    roomGroups[roomIndex++] = new List<ApiAvailRoomRate>();
                }

                var idx = currentRoomIndex;
                foreach (var roomRate in preparedRoomRates)
                {
                    if (idx >= currentRoomIndex + equalRoomOccupationsCount)
                        idx = currentRoomIndex;

                    roomGroups[idx].Add(roomRate);
                    idx++;
                }
            }

            return roomGroups;


            string RoomOccupationToString(RoomOccupationRequest roomOccupation)
                => string.Format("{0}{1}",
                    roomOccupation.AdultsNumber,
                    roomOccupation.ChildrenAges is not null && roomOccupation.ChildrenAges.Count > 0
                        ? $"-{string.Join(',', roomOccupation.ChildrenAges.OrderBy(c => c))}"
                        : string.Empty);


            string ApiOccupancyToString(ApiOccupancy occupancy)
                => string.Format("{0}{1}",
                    occupancy.Adults,
                    occupancy.ChildrenAges is not null && occupancy.ChildrenAges.Count > 0
                        ? $"-{string.Join(',', occupancy.ChildrenAges.OrderBy(c => c))}"
                        : string.Empty);
        }


        int EvaluateRoomContractSetsCount()
        {
            int roomContractSetsCount = 1;

            foreach (var roomGroup in roomGroups)
            {
                roomContractSetsCount *= roomGroup.Value.Count;
            }

            return roomContractSetsCount;
        }


        List<CachedRoomContractSet> GetAllRoomContractSets()
        {
            var roomCombinations = new List<List<CachedRoomContract>>();

            foreach (var roomGroup in roomGroups)
            {
                if (roomCombinations.Count == 0)
                {
                    foreach (var roomRate in roomGroup.Value)
                    {
                        var adultsNumber = roomRate.Occupancy?.Adults ?? availabilityRequest.Rooms.First().AdultsNumber;
                        var childrenAges = roomRate.Occupancy?.ChildrenAges ?? availabilityRequest.Rooms.First().ChildrenAges;

                        roomCombinations.Add(new()
                        {
                            CreateRoomContract(roomRate, availabilityRequest.CheckInDate, adultsNumber, childrenAges, numberOfNights, hotelTimezone)
                        });
                    }
                }
                else
                {
                    var roomCombinationsBuffer = new List<List<CachedRoomContract>>();

                    foreach (var roomCombination in roomCombinations)
                    {
                        foreach (var roomRate in roomGroup.Value)
                        {
                            var adultsNumber = roomRate.Occupancy?.Adults ?? availabilityRequest.Rooms.First().AdultsNumber;
                            var childrenAges = roomRate.Occupancy?.ChildrenAges ?? availabilityRequest.Rooms.First().ChildrenAges;

                            var roomCombinationBuffer = new List<CachedRoomContract>();

                            roomCombinationBuffer.AddRange(roomCombination);
                            roomCombinationBuffer.Add(CreateRoomContract(roomRate, availabilityRequest.CheckInDate, adultsNumber, childrenAges, numberOfNights, hotelTimezone));

                            roomCombinationsBuffer.Add(roomCombinationBuffer);
                        }
                    }

                    roomCombinations = roomCombinationsBuffer;
                }
            }

            var roomContractSets = new List<CachedRoomContractSet>();
            foreach (var roomCombination in roomCombinations)
            {
                var id = Guid.NewGuid();
                roomContractSets.Add(CreateRoomContractSet(id, roomCombination));
            }

            return roomContractSets;
        }


        List<CachedRoomContractSet> GetRoomContractSetsSelection()
        {
            var roomContractSets = new List<CachedRoomContractSet>();

            var largestGroup = roomGroups
                .OrderByDescending(g => g.Value.Count)
                .First();

            var otherGroups = roomGroups
                .Where(g => g.Key != largestGroup.Key)
                .ToList();

            for (var i = 0; i < largestGroup.Value.Count; i++)
            {
                var roomContracts = new List<CachedRoomContract>();
                var roomRate = largestGroup.Value[i];

                var adultsNumber = roomRate.Occupancy?.Adults ?? availabilityRequest.Rooms.First().AdultsNumber;
                var childrenAges = roomRate.Occupancy?.ChildrenAges ?? availabilityRequest.Rooms.First().ChildrenAges;

                roomContracts.Add(CreateRoomContract(largestGroup.Value[i], availabilityRequest.CheckInDate, adultsNumber, childrenAges, numberOfNights, hotelTimezone));

                foreach (var otherGroup in otherGroups)
                {
                    var index = otherGroup.Value.Count > i
                        ? i
                        : i % otherGroup.Value.Count;

                    var apiRoomRate = otherGroup.Value[index];

                    var apiAdultsNumber = apiRoomRate.Occupancy?.Adults ?? availabilityRequest.Rooms.First().AdultsNumber;
                    var apiChildrenAges = apiRoomRate.Occupancy?.ChildrenAges ?? availabilityRequest.Rooms.First().ChildrenAges;

                    roomContracts.Add(CreateRoomContract(apiRoomRate, availabilityRequest.CheckInDate, apiAdultsNumber, apiChildrenAges, numberOfNights, hotelTimezone));
                }

                var id = Guid.NewGuid();
                roomContractSets.Add(CreateRoomContractSet(id, roomContracts));
            }

            return roomContractSets;
        }
    }


    private CachedRoomContract CreateRoomContract(ApiAvailRoomRate roomRate, DateTimeOffset checkInDate, int adultsNumber, List<int> childrenAges, int numberOfNights, TimeSpan hotelTimezone)
    {
        if (!Enum.TryParse(roomRate.Currency, out Currencies rateCurrency))
            throw new NotSupportedException($"Currency '{roomRate.Currency}' is not supported");

        var price = roomRate.PriceBinding
            ? roomRate.PublicPrice
            : roomRate.TotalPrice;

        return new CachedRoomContract(ReservationToken: roomRate.ReservationToken,
            RoomContract: new RoomContract(
               boardBasis: GetBoardBasis(),
               mealPlan: roomRate.MealPlanName,
               contractTypeCode: default,
               isAvailableImmediately: true,
               isDynamic: false,
               contractDescription: roomRate.RoomName,
               remarks: GetRemarks(),
               dailyRoomRates: new List<DailyRate>(),
               rate: new Rate(new MoneyAmount(price, rateCurrency), new MoneyAmount(price, rateCurrency)),
               adultsNumber: adultsNumber,
               childrenAges: childrenAges,
               type: RoomTypes.NotSpecified,
               isExtraBedNeeded: false,
               deadline: DeadlineMapper.GetDeadline(roomRate.CancellationPolicy, roomRate.PricePerDay, price, numberOfNights,  checkInDate, hotelTimezone),
               isAdvancePurchaseRate: !roomRate.CancellationPolicy.Cancellable));


        BoardBasisTypes GetBoardBasis()
            => roomRate.MealPlanName switch
            {
                "Room Only" => BoardBasisTypes.RoomOnly,
                "Self Catering" => BoardBasisTypes.SelfCatering,
                "Bed & Breakfast" => BoardBasisTypes.BedAndBreakfast,
                "American Breakfast" => BoardBasisTypes.BedAndBreakfast,
                "Brunch" => BoardBasisTypes.BedAndBreakfast,
                "Buffet Breakfast" => BoardBasisTypes.BedAndBreakfast,
                "Continental Breakfast" => BoardBasisTypes.BedAndBreakfast,
                "English Breakfast" => BoardBasisTypes.BedAndBreakfast,
                "Half Board" => BoardBasisTypes.HalfBoard,
                "Dinner only" => BoardBasisTypes.HalfBoard,
                "Half Board (BB & Dinner)" => BoardBasisTypes.HalfBoard,
                "Half Board (BB & Lunch)" => BoardBasisTypes.HalfBoard,
                "Half Board Premium" => BoardBasisTypes.HalfBoard,
                "Lunch Only" => BoardBasisTypes.HalfBoard,
                "Full Board" => BoardBasisTypes.FullBoard,
                "Full Board Plus" => BoardBasisTypes.FullBoard,
                "All Inclusive" => BoardBasisTypes.AllInclusive,
                "All Inclusive Plus" => BoardBasisTypes.AllInclusive,
                "All Inclusive Soft" => BoardBasisTypes.AllInclusive,
                _ => BoardBasisTypes.NotSpecified
            };


        List<KeyValuePair<string, string>> GetRemarks()
        {
            var remarks = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(roomRate.SpecialNotes))
                remarks.Add(new("Hotel remarks", roomRate.SpecialNotes));

            return remarks;
        }
    }


    private CachedRoomContractSet CreateRoomContractSet(Guid roomContractSetId, List<CachedRoomContract> roomContracts)
    {
        return new CachedRoomContractSet(
            Id: roomContractSetId,
            Rate: GetTotalRate(),
            Deadline: GetRoomContractSetDeadline(),
            Rooms: roomContracts,
            Tags: new List<string>(),
            IsDirectContract: false,
            IsAdvancePurchaseRate: false,
            IsPackageRate: false);


        Rate GetTotalRate()
        {
            var final = roomContracts.Sum(r => r.RoomContract.Rate.Gross.Amount);
            var currency = roomContracts.First().RoomContract.Rate.Currency;

            return new(new MoneyAmount(final, currency), new MoneyAmount(final, currency));
        }


        Deadline GetRoomContractSetDeadline()
        {
            var deadlineDate = roomContracts
                .Select(r => r.RoomContract.Deadline.Date)
                .Where(d => d is not null)
                .OrderBy(d => d)
                .FirstOrDefault();

            return new(deadlineDate);
        }
    }


    private readonly AvailabilitySearchResultStorage _availabilitySearchResultStorage;
    private readonly TimezoneService _timezoneService;
}
