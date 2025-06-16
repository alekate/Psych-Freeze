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
        //PickedUpAllPickups();
    }

    public void UpdatePickupCounter()
    { 
        if(SceneManager.GetActiveScene().name != "OutsideWorld")
        {
            GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
            allPickups = pickups.Length;       
        }
    }

    /*public void PickedUpAllPickups()
    {        
        if (currentPickups == allPickups)
        {
            UIController.FinnishGameUI();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                menuController.LoadMainMenu();
            }
        }
    }*/

}
