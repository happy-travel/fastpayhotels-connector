namespace HappyTravel.FastpayhotelsConnector.Updater.Workers;

public interface IUpdateWorker
{
    Task Run(CancellationToken cancellationToken);
}
