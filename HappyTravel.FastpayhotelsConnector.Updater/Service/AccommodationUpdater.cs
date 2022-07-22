using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.FastpayhotelsConnector.Data.Models;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure.Logging;
using HappyTravel.FastpayhotelsConnector.Updater.Settings;
using HappyTravel.FastpayhotelsConnector.Updater.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace HappyTravel.FastpayhotelsConnector.Updater.Service;

public class AccommodationUpdater : IUpdateWorker
{
    public AccommodationUpdater(FastpayhotelsContext context, ILogger<AccommodationUpdater> logger, FastpayhotelsSerializer serializer, IOptions<AccommodationDataUpdateOptions> options,
            DateTimeProvider dateTimeProvider)
    {
        _context = context;
        _logger = logger;
        _serializer = serializer;
        _options = options.Value;
        _dateTimeProvider = dateTimeProvider;
    }


    public async Task Run(CancellationToken cancellationToken)
    {
        _logger.LogStartingAccommodationUpdater();

        try
        {
            await foreach (var hotels in GetHotelList(cancellationToken))
            {
                var existHotelIds = hotels.Select(p => p.Code).ToList();
                var existingAccommodationIds = await _context.Accommodations
                    .Where(a => existHotelIds.Contains(a.Code))
                    .Select(a => a.Code)
                    .ToListAsync(cancellationToken);

                foreach (var hotel in hotels)
                {
                    var accommodation = Convert(hotel);
                    if (accommodation is not default(Accommodation))
                    {
                        if (existingAccommodationIds.Contains(hotel.Code))
                            _context.Accommodations.Update(accommodation);
                        else
                            _context.Accommodations.Add(accommodation);
                    }
                }

                //await _context.SaveChangesAsync(cancellationToken);
                //_context.ChangeTracker.Clear();
            }
        }
        catch (Exception ex)
        {
            _logger.LogAccommodationUpdaterException(ex);
        }
    }


    private Accommodation Convert(Hotel hotel)
    {
        var data = _serializer.Deserialize<HotelDetails>(hotel.Data);

        var timezone = GetTimeZone(data.TimeZone);

        if (timezone is null)
        {
            _logger.LogHotelTimezoneError(hotel.Code);
            return default;
        }

        return new Accommodation
        {
            Code = hotel.Code,
            Country = string.Empty,
            Locality = data?.Location?.City,
            Name = data?.Name,
            Coordinates = GetPoint(data?.Location?.Lat, data?.Location?.Long),
            Modified = _dateTimeProvider.UtcNow(),
            Timezone = timezone
        };


        static Point GetPoint(double? latitude, double? longitude)
            => latitude.HasValue && longitude.HasValue
            ? new Point(latitude.Value, longitude.Value)
            : new Point(0, 0);


        static TimeSpan? GetTimeZone(string timezoneString)
        {
            if (timezoneString.Contains("GMT Standard Time"))
            {
                return new TimeSpan();
            }

            var utc = Regex.Match(timezoneString, @"(\+|\-){1}\d{2}:\d{2}").Value;

            bool isNegative = utc.StartsWith("-");

            if (TimeSpan.TryParseExact(utc.Remove(0, 1), "hh:mm", CultureInfo.InvariantCulture, out TimeSpan timezone))
                return isNegative ? timezone.Negate() : timezone;
            else
                return null;
        }
    }


    private async IAsyncEnumerable<List<Hotel>> GetHotelList([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var lastAccommodationsUpdate = await GetDateSinceChanges();
        var i = 0;
        var hotelsCount = await _context.Hotels.CountAsync(cancellationToken: cancellationToken);

        do
        {
            yield return await _context.Hotels
                .Where(r => r.Modified >= lastAccommodationsUpdate && r.IsActive)
                .OrderBy(r => r.Code)
                .Skip(i)
                .Take(BatchSize)
                .ToListAsync(cancellationToken);

            _logger.LogAccommodationUpdaterGetHotels(i, i + BatchSize);
            i += BatchSize;

        } while (i < hotelsCount);
    }


    private async Task<DateTimeOffset> GetDateSinceChanges()
    {
        if (_options.UpdateMode == UpdateMode.Full)
            return DateTimeOffset.MinValue;

        var lastModifiedAccommodation = await _context.Accommodations
            .OrderByDescending(a => a.Modified)
            .LastOrDefaultAsync();

        return lastModifiedAccommodation?.Modified ?? DateTimeOffset.MinValue;
    }


    private const int BatchSize = 1000;

    private readonly FastpayhotelsContext _context;
    private readonly ILogger<AccommodationUpdater> _logger;
    private readonly FastpayhotelsSerializer _serializer;
    private readonly AccommodationDataUpdateOptions _options;
    private readonly DateTimeProvider _dateTimeProvider;
}
