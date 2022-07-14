using CSharpFunctionalExtensions;
using FloxDc.CacheFlow;
using FloxDc.CacheFlow.Extensions;
using HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Logging;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Caching;

public class PreBookResultStorage
{
    public PreBookResultStorage(IDoubleFlow flow, ILogger<PreBookResultStorage> logger)
    {
        _flow = flow;
        _logger = logger;
    }


    public async Task<Result<CachedPrebookResult>> Get(string availabilityId)
    {
        var preBookResult = await _flow.GetAsync<CachedPrebookResult?>(BuildKey(availabilityId), RequestCacheLifeTime);

        if (preBookResult is not null)
            return preBookResult;

        _logger.LogGetPreBookResultFromStorageFailed(availabilityId);
        return Result.Failure<CachedPrebookResult>($"Could not get PreBookResult with id {availabilityId}");
    }


    public Task Set(string availabilityId, CachedPrebookResult data)
        => _flow.SetAsync(BuildKey(availabilityId), data, RequestCacheLifeTime);


    private string BuildKey(string availabilityId)
        => _flow.BuildKey(nameof(PreBookResultStorage), availabilityId);


    private static TimeSpan RequestCacheLifeTime => Constants.StepCacheLifeTime * 3;

    private readonly IDoubleFlow _flow;
    private readonly ILogger<PreBookResultStorage> _logger;
}
