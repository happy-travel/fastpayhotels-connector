using HappyTravel.FastpayhotelsConnector.Common;
using HappyTravel.FastpayhotelsConnector.Data;
using HappyTravel.FastpayhotelsConnector.Data.Models;
using HappyTravel.FastpayhotelsConnector.Updater.Infrastructure;
using HappyTravel.FastpayhotelsConnector.Updater.Settings;
using Microsoft.EntityFrameworkCore;

namespace HappyTravel.FastpayhotelsConnector.Updater.Service;

public class UpdateHistoryService
{
    public UpdateHistoryService(FastpayhotelsContext context, DateTimeProvider dateTimeProvider, FastpayhotelsSerializer serializer)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _serializer = serializer;
    }


    public async Task<int> Create(RawDataUpdateOptions options)
    {
        var updateLogEntry = new StaticDataUpdateHistoryEntry
        {
            StartTime = _dateTimeProvider.UtcNow(),
            Options = _serializer.Serialize(options)
        };
        _context.StaticDataUpdateHistory.Add(updateLogEntry);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        return updateLogEntry.Id;
    }


    public async Task<DateTimeOffset?> GetLastSuccessfulUpdateTime()
    {
        var lastUpdateTime = await _context.StaticDataUpdateHistory
            .Where(h => h.IsSuccess)
            .OrderByDescending(l => l.StartTime)
            .Select(l => l.StartTime)
            .FirstOrDefaultAsync();

        return lastUpdateTime == default
            ? null
            : lastUpdateTime;
    }


    public async Task<StaticDataUpdateHistoryEntry> GetLastNonSuccessUpdate(int nonIncludeUpdateId)
    {
        var lastNonSuccess = await _context.StaticDataUpdateHistory
            .Where(x => !x.IsSuccess && x.Id != nonIncludeUpdateId &&
                _context.StaticDataUpdateHistory
                    .Where(y => y.IsSuccess)
                    .OrderByDescending(l => l.StartTime)
                    .Select(l => l.StartTime).FirstOrDefault() < x.StartTime
            )
            .OrderByDescending(x => x.StartTime)
            .FirstOrDefaultAsync();

        return lastNonSuccess;
    }    


    public async Task SetError(int updateId, Exception exception)
    {
        var updateLogEntry = await _context.StaticDataUpdateHistory.FindAsync(updateId);
        updateLogEntry.FinishTime = _dateTimeProvider.UtcNow();
        updateLogEntry.IsSuccess = false;
        updateLogEntry.Message = exception.Message;
        _context.Update(updateLogEntry);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }


    public async Task SetSuccess(int updateId)
    {
        var updateLogEntry = await _context.StaticDataUpdateHistory.FindAsync(updateId);
        updateLogEntry.FinishTime = _dateTimeProvider.UtcNow();
        updateLogEntry.IsSuccess = true;
        _context.Update(updateLogEntry);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
    }


    private readonly FastpayhotelsContext _context;
    private readonly DateTimeProvider _dateTimeProvider;
    private readonly FastpayhotelsSerializer _serializer;
}
