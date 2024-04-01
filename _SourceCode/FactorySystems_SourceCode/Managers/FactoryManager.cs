using System;
using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : Singleton<FactoryManager>
{
    private List<Factory> registeredFactories;
    private float timePassed = 0f;

    private void Awake()
    {
        registeredFactories = new List<Factory>();
    }
    private void Update()
    {
        if (timePassed <= 1f)
        {
            timePassed += Time.deltaTime;
            return;
        }

        foreach (var factory in registeredFactories)
        {
            factory.ProduceGems();
        }
        timePassed = 0f;
    }

    public void QuitApp()
    {
        DateTimeHandler.GetCurrentDateTIme((TimeResponse d) =>
        {
            Debug.Log("QUITTING");
            DataHandler.LastLogoutTime = d.datetime;
            DataHandler.SaveFactoryLevels(registeredFactories.ToArray());

            if (Application.isEditor)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                Application.Quit();
            }
        }, (string error) =>
        {
            Debug.LogWarning($" Network error found: {error}. \n Quitting while resetting data!");
            DataHandler.ResetData();
            Application.Quit();
        });

    }

    public void StartGame()
    {
        DateTimeHandler.GetCurrentDateTIme((TimeResponse d) =>
        {
            DataHandler.CurrentLoginTime = d.datetime;

            // Init First Factory for slot 1 if its a new game
            if (!DataHandler.GameInitialized)
            {
                registeredFactories[0].SetLevel(1);
                DataHandler.GameInitialized = true;
            }
            else
            {
                // Retrieve old data and figure out what the collected gems value is in the time since last logged out
                SetupFactories();
                CalculateCollectedGemsCount();
            }
        }, null);
    }

    private void CalculateCollectedGemsCount()
    {
        DateTime currentLoginTime = DateTime.Parse(DataHandler.CurrentLoginTime);
        DateTime lastLogoutTime = DateTime.Parse(DataHandler.LastLogoutTime);
        TimeSpan timeElapsed = currentLoginTime - lastLogoutTime;
        int timeElapsedInSeconds = (int)timeElapsed.TotalSeconds;

        for (int i = 0; i < registeredFactories.Count; i++)
        {
            if (registeredFactories[i].GetFactoryConfig() != null)
            {
                DataHandler.GemsCollected += registeredFactories[i].GetFactoryConfig().RateOfGemProduction * timeElapsedInSeconds;
            }
        }
    }

    private void SetupFactories()
    {
        List<int> factoryLevels = DataHandler.GetFactoryLevels();

        for (int i = 0; i < Mathf.Min(factoryLevels.Count, registeredFactories.Count); i++)
        {
            registeredFactories[i].SetLevel(factoryLevels[i]);
        }
    }

    public void RegisterFactory(Factory factory)
    {
        registeredFactories.Add(factory);
    }
}