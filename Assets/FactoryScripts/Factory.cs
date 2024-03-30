using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private GameObject[] gameObjectsPerLevel;
    [SerializeField] private FactoryConfig[] factoryConfigs;

    public int CurrentLevel { get => currentLevel; }

    private void Awake()
    {
        for (int i = 0; i < gameObjectsPerLevel.Length; i++)
        {
            gameObjectsPerLevel[i].SetActive(false);
        }
    }

    private void Start()
    {
        FactoryManager.Instance.RegisterFactory(this);
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
            gameObjectsPerLevel[i].SetActive(i + 1  == level);
        }
    }

    public void IncreaseLevel()
    {
        SetLevel(currentLevel + 1);
    }

    public FactoryConfig GetFactoryConfig()
    {
        return factoryConfigs[currentLevel - 1];
    }
}
