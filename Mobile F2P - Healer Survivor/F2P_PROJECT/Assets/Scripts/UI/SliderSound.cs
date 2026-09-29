using System;
using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Manager;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Other
{
    
    public class SliderSound : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Slider _SoundSlider;
        [SerializeField] private EVolumeType _VolumeType;

        private float _DB;
        private float _SliderValue;

        private SoundManager _SoundManager => SoundManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            GetVolumeValue();
            _SliderValue = _SoundManager.DBToSliderValue(_DB);
            _SoundSlider.SetValueWithoutNotify(_SliderValue);
            _SoundSlider.onValueChanged.AddListener(ChangeSound);
        }

        private void GetVolumeValue()
        {
            switch (_VolumeType)
            {
                case EVolumeType.MASTER:
                    _SoundManager.Mixer.GetFloat(Utils.MASTER_PARAM, out _DB);
                    break;
                case EVolumeType.MUSIC:
                    _SoundManager.Mixer.GetFloat(Utils.MUSIC_PARAM, out _DB);
                    break;
                case EVolumeType.SFX:
                    _SoundManager.Mixer.GetFloat(Utils.SOUND_PARAM, out _DB);
                    break;
                default:
                    _DB = 0;
                    break;
            }
        }

        private void ChangeSound(float pValue)
        {
            switch (_VolumeType)
            {
                case EVolumeType.MASTER:
                    _SoundManager.ChangeVolume(pValue, Utils.MASTER_PARAM);
                    break;
                case EVolumeType.MUSIC:
                    _SoundManager.ChangeVolume(pValue, Utils.MUSIC_PARAM);
                    break;
                case EVolumeType.SFX:
                    _SoundManager.ChangeVolume(pValue, Utils.SOUND_PARAM);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}