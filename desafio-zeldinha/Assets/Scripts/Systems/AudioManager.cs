using UnityEngine;

namespace Zeldinha.Systems
{
    /// <summary>
    /// Gerenciador simples de áudio.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;

        [Header("Clips")]
        public AudioClip backgroundMusic;
        public AudioClip attackSwing;
        public AudioClip hitEnemy;
        public AudioClip playerHurt;
        public AudioClip itemPickup;
        public AudioClip fragmentCollect;
        public AudioClip bossMusic;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (backgroundMusic != null && musicSource != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (sfxSource != null && clip != null)
                sfxSource.PlayOneShot(clip, volume);
        }

        public void PlayAttack() => PlaySFX(attackSwing);
        public void PlayHit() => PlaySFX(hitEnemy);
        public void PlayHurt() => PlaySFX(playerHurt);
        public void PlayPickup() => PlaySFX(itemPickup);
        public void PlayFragment() => PlaySFX(fragmentCollect);

        public void PlayBossMusic()
        {
            if (musicSource != null && bossMusic != null)
            {
                musicSource.clip = bossMusic;
                musicSource.Play();
            }
        }
    }
}
