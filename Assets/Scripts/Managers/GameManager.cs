using UnityEngine;
using UnityEngine.SceneManagement;

namespace Clase09
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        ////////// VARIABLES //////////

        private GameObject player; // Referencia al jugador

        private SceneReferences main;               // Referencia a la escena principal
        private SceneReferences current;            // Referencia a la escena actual cargada (cuando hay más de una)
        private string lastSceneLoaded;

        private PickupCounter pickupCounter;

        ////////// SE INICIALIZA CUANDO SE CREA EL SINGLETON //////////

        protected override void OnAwaken()
        {
            // Se suscribe al evento cuando se carga una nueva escena con SceneReferences
            SceneReferences.onLoadedScene += SceneReferences_onLoadedScene;
            player = player = GameObject.FindWithTag("Player");
            pickupCounter = player.GetComponent<PickupCounter>();
        }

        ////////// SE LLAMA AL DESTRUIR EL OBJETO //////////

        protected override void OnDestroy()
        {
            // Se desuscribe del evento para evitar errores cuando se destruya
            SceneReferences.onLoadedScene -= SceneReferences_onLoadedScene;
        }

        ////////// EVENTO CUANDO SE CARGA UNA ESCENA CON SCENEREFERENCES //////////

        private void SceneReferences_onLoadedScene(SceneReferences obj)
        {
            if (main == null)
                main = obj; // Si aún no tenemos una escena principal, esta será la primera
            else
            {
                current = obj; // Si ya había una principal, esta nueva es la escena secundaria

                // Teletransportamos al jugador a la posición y rotación definidas por current.previousState
                player.transform.position = current.previousState.position;
                player.transform.rotation = current.previousState.rotation;
            }
        }

        ////////// CARGA OTRA ESCENA Y GUARDA LA POSICIÓN ACTUAL DEL JUGADOR //////////

        public void Load(string sceneToLoad)
        {
            lastSceneLoaded = sceneToLoad;

            main.previousState.position = player.transform.position;
            main.previousState.rotation = player.transform.rotation;

            player.SetActive(false);

            CustomSceneManager.onLoadedScene += CustomSceneManager_onLoadedScene;
            CustomSceneManager.Instance.ChangeSceneTo(sceneToLoad);
        }


        ////////// CUANDO TERMINÓ DE CARGAR LA ESCENA NUEVA //////////

        private void CustomSceneManager_onLoadedScene()
        {
            CustomSceneManager.onLoadedScene -= CustomSceneManager_onLoadedScene;

            SetAsActiveScene(lastSceneLoaded);

            // Buscar el spawn point en la nueva escena
            GameObject spawn = GameObject.FindWithTag("SpawnPoint");
            if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
                player.transform.rotation = spawn.transform.rotation;
            }
            else
            {
                Debug.LogWarning("No se encontró un objeto con el tag 'Spawn' en la nueva escena.");
            }

            pickupCounter.UpdatePickupCounter();
            player.SetActive(true);
        }



        ////////// DESCARGA LA ESCENA SECUNDARIA Y VUELVE A LA PRINCIPAL //////////

        public void Unload(string sceneToUnload)
        {
            player.transform.position = main.previousState.position;
            player.transform.rotation = main.previousState.rotation;

            SetAsActiveScene(main.gameObject.scene.name);
            main.SetActiveGo(true);

            SceneManager.UnloadSceneAsync(lastSceneLoaded);
        }

        ////////// CAMBIA LA ESCENA ACTIVA EN UNITY //////////

        private void SetAsActiveScene(string sceneName)
        {
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(newScene);
            main.SetActiveGo(false); // Ocultamos los objetos del mundo de la escena principal
        }
    }
}
