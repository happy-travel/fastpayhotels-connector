using CSharpFunctionalExtensions;
using FloxDc.CacheFlow;
using FloxDc.CacheFlow.Extensions;
using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Logging;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Caching;

public class AvailabilityRequestStorage
{
    public AvailabilityRequestStorage(IDoubleFlow flow, ILogger<AvailabilityRequestStorage> logger)
    {
        _flow = flow;
        _logger = logger;
    }


    public async Task<Result<AvailabilityRequest>> Get(string availabilityId)
    {
        var availabilityRequest = await _flow.GetAsync<AvailabilityRequest?>(BuildKey(availabilityId), RequestCacheLifeTime);

        if (availabilityRequest is not null)
            return availabilityRequest.Value;

        _logger.LogGetAvailabilityRequestFromStorageFailed(availabilityId);
        return Result.Failure<AvailabilityRequest>($"Could not get availability with id {availabilityId}");
    }


    public Task Set(string availabilityId, AvailabilityRequest request)
        => _flow.SetAsync(BuildKey(availabilityId), request, RequestCacheLifeTime);


    private string BuildKey(string availabilityId)
        => _flow.BuildKey(nameof(AvailabilityRequestStorage), availabilityId);


    private static TimeSpan RequestCacheLifeTime => Constants.StepCacheLifeTime * 3;

    private readonly IDoubleFlow _flow;
    private readonly ILogger<AvailabilityRequestStorage> _logger;
}
