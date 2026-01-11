using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;
    public AudioClip bossMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ---------------------------
    // Música normal
    // ---------------------------
    public void PlayBackgroundMusic()
    {
        if (musicSource.clip == backgroundMusic) return;

        musicSource.Stop();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    // ---------------------------
    // Música de Boss
    // ---------------------------
    public void PlayBossMusic()
    {
        if (musicSource.clip == bossMusic) return;

        musicSource.Stop();
        musicSource.clip = bossMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
}
