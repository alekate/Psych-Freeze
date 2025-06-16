using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("Player")]


    [Header("Pickups")]
    [SerializeField] private PickupCounter pickupCounterScript;
    [SerializeField] private TextMeshProUGUI currentPickupText;
    [SerializeField] private TextMeshProUGUI allPickupsText;
    [SerializeField] private TextMeshProUGUI youWinText;
    [SerializeField] private TextMeshProUGUI youLoseText;

    private void Start()
    {
        youLoseText.gameObject.SetActive(false);
        youWinText.gameObject.SetActive(false);

    }
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "OutsideWorld")
        {
            currentPickupText.gameObject.SetActive(true);
            allPickupsText.gameObject.SetActive(true);
        }
        else
        {
            currentPickupText.gameObject.SetActive(false);
            allPickupsText.gameObject.SetActive(false);
        }
        UpdatePickupUI();
    }

    public void UpdatePickupUI()
    {
        currentPickupText.text = pickupCounterScript.currentPickups.ToString();
        allPickupsText.text = pickupCounterScript.allPickups.ToString();
    }

    public void FinnishGameUI()
    {
        youWinText.gameObject.SetActive(true);
    }

    public void LoseGameUI()
    {
        youLoseText.gameObject.SetActive(true);
    }

}
