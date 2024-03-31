using System;
using System.Collections.Generic;
using UnityEngine;

public static class DataHandler
{
    public static int GemsCollected
    {
        get
        {
            return PlayerPrefs.GetInt(CommonStrings.GemsCollected, 0);
        }
        set
        {
            PlayerPrefs.SetInt(CommonStrings.GemsCollected, value);
            PlayerPrefs.Save();
        }
    }

    public static bool GameInitialized
    {
        get
        {
            return PlayerPrefs.GetInt(CommonStrings.GameInitialized, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(CommonStrings.GameInitialized, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static string CurrentLoginTime
    {
        get
        {
            return PlayerPrefs.GetString(CommonStrings.CurrentLoginTime, "");
        }
        set
        {
            PlayerPrefs.SetString(CommonStrings.CurrentLoginTime, value);
            PlayerPrefs.Save();
        }
    }

    public static string LastLogoutTime
    {
        get
        {
            return PlayerPrefs.GetString(CommonStrings.LastLogoutTime, "");
        }
        set
        {
            PlayerPrefs.SetString(CommonStrings.LastLogoutTime, value);
            PlayerPrefs.Save();
        }
    }

    internal static void SaveFactoryLevels(Factory[] factories)
    {
        int[] levels = new int[factories.Length];

        for (int i = 0; i < factories.Length; i++)
        {
            levels[i] = factories[i].CurrentLevel;
        }

        string serializedLevels = string.Join(",", levels);

        PlayerPrefs.SetString(CommonStrings.FactoryLevels, serializedLevels);
        PlayerPrefs.Save();
    }

    internal static List<int> GetFactoryLevels()
    {
        List<int> factoriesLevels = new();

        string serializedLevels = PlayerPrefs.GetString(CommonStrings.FactoryLevels, "");

        if (!string.IsNullOrEmpty(serializedLevels))
        {
            // Split the serialized string to get individual levels
            string[] levelStrings = serializedLevels.Split(',');

            // Update factory levels based on retrieved data
            for (int i = 0; i < levelStrings.Length; i++)
            {
                int level;
                if (int.TryParse(levelStrings[i], out level))
                {
                    factoriesLevels.Add(level);
                }
            }
        }

        return factoriesLevels;
    }
}