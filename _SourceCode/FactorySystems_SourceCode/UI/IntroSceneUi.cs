using System;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneUi : MonoBehaviour
{
    [SerializeField] private bool resetDataIfDifferentName = true;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button startButton;

    [SerializeField] private Button waringModal;
    private AsyncOperation asyncSceneLoading;

    private void Awake()
    {
        startButton.onClick.AddListener(StartPressed);
        waringModal.onClick.AddListener(CloseWarningModal);
        waringModal.gameObject.SetActive(false);
    }

    private void CloseWarningModal()
    {
        waringModal.gameObject.SetActive(false);
    }

    public void StartPressed()
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            Debug.LogWarning("PLEASE ENTER A NAME FOR SANITY!");
            return;
        }

        DateTimeHandler.GetCurrentDateTIme((TimeResponse d) =>
        {
            if (resetDataIfDifferentName && DataHandler.CurrentPlayerName != inputField.text)
            {
                DataHandler.ResetData();
            }

            DataHandler.CurrentPlayerName = inputField.text;
            DataHandler.CurrentLoginTime = d.datetime;
            asyncSceneLoading = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);


            asyncSceneLoading.completed += LoadComplete;
        },
        (string error) =>
        {
            waringModal.gameObject.SetActive(true);
        });
    }

    private void LoadComplete(AsyncOperation operation)
    {
        if (operation.isDone)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }
    }
}
