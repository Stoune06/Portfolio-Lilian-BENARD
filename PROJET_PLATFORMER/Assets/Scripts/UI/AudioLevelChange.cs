using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;
using Tooling;

public class AudioLevelChange : MonoBehaviour
{
    [SerializeField] private int _StartAudio;
    [SerializeField] private int _SelectedGroup;
    [SerializeField] private Slider _Slider;
    [SerializeField] private Image _Handle;
   
    private const string PREFIX = "vca:/", MASTER = "Master", MUSIC = "MUS", SFX = "SFX", VOICE = "Voices", AMB = "AMB";
    private string _Group;
    private VCA _VCA;
    private float _Volume;

    [SerializeField] private TweenProperty _Property;
    [SerializeField] Vector3 _StartScale = new Vector3(.8f, .8f, .8f);

    private void Start()
    {
        switch (_SelectedGroup)
        {
            case 0:
                _Group=MASTER; break;
            case 1:
                _Group=MUSIC; break;
            case 2:
                _Group=SFX; break;
            case 3:
                _Group=VOICE; break;
            case 4:
                _Group=AMB; break;
        }
        _VCA = RuntimeManager.GetVCA(PREFIX + _Group);
        //_VCA.setVolume(_StartAudio);
        _VCA.getVolume(out _Volume);
        _Slider.value = _Volume; //Or whatever equation may be needed to actually set it right.
        //Debug.Log(_VCA);
        //Debug.Log(_Volume);
    }

    public void OnAudioChange()
    {
        //Debug.Log("VolumeChange" + _Slider.value);
        if(_Slider.value < _Volume)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/Slider_Down");
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/Slider_UP");
        }
            _VCA.setVolume(_Slider.value);
        _VCA.getVolume(out _Volume);
        //Debug.Log("Changed " + _Volume);
        Bounce();
    }

private void Bounce()
    {
        _Handle.transform.localScale = _StartScale;
        _Handle.transform.ScaleTo(Vector3.one, _Property);
    }

}