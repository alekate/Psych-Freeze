using Clase09;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneChanger : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private string sceneToLoad;
    private string sceneToUnload;
    private bool hasChangedScene = false;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        sceneToUnload = SceneManager.GetActiveScene().name;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasChangedScene || !collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        hasChangedScene = true;

        if (sceneToUnload == "OutsideWorld")
        {
            gameManager.PreviousDoorEntered = name;
        }

        gameManager.LastDoorEntered = name;

        gameManager.Load(sceneToLoad);
        gameManager.Unload(sceneToUnload);
    }
}
