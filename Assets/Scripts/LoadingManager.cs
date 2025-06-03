using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Clase09;

public class LoadingManager : MonoBehaviourSingleton<LoadingManager>
{
    public static LoadingManager instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingSlider;
    [SerializeField] private float maxTime = 3f;

    protected override void OnAwaken()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }

    public void StartLoading()
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(LoadGameAsynchronously());
    }

    private IEnumerator LoadGameAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(GameManager.Instance.sceneToLoad);
        operation.allowSceneActivation = false;

        float onTime = 0f;
        float percentage = 0.9f;

        while (onTime < maxTime * percentage)
        {
            onTime += Time.deltaTime * 10f;
            if (loadingSlider != null)
                loadingSlider.fillAmount = Mathf.SmoothStep(loadingSlider.fillAmount, onTime / maxTime, 0.2f);

            yield return null;
        }

        while (operation.progress < 0.89f)
        {
            yield return null;
        }

        while (onTime < maxTime)
        {
            onTime += Time.deltaTime * 10f;
            if (loadingSlider != null)
                loadingSlider.fillAmount = Mathf.SmoothStep(loadingSlider.fillAmount, onTime / maxTime, 0.2f);

            yield return null;
        }

        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        operation.allowSceneActivation = true;
    }
}
