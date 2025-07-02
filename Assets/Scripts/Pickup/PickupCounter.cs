using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupCounter : MonoBehaviour
{
    public int currentPickups;
    public int allPickupsInLevel;
    private int totalPickupsToFinishGame;

    [SerializeField] private UIController UIController;
    [SerializeField] private MenuController menuController;

    private static HashSet<string> levelsCounted = new(); // evita sumar dos veces un mismo nivel

    void Start()
    {
        currentPickups = 0;
        UpdatePickupCounter();
    }

    private void Update()
    {
        CheckIfAllPickupsCollected();
    }

    public void UpdatePickupCounter()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != "OutsideWorld" && !levelsCounted.Contains(currentScene))
        {
            GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
            allPickupsInLevel = pickups.Length;

            totalPickupsToFinishGame += allPickupsInLevel;
            levelsCounted.Add(currentScene);
        }
    }

    public void PickupCollected()
    {
        currentPickups++;
    }

    private void CheckIfAllPickupsCollected()
    {
        if (currentPickups >= totalPickupsToFinishGame && totalPickupsToFinishGame > 0)
        {
            UIController.ReturnToOutsideWorldUI();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                menuController.LoadMainMenu();
            }
        }
    }
}
