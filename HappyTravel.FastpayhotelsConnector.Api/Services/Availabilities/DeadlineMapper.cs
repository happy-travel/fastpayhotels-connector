using HappyTravel.EdoContracts.Accommodations;
using HappyTravel.EdoContracts.Accommodations.Internals;
using HappyTravel.FastpayhotelsConnector.Api.Infrastructure.Extensions;
using HappyTravel.FastpayhotelsConnector.Api.Models.Availability.Api;

namespace HappyTravel.FastpayhotelsConnector.Api.Services.Availabilities;

public static class DeadlineMapper
{
    public static Deadline GetDeadline(ApiCancellationPolicy cancelPolicy, List<decimal> pricePerDay, decimal totalPrice, int numberOfNights, DateTimeOffset checkInDate,  TimeSpan hotelTimezone) 
    {
        var remarks = new List<string>() { cancelPolicy.Description };

        if (!cancelPolicy.Cancellable)
            return new Deadline(DateTimeOffset.UtcNow, new List<CancellationPolicy>() { new(DateTimeOffset.UtcNow, 100) }, remarks);

        var actualPolicies = new List<CancellationPolicy>();

        var code = cancelPolicy.Code; // Example: 100P_0D80P_1D70P_5D2N_AD0#9PM

        if (FreeCancellationCodes.Contains(code))
            return new Deadline(date: default, remarks: remarks);

        var deadlineHourStr = code.Split('#').LastOrDefault();
        if (deadlineHourStr is not null)
            code = code.RemoveAll($"#{deadlineHourStr}");
        else
            deadlineHourStr = Constants.DefaultDeadlineTime;
        var deadlineHour = DateTimeOffset.Parse(deadlineHourStr).Hour;
                
        foreach (var apiPolicy in code.Split('_'))
        {
            if (apiPolicy.Contains("T")) // Time Range
                actualPolicies.Add(GetPolicy(apiPolicy, checkNoShow: false).Value);
            else
                actualPolicies.Add(GetPolicy(apiPolicy, checkNoShow: true).Value);
        }

        actualPolicies = actualPolicies
            .Where(p => p.Percentage > 0)
            .ToList();

        var deadlineDate = actualPolicies
            .OrderBy(p => p.FromDate)
            .First().FromDate;

        return new Deadline(date: deadlineDate,
            policies: actualPolicies,
            remarks: new List<string>()
            {
                cancelPolicy.Description
            });


        CancellationPolicy? GetPolicy(string apiPolicy, bool checkNoShow)
        {
            var cancellationPolicy = GetUnlimitedAdvanceDayPolicy(apiPolicy, pricePerDay, totalPrice, numberOfNights)
                ?? GetAdvanceDayPolicy(apiPolicy, deadlineHour, pricePerDay, totalPrice, numberOfNights, checkInDate, hotelTimezone)
                ?? GetAdvanceHourPolicy(apiPolicy, deadlineHour, pricePerDay, totalPrice, numberOfNights, checkInDate, hotelTimezone);

            if (cancellationPolicy is null && checkNoShow)
                return GetNoShowPolicy(apiPolicy, pricePerDay, totalPrice, numberOfNights, checkInDate, hotelTimezone);

            return cancellationPolicy;
        }
    }


    private static CancellationPolicy? GetUnlimitedAdvanceDayPolicy(string apiPolicy, List<decimal> pricePerDay, decimal totalPrice, int numberOfNights)
    {
        if (apiPolicy.Contains("AD")) // AD means unlimited advance day
        {
            var value = apiPolicy.RemoveAll("AD");
            var percentage = GetPercentage(value, pricePerDay, totalPrice, numberOfNights);

            return new CancellationPolicy(DateTimeOffset.UtcNow, percentage);
        }

        return null;
    }


    private static CancellationPolicy? GetAdvanceDayPolicy(string apiPolicy, int deadlineHour, List<decimal> pricePerDay,
        decimal totalPrice, int numberOfNights, DateTimeOffset checkInDate, TimeSpan hotelTimezone)
    {
        if (apiPolicy.Contains('D')) // D means advance day
        {
            var values = apiPolicy.Split('D');

            var advanceDay = Convert.ToInt32(values[0].RemoveAll("D"));
            var fromDate = new DateTimeOffset(checkInDate.Date.AddHours(deadlineHour).AddDays(-advanceDay), hotelTimezone).ToUniversalTime();

            var percentage = GetPercentage(values[1], pricePerDay, totalPrice, numberOfNights);

            return new CancellationPolicy(fromDate, percentage);
        }

        return null;
    }


    private static CancellationPolicy? GetAdvanceHourPolicy(string apiPolicy, int deadlineHour, List<decimal> pricePerDay,
        decimal totalPrice, int numberOfNights, DateTimeOffset checkInDate, TimeSpan hotelTimezone)
    {
        if (apiPolicy.Contains('H')) // H means advance hour
        {
            var values = apiPolicy.Split('H');

            var advanceHour = Convert.ToInt32(values[0].RemoveAll("H"));
            var fromDate = new DateTimeOffset(checkInDate.Date.AddHours(deadlineHour).AddHours(-advanceHour), hotelTimezone).ToUniversalTime();

            var percentage = GetPercentage(values[1], pricePerDay, totalPrice, numberOfNights);

            return new CancellationPolicy(fromDate, percentage);
        }

        return null;
    }


    private static CancellationPolicy GetNoShowPolicy(string apiPolicy, List<decimal> pricePerDay, decimal totalPrice, int numberOfNights, DateTimeOffset checkInDate, TimeSpan hotelTimezone)
    {
        var percentage = GetPercentage(apiPolicy, pricePerDay, totalPrice, numberOfNights);
        var fromDate = new DateTimeOffset(checkInDate.AddDays(1).Date, hotelTimezone).ToUniversalTime(); // The customer doesn’t cancel the booking or cancel it after arrival day

        return new CancellationPolicy(fromDate, percentage);
    }


    private static double GetPercentage(string value, List<decimal> pricePerDay, decimal totalPrice, int numberOfNights)
    {
        var price = Convert.ToDouble(totalPrice);

        //if (value.Contains("MEN")) // Not used by suuplier yet
        //{
        //    var percentage = GetPercentageValue(value.RemoveAll("MEN"));

        //    // need to check
        //    var meNight = Convert.ToDouble(pricePerDay.Max());

        //    return Math.Round(meNight * percentage / price, 2);
        //}

        if (value.Contains("AVG"))
        {
            var percentage = GetPercentageValue(value.RemoveAll("AVG"));

            return Math.Round(percentage / numberOfNights, 2);
        }

        if (value.Contains('P'))
            return GetPercentageValue(value);

        if (value.Contains('N'))
        {
            var nights = Convert.ToDouble(value.RemoveAll("N"));
            
            return Math.Round(100 * nights / numberOfNights);
        }

        var amount = Convert.ToDouble(value);
            return Math.Round(100 * amount / price, 2);

        double GetPercentageValue(string value)
            => Convert.ToDouble(value.RemoveAll("P"));
    }


    private static List<string> FreeCancellationCodes = new List<string>()
    {
        "0",
        "0P",
        "AD0_0",
        "AD0P_0P"
    };
}