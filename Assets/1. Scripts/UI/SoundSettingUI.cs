using UnityEngine;
using UnityEngine.UI;

public class SoundSettingUI : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetAllSFXVolume);

        bgmSlider.value = SoundManager.Instance.GetBGMVolume();
        sfxSlider.value = SoundManager.Instance.GetAllSFXVolume();
    }
}