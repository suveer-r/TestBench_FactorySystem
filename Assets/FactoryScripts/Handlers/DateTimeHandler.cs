using System;
using UnityEngine;
using UnityEngine.Networking;


[Serializable]
public class TimeResponse
{
    public string client_ip;
    public string datetime;
    public string timezone;
    public bool isDayLightSavingsTime;
    public string unixtime;
    public int day_of_week;
    public int day_of_year;
}

public static class DateTimeHandler
{
    private const string API_URL = "http://worldtimeapi.org/api/timezone/utc";

    public static void GetCurrentDateTIme(Action<TimeResponse> onTimeFetched, Action<string> onError)
    {
        UnityWebRequest request = UnityWebRequest.Get(API_URL);

        var asyncOperation = request.SendWebRequest();
        asyncOperation.completed += operation =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                TimeResponse timeResponse = JsonUtility.FromJson<TimeResponse>(jsonResponse);
                onTimeFetched?.Invoke(timeResponse);
            }
            else
            {
                onError?.Invoke("Failed to fetch time: " + request.error);
            }
        };
    }
}
