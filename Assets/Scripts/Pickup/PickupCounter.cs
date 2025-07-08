using Clase09;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupCounter : MonoBehaviour
{
    public int currentPickups;
    public int allPickupsInLevel;
    public int totalPickupsToFinishGame = 2;

    [SerializeField] private MenuController menuController;
    [SerializeField] private UIController UIController;


    [SerializeField] private GameManager gameManager;

    private string currentScene;

    void Start()
    {
        currentPickups = 0;
    }

    private void Update()
    {
        currentScene = SceneManager.GetActiveScene().name;

        CheckIfAllPickupsCollected();
        UpdatePickupCounter();
    }

    public void UpdatePickupCounter()
    {
        if (currentScene != "OutsideWorld")
        {
            GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
            allPickupsInLevel = pickups.Length;
        }
    }

    public void PickupCollected()
    {
        currentPickups++;
    }

    private void CheckIfAllPickupsCollected()
    {
        if (currentPickups >= totalPickupsToFinishGame && gameManager.hasEnteredB1 == true && gameManager.hasEnteredB2 == true)
        {
            UIController.FinnishGameUI();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                menuController.LoadMainMenu();
            }
        }
    }
}
