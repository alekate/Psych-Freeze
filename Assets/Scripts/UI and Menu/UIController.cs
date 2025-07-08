using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI youWinText;
    [SerializeField] private TextMeshProUGUI youLoseText;

    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI timer;


    public MenuController menuController;

    private void Start()
    {
        youLoseText.gameObject.SetActive(false);
        youWinText.gameObject.SetActive(false);
        timer.gameObject.SetActive(true);
    }

    public void FinnishGameUI()
    {
        youWinText.gameObject.SetActive(true);
        StartCoroutine(menuController.SlowDownTime());
    }

    public void LoseGameUI()
    {
        youLoseText.gameObject.SetActive(true);
        timer.gameObject.SetActive(false);
        StartCoroutine(menuController.SlowDownTime());
    }

    public void UpdateTimerUI(float timeRemaining)
    {
        if (timer == null) return;

        if (timeRemaining <= 0f)
        {
            timer.gameObject.SetActive(false);
            return;
        }


        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timer.text = $"Exporuse resistence: {minutes:00}:{seconds:00}";
        timer.gameObject.SetActive(true);
    }

    public void ResetTimerUI()
    {
        timer.gameObject.SetActive(false);
    }

}
