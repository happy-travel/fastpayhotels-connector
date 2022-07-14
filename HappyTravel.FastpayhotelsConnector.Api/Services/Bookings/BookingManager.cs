using CSharpFunctionalExtensions;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.FastpayhotelsConnector.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Bookings;

public class BookingManager
{
    public BookingManager(FastpayhotelsContext context)
    {
        _context = context;
    }


    public async Task Add(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
    }


    public async Task<Result<Booking>> Get(string referenceCode)
    {
        var booking = await _context.Bookings
            .SingleOrDefaultAsync(b => b.ReferenceCode == referenceCode);

        return booking ?? Result.Failure<Booking>($"Booking `{referenceCode}` not found");
    }


    private readonly FastpayhotelsContext _context;
}
