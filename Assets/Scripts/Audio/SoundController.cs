using UnityEngine;

public class SoundController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource winSource;
    [SerializeField] private AudioSource loseSource;
    [SerializeField] private AudioSource jumpSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip pickupSound;


    private bool hasPlayedWin = false;
    private bool hasPlayedLose = false;

    // --- SFX ---
    public void PickupSFX()
    {
        audioSource.PlayOneShot(pickupSound);
    }

    public void JumpSFX()
    {
        if (!jumpSource.isPlaying)
            jumpSource.Play();
    }

    public void ButtonSFX()
    {
        audioSource.PlayOneShot(buttonSound);
    }

    public void LoseSFX()
    {
        if (hasPlayedLose || hasPlayedWin) return;

        hasPlayedLose = true;
        PlayExclusiveSource(loseSource);
    }

    public void WinSFX()
    {
        if (hasPlayedWin || hasPlayedLose) return;

        hasPlayedWin = true;
        PlayExclusiveSource(winSource);
    }

    private void PlayExclusiveSource(AudioSource source)
    {
        StopAllSoundsExcept(source);
        source.Play();
    }

    private void StopAllSoundsExcept(AudioSource exception)
    {
        AudioSource[] sources = {  winSource, loseSource, jumpSource };

        foreach (var src in sources)
        {
            if (src != null && src != exception)
            {
                src.Stop();
            }
        }
    }

    public void ResetWinLoseFlags()
    {
        hasPlayedWin = false;
        hasPlayedLose = false;
    }
}
