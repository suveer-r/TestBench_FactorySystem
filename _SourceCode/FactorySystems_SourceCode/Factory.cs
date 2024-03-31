using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private GameObject[] gameObjectsPerLevel;
    [SerializeField] private FactoryConfig[] factoryConfigs;

    public int CurrentLevel { get => currentLevel; }
    public int MaxLevel {  get => factoryConfigs.Length; }

    private void Awake()
    {
        for (int i = 0; i < gameObjectsPerLevel.Length; i++)
        {
            gameObjectsPerLevel[i].SetActive(false);
        }
    }

    public void SetLevel(int level)
    {
        if (level > gameObjectsPerLevel.Length || currentLevel == level)
        {
            return;
        }

        currentLevel = level;
        for (int i = 0; i < gameObjectsPerLevel.Length; i++)
        {
            gameObjectsPerLevel[i].SetActive(i + 1 == level);
        }
    }

    public void IncreaseLevel()
    {
        if (CanBeCostructedOrUpgraded())
        {
            DataHandler.GemsCollected -= factoryConfigs[currentLevel].CostToBuildFactory;
            SetLevel(currentLevel + 1);
        }
    }

    public bool CanBeCostructedOrUpgraded()
    {
        return factoryConfigs.Length > currentLevel && DataHandler.GemsCollected >= factoryConfigs[currentLevel].CostToBuildFactory;
    }

    public FactoryConfig GetFactoryConfig()
    {
        if (currentLevel == 0)
        {
            return null;
        }

        return factoryConfigs[currentLevel - 1];
    }

    public FactoryConfig GetFactoryConfigForNextLevel()
    {
        if (currentLevel >= factoryConfigs.Length)
        {
            return factoryConfigs[factoryConfigs.Length - 1];
        }
        return factoryConfigs[currentLevel];
    }

    // Called from update every 1 second
    internal void ProduceGems()
    {
        if (currentLevel > 0)
        {
            DataHandler.GemsCollected += factoryConfigs[currentLevel - 1].RateOfGemProduction;
        }
    }
}
