using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupCounter : MonoBehaviour
{
    public int currentPickups;
    public int allPickupsInLevel;
    private int totalPickupsToFinishGame;

    [SerializeField] private UIController UIController;
    [SerializeField] private MenuController menuController;

    public bool hasEnteredB1 = false;
    public bool hasEnteredB2 = false;

    private string currentScene;
    void Start()
    {
        currentPickups = 0;
        currentScene = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        CheckIfAllPickupsCollected();
    }

    public void UpdatePickupCounter()
    {

        if (currentScene != "OutsideWorld")
        {
            GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
            allPickupsInLevel = pickups.Length;

            totalPickupsToFinishGame += allPickupsInLevel;
        }
    }

    public void PickupCollected()
    {
        currentPickups++;
    }

    private void CheckIfAllPickupsCollected()
    {
        if (currentPickups >= totalPickupsToFinishGame && totalPickupsToFinishGame > 0 && hasEnteredB1 == true && hasEnteredB2 == true)
        {
            UIController.ReturnToOutsideWorldUI();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                menuController.LoadMainMenu();
            }
        }
    }
}
