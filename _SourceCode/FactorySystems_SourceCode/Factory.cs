using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Factory : MonoBehaviour
{
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private GameObject[] gameObjectsPerLevel;
    [SerializeField] private FactoryConfig[] factoryConfigs;

    private Vector3 originalPosition;
    public int CurrentLevel { get => currentLevel; }
    public int MaxLevel {  get => factoryConfigs.Length; }

    private void Awake()
    {
        for (int i = 0; i < gameObjectsPerLevel.Length; i++)
        {
            gameObjectsPerLevel[i].SetActive(false);
        }
    }

    private void Start()
    {
        originalPosition = transform.position;
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
            StartCoroutine(ShakeObject());
            DataHandler.GemsCollected += factoryConfigs[currentLevel - 1].RateOfGemProduction;
        }
    }


    IEnumerator ShakeObject()
    {
        float shakeDuration = 0.5f;
        float shakeMagnitude = 0.1f;

        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPosition.x + Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = originalPosition.y + Random.Range(-shakeMagnitude, shakeMagnitude);
            float z = originalPosition.z + Random.Range(-shakeMagnitude, shakeMagnitude);

            transform.position = new Vector3(x, y, z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
    }
}
