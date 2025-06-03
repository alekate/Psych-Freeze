using UnityEngine;
using UnityEngine.SceneManagement;

namespace Clase09
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private GameObject player;
        public string mainGameplayScene;
        public string sceneToLoad;

        private SceneReferences main;
        private SceneReferences current;
        protected override void OnAwaken ()
        {
            SceneReferences.onLoadedScene += SceneReferences_onLoadedScene;
        }

        protected override void OnDestroy()
        {
            SceneReferences.onLoadedScene -= SceneReferences_onLoadedScene;
        }

        private void SceneReferences_onLoadedScene (SceneReferences obj)
        {
            if (main == null)
                main = obj;
            else
            {
                current = obj;

                player.transform.position = current.previousState.position;
                player.transform.rotation = current.previousState.rotation;
            }
        }

        private void Update ()
        {
            if (Input.GetKeyDown(KeyCode.E))
                Load(sceneToLoad);

            if (Input.GetKeyDown(KeyCode.Q))
                Unload();
        }

        private void Load(string newSceneName)
        {
            main.previousState.position = player.transform.position;
            main.previousState.rotation = player.transform.rotation;

            CustomSceneManager.onLoadedScene += CustomSceneManager_onLoadedScene;
            CustomSceneManager.Instance.ChangeSceneTo(newSceneName);
        }

        private void CustomSceneManager_onLoadedScene()
        {
            CustomSceneManager.onLoadedScene -= CustomSceneManager_onLoadedScene;
            SetAsActiveScene(sceneToLoad);
        }

        private void Unload()
        {
            player.transform.position = main.previousState.position;
            player.transform.rotation = main.previousState.rotation;

            SetAsActiveScene(sceneToLoad);
            main.SetActiveGo(true);

            Scene gameplay = SceneManager.GetSceneByName(mainGameplayScene);
            SceneManager.SetActiveScene(gameplay);
            SceneManager.UnloadSceneAsync(sceneToLoad);
        }

        private void SetAsActiveScene (string sceneName)
        {
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(newScene);

            main.SetActiveGo(false);
        }
    }
}