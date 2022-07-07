namespace HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;

public static class StringExtensions
{
    public static string RemoveAll(this string value, string removingValue)
        => value.Replace(removingValue, string.Empty);
}
