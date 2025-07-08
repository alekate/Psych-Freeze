using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Clase09
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        private GameObject player;
        public string lastSceneLoaded;

        public string LastDoorEntered { get; set; }
        public string PreviousDoorEntered { get; set; }

        [field: SerializeField] public bool hasEnteredB1 { get; private set; }
        [field: SerializeField] public bool hasEnteredB2 { get; private set; }

        [SerializeField] GameObject building1Beam;
        [SerializeField] GameObject building2Beam;

        [SerializeField] float spawnOffset;

        [SerializeField] private OutsideWorldTimer outsideWorldTimer;
        [SerializeField] private UIController UIController;


        protected override void OnAwaken()
        {
            if (FindObjectsOfType<GameManager>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            player = GameObject.FindWithTag("Player");

            lastSceneLoaded = SceneManager.GetActiveScene().name;

            CustomSceneManager.onLoadedScene += () => StartCoroutine(OnSceneLoaded());

            if (lastSceneLoaded == "OutsideWorld" && outsideWorldTimer != null)
            {
                outsideWorldTimer.gameObject.SetActive(true);
                outsideWorldTimer.StartTimer();
            }

        }


        protected override void OnDestroy()
        {
            CustomSceneManager.onLoadedScene -= () => StartCoroutine(OnSceneLoaded());
        }

        public void Load(string sceneToLoad)
        {
            lastSceneLoaded = sceneToLoad;

            var currentSceneRefs = FindSceneReferencesInScene(player.scene);
            if (currentSceneRefs != null)
            {
                currentSceneRefs.previousState.position = player.transform.position;
                currentSceneRefs.previousState.rotation = player.transform.rotation;
            }

            player.SetActive(false);
            CustomSceneManager.Instance.ChangeSceneTo(sceneToLoad);
        }

        public void Unload(string sceneToUnload)
        {
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }

        private IEnumerator OnSceneLoaded()
        {
            Scene newScene = SceneManager.GetSceneByName(lastSceneLoaded);
            SceneManager.SetActiveScene(newScene);

            // Desactivar escena anterior
            var previousRefs = FindSceneReferencesInScene(player.scene);
            previousRefs?.SetActiveGo(false);

            yield return null;

            var sceneRefs = FindSceneReferencesInScene(newScene);
            sceneRefs?.SetActiveGo(true);

            // Esperar a que el SpawnPoint esté disponible (si no es OutsideWorld)
            GameObject spawn = null;
            if (lastSceneLoaded != "OutsideWorld")
            {
                float timer = 0f;
                while (spawn == null && timer < 1f)
                {
                    spawn = GameObject.FindWithTag("SpawnPoint");
                    timer += Time.deltaTime;
                    yield return null;
                }
            }

            // Spawn del jugador
            if (lastSceneLoaded == "OutsideWorld")
            {
                GameObject door = GameObject.Find(PreviousDoorEntered);
                if (door != null)
                {
                    Vector3 offset = door.transform.up * spawnOffset;
                    player.transform.position = door.transform.position + offset;
                    player.transform.rotation = Quaternion.LookRotation(door.transform.forward);
                }
                else
                {
                    Debug.LogWarning("No se encontró la puerta " + PreviousDoorEntered);
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

            if (lastSceneLoaded == "OutsideWorld")
            {
                outsideWorldTimer?.gameObject.SetActive(true);
                outsideWorldTimer?.StartTimer();
            }
            else if (lastSceneLoaded == "Building1")
            {
                hasEnteredB1 = true;
                building1Beam.SetActive(false);
                building2Beam.SetActive(true);

                outsideWorldTimer?.ResetTimer();
                UIController?.ResetTimerUI();
            }
            else if (lastSceneLoaded == "Building2")
            {
                hasEnteredB2 = true;
                building1Beam.SetActive(false);
                building2Beam.SetActive(false);

                outsideWorldTimer?.ResetTimer();
                UIController?.ResetTimerUI();
            }
        }

        private SceneReferences FindSceneReferencesInScene(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var sr = root.GetComponentInChildren<SceneReferences>();
                if (sr != null) return sr;
            }

            return null;
        }
    }
}
