﻿namespace HappyTravel.FastpayhotelsConnector.Common;

public static class Constants
{   
    public const string HeaderGrandtype = "grant_type";
    public const string HeaderClientId = "client_id";
    public const string HeaderClientSecret = "client_secret";
    public const string HeaderVersion = "version";
    public const string HeaderUser = "username";
    public const string HeaderPassword = "password";
    public const string ApiVersion = "1";
    public const string FastpayhotelsTokenClient = "FastpayhotelsTokenClient";
    public static readonly TimeSpan AuthTokenLifeTime = TimeSpan.FromDays(400); // The access token has a validity of 400 days 
}
