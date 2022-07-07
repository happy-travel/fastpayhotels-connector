namespace HappyTravel.FastpayhotelsConnector.Api;

public static class Constants
{
    public static readonly TimeSpan StepCacheLifeTime = TimeSpan.FromMinutes(10);

    public const int RoomContractSetsMaxCount = 100; // After consulting with the team, it was decided to set this number

    public const string DefaultDeadlineTime = "12AM";
}
