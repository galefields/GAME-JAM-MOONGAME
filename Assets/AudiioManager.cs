using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;    // Background music
    public AudioSource sfxSource;      // Sound effects

    [Header("Audio Clips")]
    public AudioClip PistolgunshotSound;
    public AudioClip PistolrelaodSound;
    public AudioClip ShotgunshotSound;
    public AudioClip ShotgunreloadSound;
    public AudioClip RailgunshotSound;
    public AudioClip RailgununreloadSound;
    public AudioClip enemyHitSound;
    public AudioClip barrierHitSound;
    public AudioClip buttonClickSound;
    public AudioClip Chingyvoiceacting;
    public AudioClip bgmusicbattle;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
}
