using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioClip buttonClickClip;
    public AudioClip buttonHoverClip;
    public AudioClip gameStartClip;
    public AudioClip gameEndClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(bgmSource);
            DontDestroyOnLoad(sfxSource);
        }
    }

    public void PlayBGM(AudioClip bgm)
    {
        bgmSource.clip = bgm;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    public void StopBGM()
    {
        bgmSource.loop = false;
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip sfx)
    {
        sfxSource.PlayOneShot(sfx);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickClip);
    }

    public void PlayButtonHover()
    {
        PlaySFX(buttonHoverClip);
    }

    public void PlayGameStart()
    {
        PlaySFX(gameStartClip);
    }

    public void PlayGameEnd()
    {
        PlaySFX(gameEndClip);
    }
}
