using Clase09;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneChanger : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public string sceneToLoad;
    private string sceneToUnload;

    private bool hasChangedScene = false;

    private void Start()
    {
        FindObjectOfType<GameManager>();
        sceneToUnload = SceneManager.GetActiveScene().name;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasChangedScene) return; // Previene múltiples cargas

        if (collision.gameObject.CompareTag("Player"))
        {
            hasChangedScene = true;

            gameManager.Load(sceneToLoad);
            gameManager.Unload(sceneToUnload);
        }
    }
}
