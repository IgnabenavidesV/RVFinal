using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource; // Este será el AudioSource que reproduce la música
    public AudioClip backgroundMusic;
    public AudioClip bossMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;   // Repetir infinitamente
        musicSource.Play();
    }

    public void PlayBossMusic()
    {
        musicSource.clip = bossMusic;
        musicSource.loop = true;   // Repetir mientras dure el boss
        musicSource.Play();
    }
}
