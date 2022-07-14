using HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure.Logging;
using HappyTravel.FastpayhotelsConnector.Updater.Models;
using HappyTravel.FastpayhotelsConnector.Updater.Service;
using HappyTravel.FastpayhotelsConnector.Updater.Settings;
using Microsoft.Extensions.Options;

namespace HappyTravel.FastpayhotelsConnector.Updater.Workers;

public class HotelLoader : IUpdateWorker
{
    public HotelLoader(FastpayhotelsContentClient client, ILogger<HotelLoader> logger, HotelUpdater hotelsUpdater, IOptions<RawDataUpdateOptions> options)
    {
        _client = client;
        _logger = logger;        
        _hotelsUpdater = hotelsUpdater;
        _options = options.Value;
    }


    public async Task Run(CancellationToken cancellationToken)
    {        
        _logger.LogStartingHotelsUpdate();

        try
        {
            var lastSuccesUpdateDate = DateTimeOffset.MinValue;
            if (_options.UpdateMode == UpdateMode.Incremental)
            {
                lastSuccesUpdateDate = await _hotelsUpdater.GetLastUpdateHotelDate(cancellationToken);
            }            

            var hotelList = await _client.GetHotelList(new HotelListRequest(lastSuccesUpdateDate), cancellationToken);

            foreach (var hotelSummary in hotelList.HotelSummary)
            {
                var hotelDetails = await _client.GetHotelDetails(new HotelDetailsRequest(hotelSummary.Code), cancellationToken);
                await _hotelsUpdater.AddUpdateHotel(hotelSummary.Code, hotelDetails.HotelDetail, cancellationToken);
            }            
        }
        catch (Exception ex)
        {
            _logger.LogHotelsLoaderException(ex);            
        }

        _logger.LogFinishHotelsUpdate();
    }    


    private readonly FastpayhotelsContentClient _client;
    private readonly ILogger<HotelLoader> _logger;    
    private readonly HotelUpdater _hotelsUpdater;
    private readonly RawDataUpdateOptions _options;
}
