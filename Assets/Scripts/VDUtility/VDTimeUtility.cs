using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public static class VDTimeUtility
{
    private static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static DateTime onlineTimeNow;
    private static string TIME_FORMAT = "HH:mm:ss";
    private static string MONTH_FORMAT = "MM";
    private static string YEAR_FORMAT = "yyyy";
    private static string DAY_FORMAT = "dd";
    private static string DATE_FORMAT = string.Format("{0}-{1}-{2}", YEAR_FORMAT, MONTH_FORMAT, DAY_FORMAT);
    private static string DATE_TIME_FORMAT = string.Format("{0} {1}", DATE_FORMAT, TIME_FORMAT);
    public static string GetTimeStringFormat(double timer)
    {
        if (timer < 0) timer = 0;

        int months = (int)(timer / (30 * 24 * 60 * 60)); // Perkiraan 30 hari dalam sebulan
        timer %= (30 * 24 * 60 * 60);

        int weeks = (int)(timer / (7 * 24 * 60 * 60));
        timer %= (7 * 24 * 60 * 60);

        int days = (int)(timer / (24 * 60 * 60));
        timer %= (24 * 60 * 60);

        int hours = (int)(timer / (60 * 60));
        timer %= (60 * 60);

        int minutes = (int)(timer / 60);
        int seconds = (int)(timer % 60);

        List<string> timeParts = new List<string>();

        if (months > 0) timeParts.Add($"{months:D2}M");
        if (weeks > 0) timeParts.Add($"{weeks:D2}W");
        if (days > 0) timeParts.Add($"{days:D2}D");
        if (hours > 0) timeParts.Add($"{hours:D2}H");
        if (minutes > 0) timeParts.Add($"{minutes:D2}M");
        if (seconds > 0 || timeParts.Count == 0) timeParts.Add($"{seconds:D2}S");

        return string.Join(" ", timeParts);
    }

    public static double GetDateTimeInSec(DateTime dateTime)
    {
        double result = (dateTime.ToUniversalTime() - unixEpoch).TotalSeconds;
        return result;
    }

    public static DateTime GetDateTime(string dateTimeFormat)
    {
        DateTime result;
        DateTime.TryParseExact(dateTimeFormat, DATE_TIME_FORMAT, null, System.Globalization.DateTimeStyles.None, out result);
        return result;
    }

    public static string GetDateTimeFormat(DateTime dateTime)
    {
        string result = dateTime.ToString(DATE_TIME_FORMAT);
        return result;
    }

    public static string GetDateFormat(string dateTimeFormat)
    {
        string result = GetShortDateFormat(dateTimeFormat, DATE_FORMAT);
        return result;
    }

    public static string GetDayFormat(string dateTimeFormat)
    {
        string result = GetShortDateFormat(dateTimeFormat, DAY_FORMAT);
        return result;
    }

    public static string GetMonthFormat(string dateTimeFormat)
    {
        string result = GetShortDateFormat(dateTimeFormat, MONTH_FORMAT);
        return result;
    }

    public static string GetYearFormat(string dateTimeFormat)
    {
        string result = GetShortDateFormat(dateTimeFormat, YEAR_FORMAT);
        return result;
    }

    private static string GetShortDateFormat(string dateTimeFormat, string format)
    {
        string result = null;
        DateTime dateTime;

        if (DateTime.TryParse(dateTimeFormat, out dateTime))
        {
            result = dateTime.ToString(format);
        }

        return result;
    }

    public static DateTime GetOnlineTimeNow()
    {
        DateTime result = onlineTimeNow;
        return result;
    }

    public static IEnumerator FetchOnlineTimeNow()
    {
        DateTime requestTime = DateTime.UtcNow;
        string url = string.Format(VDParameter.TIME_ZONE_API, VDParameter.TIME_ZONE);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                TimeApiResponse response = JsonUtility.FromJson<TimeApiResponse>(request.downloadHandler.text);
                DateTime serverTime = DateTime.Parse(response.dateTime);
                TimeSpan roundTripTime = DateTime.UtcNow - requestTime;
                onlineTimeNow = serverTime.AddMilliseconds(-roundTripTime.TotalMilliseconds / 2);
            }
            else
            {
                Debug.LogError("Fetch Online Time Now Error: " + request.error);
                yield break;
            }
        }
    }
}
