using HappyTravel.BaseConnector.Api.Services.Accommodations;
using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.FastpayhotelsConnector.Data;
using Microsoft.EntityFrameworkCore;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Accommodations;

public class AccommodationService : IAccommodationService
{
    public AccommodationService(FastpayhotelsContext context, MultilingualAccommodationMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<List<MultilingualAccommodation>> Get(int skip, int top, DateTimeOffset? modificationDate, CancellationToken cancellationToken)
    {
        var hotels = _context.Hotels.AsQueryable();

        if (modificationDate is not null)
            hotels = hotels.Where(a => a.Modified >= modificationDate);

        return await hotels
            .OrderBy(h => h.Code)
            .Skip(skip)
            .Take(top)
            .Select(h => _mapper.Map(h))
            .ToListAsync(cancellationToken);
    }


    private readonly FastpayhotelsContext _context;
    private readonly MultilingualAccommodationMapper _mapper;
}
