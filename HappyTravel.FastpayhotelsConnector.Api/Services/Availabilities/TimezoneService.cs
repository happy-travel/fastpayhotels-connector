using HappyTravel.FastpayhotelsConnector.Data;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities;

public class TimezoneService
{
    public TimezoneService(FastpayhotelsContext context)
    {
        _context = context;
    }


    public async Task<TimeSpan> GetTimezone(string accommodationId)
    {
        var accommodation = await _context.Accommodations.FindAsync(accommodationId);

        return accommodation.Timezone.Value;
    }


    private readonly FastpayhotelsContext _context;
}
