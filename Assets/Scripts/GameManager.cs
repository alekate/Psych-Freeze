using UnityEngine;
using UnityEngine.SceneManagement;

namespace Clase09
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        ////////// VARIABLES //////////

        [SerializeField] private GameObject player; // Referencia al jugador
        public string mainGameplayScene;            // Nombre de la escena principal (bosque, por ejemplo)
        public string sceneToLoad;                  // Nombre de la escena secundaria a cargar (cueva, etc.)

        private SceneReferences main;               // Referencia a la escena principal
        private SceneReferences current;            // Referencia a la escena actual cargada (cuando hay más de una)

        ////////// SE INICIALIZA CUANDO SE CREA EL SINGLETON //////////

        protected override void OnAwaken()
        {
            // Se suscribe al evento cuando se carga una nueva escena con SceneReferences
            SceneReferences.onLoadedScene += SceneReferences_onLoadedScene;
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

        ////////// CHECK DE INPUTS PARA CAMBIAR ESCENAS //////////

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                Load(sceneToLoad); // Cargar nueva escena al presionar "E"

            if (Input.GetKeyDown(KeyCode.Q))
                Unload();          // Descargar la escena secundaria al presionar "Q"
        }

        ////////// CARGA OTRA ESCENA Y GUARDA LA POSICIÓN ACTUAL DEL JUGADOR //////////

        private void Load(string newSceneName)
        {
            // Guardamos la posición y rotación actuales del jugador
            main.previousState.position = player.transform.position;
            main.previousState.rotation = player.transform.rotation;

            player.SetActive(false);

            // Nos suscribimos al evento del SceneManager personalizado
            CustomSceneManager.onLoadedScene += CustomSceneManager_onLoadedScene;

            // Llamamos al método que inicia la carga
            CustomSceneManager.Instance.ChangeSceneTo(newSceneName);
        }

        ////////// CUANDO TERMINÓ DE CARGAR LA ESCENA NUEVA //////////

        private void CustomSceneManager_onLoadedScene()
        {
            // Nos desuscribimos del evento porque ya se ejecutó
            CustomSceneManager.onLoadedScene -= CustomSceneManager_onLoadedScene;

            player.SetActive(true);

            // Hacemos activa la nueva escena
            SetAsActiveScene(sceneToLoad);
        }

        ////////// DESCARGA LA ESCENA SECUNDARIA Y VUELVE A LA PRINCIPAL //////////

        private void Unload()
        {
            // Restauramos la posición previa del jugador en la escena principal
            player.transform.position = main.previousState.position;
            player.transform.rotation = main.previousState.rotation;

            // Activamos la escena principal
            SetAsActiveScene(sceneToLoad);
            main.SetActiveGo(true); // Volvemos a activar los objetos del mundo

            // Hacemos activa la escena del gameplay principal y descargamos la secundaria
            Scene gameplay = SceneManager.GetSceneByName(mainGameplayScene);
            SceneManager.SetActiveScene(gameplay);
            SceneManager.UnloadSceneAsync(sceneToLoad);
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
