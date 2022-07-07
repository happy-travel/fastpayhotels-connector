using System;
using Microsoft.Extensions.Logging;

namespace HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Logging;

public static partial class LoggerExtensions
{
    [LoggerMessage(30000, LogLevel.Critical, "FastpayhotelsShoppingClient: {message}")]
    static partial void ApiResponseDeserializationException(ILogger logger, string message);
    
    [LoggerMessage(30010, LogLevel.Warning, "Get availability request by id `{AvailabilityId}` from storage failed")]
    static partial void GetAvailabilityRequestFromStorageFailed(ILogger logger, string AvailabilityId);
    
    [LoggerMessage(30011, LogLevel.Warning, "Get accommodation by availabilityId `{AvailabilityId}` from storage failed")]
    static partial void GetAccommodationFromStorageFailed(ILogger logger, string AvailabilityId);
    
    
    
    public static void LogApiResponseDeserializationException(this ILogger logger, string message)
        => ApiResponseDeserializationException(logger, message);
    
    public static void LogGetAvailabilityRequestFromStorageFailed(this ILogger logger, string AvailabilityId)
        => GetAvailabilityRequestFromStorageFailed(logger, AvailabilityId);
    
    public static void LogGetAccommodationFromStorageFailed(this ILogger logger, string AvailabilityId)
        => GetAccommodationFromStorageFailed(logger, AvailabilityId);
}