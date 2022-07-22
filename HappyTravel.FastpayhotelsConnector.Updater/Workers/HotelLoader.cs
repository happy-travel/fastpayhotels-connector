using HappyTravel.FastpayhotelsConnector.Common.Models.Hotel;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure.Logging;
using HappyTravel.FastpayhotelsConnector.Updater.Models;
using HappyTravel.FastpayhotelsConnector.Updater.Service;
using HappyTravel.FastpayhotelsConnector.Updater.Settings;
using Microsoft.Extensions.Options;

namespace HappyTravel.FastpayhotelsConnector.Updater.Workers;

public class HotelLoader : IUpdateWorker
{
    public HotelLoader(FastpayhotelsContentClient client, ILogger<HotelLoader> logger, UpdateHistoryService updateHistoryService, HotelUpdater hotelsUpdater, IOptions<RawDataUpdateOptions> options)
    {
        _client = client;
        _logger = logger;
        _updateHistoryService = updateHistoryService;
        _hotelsUpdater = hotelsUpdater;
        _options = options.Value;
    }


    public async Task Run(CancellationToken cancellationToken)
    {
        var updateId = await _updateHistoryService.Create(_options);        
        _logger.LogStartingHotelsUpdate(updateId);

        try
        {
            var lastSuccesUpdateDate = DateTimeOffset.MinValue;
            if (_options.UpdateMode == UpdateMode.Full)
            {
                _logger.LogDeactivateAllHotels();
                await _hotelsUpdater.DeactivateAllHotels(cancellationToken);
            }
            else
            {
                var lastSuccesUpdate = await _updateHistoryService.GetLastSuccessfulUpdateTime();
                lastSuccesUpdateDate = lastSuccesUpdate.Value;
            }

            var hotelList = await _client.GetHotelList(new HotelListRequest(lastSuccesUpdateDate), cancellationToken);

            foreach (var hotelSummary in hotelList.HotelSummary)
            {
                var hotelDetails = await _client.GetHotelDetails(new HotelDetailsRequest(hotelSummary.Code), cancellationToken);
                await _hotelsUpdater.AddUpdateHotel(hotelSummary.Code, hotelDetails.HotelDetail, cancellationToken);
            }

            await _updateHistoryService.SetSuccess(updateId);
        }
        catch (Exception ex)
        {
            _logger.LogHotelsLoaderException(ex);
            await _updateHistoryService.SetError(updateId, ex);
        }

        _logger.LogFinishHotelsUpdate(updateId);
    }    


    private readonly FastpayhotelsContentClient _client;
    private readonly ILogger<HotelLoader> _logger;
    private readonly UpdateHistoryService _updateHistoryService;
    private readonly HotelUpdater _hotelsUpdater;
    private readonly RawDataUpdateOptions _options;
}
