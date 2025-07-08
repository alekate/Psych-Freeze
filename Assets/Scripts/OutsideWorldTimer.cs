using UnityEngine;

public class OutsideWorldTimer : MonoBehaviour
{
    [SerializeField] private float startTimeInSeconds = 300f;
    private float currentTime;

    public bool TimerActive { get; private set; } = false;
    public float TimeRemaining => currentTime;

    [SerializeField] private UIController UIController;

    private bool gameEnded = false;

    private void Update()
    {
        if (gameEnded && Input.GetKeyDown(KeyCode.Space))
        {
            UIController.menuController.LoadMainMenu();
        }

        if (!TimerActive) return;

        currentTime -= Time.deltaTime;

        UIController?.UpdateTimerUI(currentTime);

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            TimerEnded();
        }
    }

    public void StartTimer()
    {
        currentTime = startTimeInSeconds;
        TimerActive = true;
        gameEnded = false;
    }

    public void ResetTimer()
    {
        TimerActive = false;
        currentTime = startTimeInSeconds;
        gameEnded = false;
    }

    private void TimerEnded()
    {
        TimerActive = false;
        gameEnded = true;
        UIController.LoseGameUI();
    }
}
