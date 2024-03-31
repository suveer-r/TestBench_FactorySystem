using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Gems Counter")]
    [SerializeField] private TextMeshProUGUI gemsCountText;

    [Header("Factory Select UI")]
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI costToCreateOrUpgrade;
    [SerializeField] private GameObject factorySelectUi;
    [SerializeField] private Button[] closeUiButtons;
    [SerializeField] private Button createOrUpgradeButton;
    [SerializeField] private TextMeshProUGUI createOrUpgradeButtonText;

    private Factory selectedFactory;
    private bool selectFactoryUiOpen = false;

    private void Awake()
    {
        GameEvents.OnFactoryClicked += OnFactoryClicked;

        for (int i = 0; i < closeUiButtons.Length; i++)
        {
            closeUiButtons[i].onClick.AddListener(CloseFactorySelectUi);
        }
        factorySelectUi.SetActive(false);
        selectFactoryUiOpen = false;
        createOrUpgradeButton.onClick.AddListener(CreateOrUpgradeBtnPressed);
    }
    private void Update()
    {
        if (!selectFactoryUiOpen)
        {
            return;
        }

        if (selectedFactory.GetFactoryConfigForNextLevel().CostToBuildFactory > DataHandler.GemsCollected)
        {
            createOrUpgradeButton.interactable = false;
            createOrUpgradeButtonText.text = "Not Enough Gems!";
        }
        else
        {
            createOrUpgradeButton.interactable = true;
            createOrUpgradeButtonText.text = "Create Or Upgrade";
        }
    }

    private void LateUpdate()
    {
        gemsCountText.text = DataHandler.GemsCollected.ToString();
    }


    private void CreateOrUpgradeBtnPressed()
    {
        if (selectFactoryUiOpen)
        {
            selectedFactory.IncreaseLevel();
            CloseFactorySelectUi();
        }
    }

    private void CloseFactorySelectUi()
    {
        factorySelectUi.SetActive(false);
        selectFactoryUiOpen = false;
    }

    private void OnFactoryClicked(Factory factory)
    {
        if (!selectFactoryUiOpen && factory.CurrentLevel < factory.MaxLevel)
        {
            selectedFactory = factory;
            selectFactoryUiOpen = true;

            // Open the Factory Select UI
            factorySelectUi.SetActive(true);
            currentLevel.text = factory.CurrentLevel == 0 ? $"{factory.CurrentLevel.ToString()} - Create Factory" : factory.CurrentLevel.ToString();
            costToCreateOrUpgrade.text = factory.GetFactoryConfigForNextLevel().CostToBuildFactory.ToString();
        }
    }

}
