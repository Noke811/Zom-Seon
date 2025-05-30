using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSFXSource;

    [Header("Audio Clips")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;
    public AudioClip[] loopSFXClips;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        SoundManager.Instance.PlayBGM(0);
    }
    public void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmClips.Length) return;
        if (bgmSource.clip == bgmClips[index]) return; // 중복 방지

        bgmSource.clip = bgmClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxClips.Length) return;
        sfxSource.PlayOneShot(sfxClips[index]);
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
    
    public void PlayLoopSFX(int index)
    {
        if (index < 0 || index >= loopSFXClips.Length) return;

        if (loopSFXSource.clip == loopSFXClips[index] && loopSFXSource.isPlaying) return;

        loopSFXSource.clip = loopSFXClips[index];
        loopSFXSource.loop = true;
        loopSFXSource.Play();
    }

    public void StopLoopSFX()
    {
        if (loopSFXSource.isPlaying)
            loopSFXSource.Stop();
    }
}
