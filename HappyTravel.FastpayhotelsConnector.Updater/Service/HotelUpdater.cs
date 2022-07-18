using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HappyTravel.FastpayhotelsConnector.Updater.Service;

public class HotelUpdater
{
    public HotelUpdater(FastpayhotelsContext context, FastpayhotelsSerializer serializer, DateTimeProvider dateTimeProvider)
    {
        _context = context;
        _serializer = serializer;
        _dateTimeProvider = dateTimeProvider;
    }


    public async Task AddUpdateHotel(string code, HotelDetails hotel, CancellationToken cancellationToken)
    {
        var hotelFromDb = await _context.Hotels.FindAsync(code);

        if (hotelFromDb is not null)
        {
            hotelFromDb.Data = _serializer.Serialize(hotel);
            hotelFromDb.Modified = _dateTimeProvider.UtcNow();
            hotelFromDb.IsActive = true;

            _context.Update(hotelFromDb);
        }
        else
        {
            _context.Hotels.Add(new Data.Models.Hotel
            {
                Code = code,
                Data = _serializer.Serialize(hotel),
                Modified = _dateTimeProvider.UtcNow(),
                IsActive = true
            });
        }

        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }    


    public async Task<DateTimeOffset> GetLastUpdateHotelDate(CancellationToken cancellationToken)
    {
        var lastUpdateHotel = await _context.Hotels.OrderBy(x => x.Modified).FirstAsync(x => x.IsActive, cancellationToken);

        return lastUpdateHotel.Modified;
    }


    public async Task<int> DeactivateNotFetched(DateTimeOffset modified, CancellationToken cancellationToken)
       => await _context.Database.ExecuteSqlInterpolatedAsync(
            @$"UPDATE ""Hotels"" SET ""IsActive"" = false WHERE ""IsActive"" = true AND ""Modified"" < {modified}",
            cancellationToken);


    private readonly FastpayhotelsContext _context;
    private readonly FastpayhotelsSerializer _serializer;
    private readonly DateTimeProvider _dateTimeProvider;
}
