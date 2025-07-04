using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Clase09
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        private GameObject player;
        private PickupCounter pickupCounter;

        public string LastDoorEntered;
        private string lastSceneLoaded;

        protected override void OnAwaken()
        {
            player = GameObject.FindWithTag("Player");
            pickupCounter = player.GetComponent<PickupCounter>();

            CustomSceneManager.onLoadedScene += () => StartCoroutine(OnSceneLoaded());
        }

        protected override void OnDestroy()
        {
            CustomSceneManager.onLoadedScene += () => StartCoroutine(OnSceneLoaded());
        }

        public void Load(string sceneToLoad)
        {
            lastSceneLoaded = sceneToLoad;

            SceneReferences currentSceneRefs = FindSceneReferencesInScene(player.scene);
            if (currentSceneRefs != null)
            {
                currentSceneRefs.previousState.position = player.transform.position;
                currentSceneRefs.previousState.rotation = player.transform.rotation;
            }

            player.SetActive(false);
            CustomSceneManager.Instance.ChangeSceneTo(sceneToLoad);
        }

        private IEnumerator OnSceneLoaded()
        {
            Scene newScene = SceneManager.GetSceneByName(lastSceneLoaded);
            SceneManager.SetActiveScene(newScene);

            Scene currentActive = player.scene;
            SceneReferences previousRefs = FindSceneReferencesInScene(currentActive);
            if (previousRefs != null)
                previousRefs.SetActiveGo(false);

            yield return null;

            GameObject spawn = null;
            float timer = 0f;
            while (spawn == null && timer < 1f)
            {
                spawn = GameObject.FindWithTag("SpawnPoint");
                timer += Time.deltaTime;
                yield return null;
            }

            SceneReferences sceneRefs = FindSceneReferencesInScene(newScene);
            if (sceneRefs != null)
                sceneRefs.SetActiveGo(true);

            if (lastSceneLoaded == "OutsideWorld")
            {
                GameObject door = GameObject.Find(LastDoorEntered);
                if (door != null)
                {
                    Vector3 offset = door.transform.forward * 2f;
                    player.transform.position = door.transform.position + offset;
                    player.transform.rotation = Quaternion.LookRotation(door.transform.forward);
                }
                else
                {
                    Debug.LogWarning("No se encontró la puerta " + LastDoorEntered);
                }
            }
            else if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
                player.transform.rotation = spawn.transform.rotation;
            }
            else
            {
                Debug.LogWarning("No se encontró SpawnPoint en escena " + lastSceneLoaded);
            }

            player.SetActive(true);
            pickupCounter.UpdatePickupCounter();

            // Flags por edificio
            if (lastSceneLoaded == "Building1")
                pickupCounter.hasEnteredB1 = true;
            if (lastSceneLoaded == "Building2")
                pickupCounter.hasEnteredB2 = true;
        }



        public void Unload(string sceneToUnload)
        {
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }

        private SceneReferences FindSceneReferencesInScene(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                SceneReferences sr = root.GetComponentInChildren<SceneReferences>();
                if (sr != null)
                    return sr;
            }

            return null;
        }
    }
}
