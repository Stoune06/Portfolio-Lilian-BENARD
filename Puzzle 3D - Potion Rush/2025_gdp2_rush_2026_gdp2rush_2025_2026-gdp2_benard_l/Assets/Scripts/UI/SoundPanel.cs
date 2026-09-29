using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioMixer _AudioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider _MasterSlider;
    [SerializeField] private Slider _MusicSlider;
    [SerializeField] private Slider _VfxSlider;

    [Header("Mixer Param")]
    private const string MASTER_PARAM = "masterVolume";
    private const string MUSIC_PARAM = "musicVolume";
    private const string VFX_PARAM = "sfxVolume";

    private void Start()
    {
        _MasterSlider.onValueChanged.AddListener(SetMasterVolume);
        _MusicSlider.onValueChanged.AddListener(SetMusicVolume);
        _VfxSlider.onValueChanged.AddListener(SetVFXVolume);

        SetMasterVolume(_MasterSlider.value);
        SetMusicVolume(_MusicSlider.value);
        SetVFXVolume(_VfxSlider.value);

    }

    public void SetMasterVolume(float pValue)
    {
        SetVolume(MASTER_PARAM, pValue);
    }

    public void SetMusicVolume(float pValue)
    {
        SetVolume(MUSIC_PARAM, pValue);
    }

    public void SetVFXVolume(float pValue)
    {
        SetVolume(VFX_PARAM, pValue);
    }

    private void SetVolume(string pParameterName, float pSliderValue)
    {
        if (pSliderValue <= 0) pSliderValue = 0.0001f;

        float dbValue = Mathf.Log10(pSliderValue) * 20;

        _AudioMixer.SetFloat(pParameterName, dbValue);
    }
}