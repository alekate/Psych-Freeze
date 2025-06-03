using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Clase09
{
    public class CustomSceneManager : MonoBehaviourSingleton<CustomSceneManager>
    {
        public static event Action onLoadedScene;

        private IEnumerator loadingScene;
        [SerializeField] private Image image;
        [SerializeField] private float maxTime = 10;
        [SerializeField] private AnimationCurve animationCurve;

        protected override void OnAwaken ()
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }

        public void ChangeSceneTo (string sceneName)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (loadingScene == null)
            {
                loadingScene = LoadingScene(sceneName);
                StartCoroutine(loadingScene);
            }
        }

        private IEnumerator LoadingScene (string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); //aditiva para que no destruya a la otra cuando carga
            operation.allowSceneActivation = false;
            operation.completed += Operation_completed;
            float onTime = 0;

            float percentage = 0.9f;

            while (onTime < maxTime * percentage) // Fake
            {
                onTime += Time.deltaTime;

                image.fillAmount = animationCurve.Evaluate(onTime / maxTime);
                yield return null;
            }

            while (operation.progress < 0.89) // Real
            {
                yield return null;
            }

            while (onTime < maxTime) // Fake que espera al real
            {
                onTime += Time.deltaTime * 10;

                image.fillAmount = animationCurve.Evaluate(onTime / maxTime);
                yield return null;
            }

            gameObject.SetActive(false);
            operation.allowSceneActivation = true;
            yield return null;
            operation.completed -= Operation_completed;
            loadingScene = null;
        }

        private void Operation_completed (AsyncOperation obj)
        {
            onLoadedScene?.Invoke();
        }
    }
}