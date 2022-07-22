using System;
using Microsoft.Extensions.Logging;

namespace HappyTravel.FastpayhotelsConnector.Updater.Infrastructure.Logging;

public static partial class LoggerExtensions
{
    [LoggerMessage(40001, LogLevel.Information, "Started worker '{workerName}'")]
    static partial void StartedWorker(ILogger logger, string workerName);
    
    [LoggerMessage(40002, LogLevel.Information, "Finished worker '{workerName}'")]
    static partial void FinishedWorker(ILogger logger, string workerName);
    
    [LoggerMessage(40003, LogLevel.Error, "Cancelling operation '{operationName}'")]
    static partial void CancellingOperation(ILogger logger, string operationName);
    
    [LoggerMessage(40004, LogLevel.Error, "Updater: ")]
    static partial void StaticDataUpdateHostedServiceException(ILogger logger, System.Exception exception);
    
    [LoggerMessage(40005, LogLevel.Information, "Stopping operation '{operationName}'")]
    static partial void StoppingOperation(ILogger logger, string operationName);
    
    [LoggerMessage(40006, LogLevel.Information, "Starting raw data hotels update")]
    static partial void StartingHotelsUpdate(ILogger logger);
    
    [LoggerMessage(40007, LogLevel.Error, "Hotelts loader: ")]
    static partial void HotelsLoaderException(ILogger logger, System.Exception exception);
    
    [LoggerMessage(40008, LogLevel.Information, "Finish raw data update")]
    static partial void FinishHotelsUpdate(ILogger logger);
    
    [LoggerMessage(40009, LogLevel.Error, "Hotel load exception")]
    static partial void HotelLoadException(ILogger logger, System.Exception exception);
    
    [LoggerMessage(40010, LogLevel.Error, "Hotel detail exception")]
    static partial void HotelDetailException(ILogger logger, System.Exception exception);
    
    [LoggerMessage(40011, LogLevel.Information, "Deactivating hotels complete")]
    static partial void DeactivateHotels(ILogger logger);
    
    
    
    public static void LogStartedWorker(this ILogger logger, string workerName)
        => StartedWorker(logger, workerName);
    
    public static void LogFinishedWorker(this ILogger logger, string workerName)
        => FinishedWorker(logger, workerName);
    
    public static void LogCancellingOperation(this ILogger logger, string operationName)
        => CancellingOperation(logger, operationName);
    
    public static void LogStaticDataUpdateHostedServiceException(this ILogger logger, System.Exception exception)
        => StaticDataUpdateHostedServiceException(logger, exception);
    
    public static void LogStoppingOperation(this ILogger logger, string operationName)
        => StoppingOperation(logger, operationName);
    
    public static void LogStartingHotelsUpdate(this ILogger logger)
        => StartingHotelsUpdate(logger);
    
    public static void LogHotelsLoaderException(this ILogger logger, System.Exception exception)
        => HotelsLoaderException(logger, exception);
    
    public static void LogFinishHotelsUpdate(this ILogger logger)
        => FinishHotelsUpdate(logger);
    
    public static void LogHotelLoadException(this ILogger logger, System.Exception exception)
        => HotelLoadException(logger, exception);
    
    public static void LogHotelDetailException(this ILogger logger, System.Exception exception)
        => HotelDetailException(logger, exception);
    
    public static void LogDeactivateHotels(this ILogger logger)
        => DeactivateHotels(logger);
}