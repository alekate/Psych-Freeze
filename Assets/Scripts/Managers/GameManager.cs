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

        public string LastDoorEntered;

        private PickupCounter pickupCounter;

        ////////// SE INICIALIZA CUANDO SE CREA EL SINGLETON //////////

        protected override void OnAwaken()
        {
            // Se suscribe al evento cuando se carga una nueva escena con SceneReferences
            SceneReferences.onLoadedScene += SceneReferences_onLoadedScene;
            player = GameObject.FindWithTag("Player");
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

            Scene existingScene = SceneManager.GetSceneByName(sceneToLoad);
            if (existingScene.isLoaded)
            {
                SetAsActiveScene(sceneToLoad);
                ActivateSceneObjects(existingScene);
                SpawnPlayerAtCorrectLocation();
                player.SetActive(true);
                pickupCounter.UpdatePickupCounter();
            }
            else
            {
                player.SetActive(false);

                CustomSceneManager.onLoadedScene += CustomSceneManager_onLoadedScene;
                CustomSceneManager.Instance.ChangeSceneTo(sceneToLoad);
            }
        }

        private void ActivateSceneObjects(Scene scene)
        {
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                go.SetActive(true);
            }
        }


        ////////// CUANDO TERMINÓ DE CARGAR LA ESCENA NUEVA //////////

        private void CustomSceneManager_onLoadedScene()
        {
            CustomSceneManager.onLoadedScene -= CustomSceneManager_onLoadedScene;

            SetAsActiveScene(lastSceneLoaded);
            SpawnPlayerAtCorrectLocation();
            pickupCounter.UpdatePickupCounter();
            player.SetActive(true);
        }

        private void SpawnPlayerAtCorrectLocation()
        {
            string activeScene = SceneManager.GetActiveScene().name;

            if (activeScene == "OutsideWorld")
            {
                GameObject door = GameObject.Find(LastDoorEntered);
                if (door != null)
                {
                    Vector3 offset = door.transform.forward * 2f;
                    player.transform.position = door.transform.position + offset;
                    player.transform.rotation = Quaternion.LookRotation(door.transform.forward);
                }
            }
            else
            {
                GameObject spawn = GameObject.FindWithTag("SpawnPoint");
                if (spawn != null)
                {
                    player.transform.position = spawn.transform.position;
                    player.transform.rotation = spawn.transform.rotation;
                }
            }
        }

        ////////// DESCARGA LA ESCENA SECUNDARIA Y VUELVE A LA PRINCIPAL //////////

        public void Unload(string sceneToUnload)
        {
            SetAsActiveScene(main.gameObject.scene.name);
            main.SetActiveGo(true);

            SpawnPlayerAtCorrectLocation();

            SceneManager.UnloadSceneAsync(sceneToUnload);
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
