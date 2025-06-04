using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Clase09
{
    public class CustomSceneManager : MonoBehaviourSingleton<CustomSceneManager>
    {
        ////////// EVENTO QUE SE LANZA CUANDO SE CARGA UNA ESCENA //////////

        public static event Action onLoadedScene;

        private IEnumerator loadingScene;
        [SerializeField] private GameObject loadingPanel;              // Pantalla de carga
        [SerializeField] private Image image;              // Imagen de carga (relleno)
        [SerializeField] private float maxTime = 10;       // Tiempo máximo de carga falsa
        [SerializeField] private AnimationCurve animationCurve; // Curva de animación para suavizar la barra

        ////////// DESACTIVA ESTE OBJETO SI YA ESTÁ ACTIVO AL INICIAR //////////

        protected override void OnAwaken()
        {
            loadingPanel.SetActive(false);
        }

        ////////// CAMBIAR A OTRA ESCENA //////////

        public void ChangeSceneTo(string sceneName)
        {
            if (loadingScene == null)
            {
                loadingScene = LoadingScene(sceneName);
                StartCoroutine(loadingScene);
            }
        }

        ////////// COROUTINE PARA CARGAR UNA ESCENA CON BARRA DE CARGA FALSA Y REAL //////////

        private IEnumerator LoadingScene(string sceneName)
        {
            loadingPanel.SetActive(true);


            // Carga aditiva -> no destruye lo anterior
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;

            // Cuando termine, llama a Operation_completed
            operation.completed += Operation_completed;

            float onTime = 0;
            float percentage = 0.9f;

            // Falsa barra de carga
            while (onTime < maxTime * percentage)
            {
                onTime += Time.deltaTime;
                image.fillAmount = animationCurve.Evaluate(onTime / maxTime);
                yield return null;
            }

            // Esperamos a que Unity realmente cargue (hasta 89%)
            while (operation.progress < 0.89f)
            {
                yield return null;
            }

            // Falsa espera final para completar la barra
            while (onTime < maxTime)
            {
                onTime += Time.deltaTime * 10;
                image.fillAmount = animationCurve.Evaluate(onTime / maxTime);
                yield return null;
            }

            operation.allowSceneActivation = true; // Termina la carga
            loadingPanel.SetActive(false);         // Oculta el panel de loading
            yield return null;

            operation.completed -= Operation_completed;
            loadingScene = null;
        }

        ////////// CUANDO UNITY TERMINA DE CARGAR LA ESCENA //////////

        private void Operation_completed(AsyncOperation obj)
        {
            onLoadedScene?.Invoke(); // Dispara evento a los que estén escuchando
        }
    }
}
