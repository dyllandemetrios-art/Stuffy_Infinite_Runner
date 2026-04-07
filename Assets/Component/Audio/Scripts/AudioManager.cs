using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton instance accessible from all scripts.

    // ===========================
    // MUSIC TRACKS
    // ===========================

    [Header("Music Tracks")]
    [SerializeField] private AudioClip _menuMusic;     // Played on the main menu scene.
    [SerializeField] private AudioClip _gameplayMusic; // Played during an active run.
    [SerializeField] private AudioClip _gameOverMusic; // Played on the game over screen.

    // ===========================
    // SOUND EFFECTS
    // ===========================

    [Header("SFX - Player")]
    [SerializeField] private AudioClip _jumpSound;       // Played when the player jumps.
    [SerializeField] private AudioClip _damageSound;     // Played when the player takes damage.
    [SerializeField] private AudioClip _deathSound;      // Played when the player dies.

    [Header("SFX - Collectibles")]
    [SerializeField] private AudioClip _wastePickupSound;     // Played when collecting a waste pickup.
    [SerializeField] private AudioClip _componentPickupSound; // Played when collecting an electronic component.

    [Header("SFX - Enemy")]
    [SerializeField] private AudioClip _projectileSound; // Played when the enemy throws a projectile.

    [Header("SFX - UI")]
    [SerializeField] private AudioClip _clickSound;      // Played on UI button clicks.
    [SerializeField] private AudioClip _purchaseSound;   // Played when buying a skill in the shop.

    // ===========================
    // VOLUME SETTINGS
    // ===========================

    [Header("Settings")]
    [SerializeField] private float _masterVolume = 0.5f; // Global volume multiplier (0-1).
    [SerializeField] private float _musicVolume  = 0.5f; // Background music volume (0-1).
    [SerializeField] private float _sfxVolume    = 0.5f; // Sound effects volume (0-1).

    // ===========================
    // AUDIO SOURCES
    // ===========================

    private AudioSource _musicSource; // Dedicated AudioSource for looping background music.
    private AudioSource _sfxSource;   // Dedicated AudioSource for one-shot sound effects.

    // ===========================
    // INITIALISATION
    // ===========================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
        _musicSource.volume = _masterVolume * _musicVolume;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;

        // Play menu music immediately after setup
        PlayMusic(MusicTrack.Menu);
    }

    /// <summary>Subscribes to game events to trigger sounds automatically.</summary>
    private void Start()
    {
        EventSystem.OnStateChanged        += HandleStateChanged;
        EventSystem.OnPlayerHealed        += _ => PlaySFX(SoundEffect.WastePickup);
        EventSystem.OnComponentCollected  += _ => PlaySFX(SoundEffect.ComponentPickup);
        EventSystem.OnPlayerHit           += () => PlaySFX(SoundEffect.Damage);
        EventSystem.OnPlayerLifeUpdated   += HandlePlayerLifeUpdated;
    }

    /// <summary>Unsubscribes from all events to prevent memory leaks.</summary>
    private void OnDestroy()
    {
        EventSystem.OnStateChanged       -= HandleStateChanged;
        EventSystem.OnPlayerLifeUpdated  -= HandlePlayerLifeUpdated;
    }

    // ===========================
    // EVENT HANDLERS
    // ===========================

    /// <summary>Switches music based on the current game state.</summary>
    private void HandleStateChanged(State newState)
    {
        if (newState is GameState)
            PlayMusic(MusicTrack.Gameplay);
        else if (newState is GameOverState)
            PlayMusic(MusicTrack.GameOver);
    }

    /// <summary>Plays death sound when player life reaches zero.</summary>
    private void HandlePlayerLifeUpdated(int playerLife)
    {
        if (playerLife <= 0)
            PlaySFX(SoundEffect.Death);
    }

    // ===========================
    // MUSIC CONTROL
    // ===========================

    /// <summary>Plays a background music track, ignoring the call if the same track is already playing.</summary>
    public void PlayMusic(MusicTrack track)
    {
        AudioClip clip = track switch
        {
            MusicTrack.Menu     => _menuMusic,
            MusicTrack.Gameplay => _gameplayMusic,
            MusicTrack.GameOver => _gameOverMusic,
            _ => null
        };

        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] Music clip not assigned : " + track);
            return;
        }

        if (_musicSource.clip == clip && _musicSource.isPlaying)
            return;

        _musicSource.clip = clip;
        _musicSource.Play();
    }

    /// <summary>Stops background music playback.</summary>
    public void StopMusic() => _musicSource.Stop();

    /// <summary>Pauses background music, preserving playback position.</summary>
    public void PauseMusic() => _musicSource.Pause();

    /// <summary>Resumes background music from its paused position.</summary>
    public void ResumeMusic() => _musicSource.UnPause();

    // ===========================
    // SFX CONTROL
    // ===========================

    /// <summary>Plays a one-shot sound effect, allowing multiple simultaneous sounds.</summary>
    public void PlaySFX(SoundEffect sfx)
    {
        AudioClip clip = sfx switch
        {
            SoundEffect.Jump            => _jumpSound,
            SoundEffect.Damage          => _damageSound,
            SoundEffect.Death           => _deathSound,
            SoundEffect.WastePickup     => _wastePickupSound,
            SoundEffect.ComponentPickup => _componentPickupSound,
            SoundEffect.Projectile      => _projectileSound,
            SoundEffect.Click           => _clickSound,
            SoundEffect.Purchase        => _purchaseSound,
            _ => null
        };

        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] SFX clip not assigned : " + sfx);
            return;
        }

        _sfxSource.PlayOneShot(clip, _sfxVolume);
    }

    // ===========================
    // VOLUME CONTROL
    // ===========================

    /// <summary>Sets the global master volume affecting all audio output.</summary>
    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        _musicSource.volume = _masterVolume * _musicVolume;
    }

    /// <summary>Sets the background music volume independently from SFX.</summary>
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        _musicSource.volume = _masterVolume * _musicVolume;
    }

    /// <summary>Sets the sound effects volume independently from music.</summary>
    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
    }

    // ===========================
    // ENUMS
    // ===========================

    /// <summary>Available background music tracks for each game scene.</summary>
    public enum MusicTrack
    {
        Menu,
        Gameplay,
        GameOver
    }

    /// <summary>Available sound effects mapped to in-game events.</summary>
    public enum SoundEffect
    {
        Jump,
        Damage,
        Death,
        WastePickup,
        ComponentPickup,
        Projectile,
        Click,
        Purchase
    }
}