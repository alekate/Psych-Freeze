using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupCounter : MonoBehaviour
{
    public int currentPickups;               
    public int allPickups;            
    [SerializeField] private UIController UIController;
    [SerializeField] private MenuController menuController;


    void Start()
    {
        currentPickups = 0;
    }

    private void Update()
    {
        PickedUpAllPickupsInLevel();
    }

    public void UpdatePickupCounter()
    { 
        if(SceneManager.GetActiveScene().name != "OutsideWorld")
        {
            GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
            allPickups = pickups.Length;       
        }
    }

    public void PickedUpAllPickupsInLevel()
    {        
        if (currentPickups == allPickups)
        {
            UIController.ReturnToOutsideWorldUI();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                menuController.LoadMainMenu();
            }
        }
    }



}
