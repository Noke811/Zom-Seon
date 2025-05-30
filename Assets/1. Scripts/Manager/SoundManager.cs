using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;         // 배경음악 재생용 AudioSource
    [SerializeField] private AudioSource sfxSource;         // 일반 효과음 재생용 AudioSource
    [SerializeField] private AudioSource loopSFXSource;     // 루프 효과음(걷기 등) 재생용 AudioSource

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] bgmClips;          // BGM 클립 리스트
    [SerializeField] private AudioClip[] sfxClips;          // 효과음 클립 리스트
    [SerializeField] private AudioClip[] loopSFXClips;      // 루프 효과음 클립 리스트

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // 시작 BGM
    private void Start()
    {
        PlayBGM(0);
    }
    // BGM 재생
    public void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmClips.Length) return;
        if (bgmSource.clip == bgmClips[index]) return;

        bgmSource.clip = bgmClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // BGM 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    // 현재 BGM 볼륨
    public float GetBGMVolume() => bgmSource.volume;

    // 효과음 재생
    public void PlaySFX(int index)
    {
        if (index < 0 || index >= sfxClips.Length) return;
        sfxSource.PlayOneShot(sfxClips[index]);
    }

    // 루프 효과음 재생
    public void PlayLoopSFX(int index)
    {
        if (index < 0 || index >= loopSFXClips.Length) return;
        if (loopSFXSource.clip == loopSFXClips[index] && loopSFXSource.isPlaying) return;

        loopSFXSource.clip = loopSFXClips[index];
        loopSFXSource.loop = true;
        loopSFXSource.Play();
    }

    // 루프 효과음 정지
    public void StopLoopSFX()
    {
        if (loopSFXSource.isPlaying)
            loopSFXSource.Stop();
    }

    // 일반 효과음 + 루프 효과음 볼륨 모두 설정
    public void SetAllSFXVolume(float volume)
    {
        float v = Mathf.Clamp01(volume);
        sfxSource.volume = v;
        loopSFXSource.volume = v;
    }

    // 일반/루프 효과음 중 더 큰 볼륨 반환
    public float GetAllSFXVolume()
    {
        return Mathf.Max(sfxSource.volume, loopSFXSource.volume);
    }
}
