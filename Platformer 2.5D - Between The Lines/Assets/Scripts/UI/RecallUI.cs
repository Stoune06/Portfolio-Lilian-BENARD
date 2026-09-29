using FMOD.Studio;
using FMODUnity;
using Platformer.Player;
using Platformer.Utils;
using Player;
using System.Collections;
using Tooling;
using UnityEngine;
using UnityEngine.UI;

//Author : Noé SALES
namespace Platformer
{
    public class RecallUI : MonoBehaviour
    {
        [SerializeField] private float _ReloadTime = 0.5f;
        [SerializeField] private GlobalPlatformPlacer _DrawZone;
        [SerializeField] private Image _CircleTexture;

        private WaitForEndOfFrame _WaitForEndOfFrame = new WaitForEndOfFrame();
        private EventInstance instance;

        [Header("Tween")]
        [SerializeField] private TweenProperty _FadeOutProperty;
        [SerializeField] private TweenProperty _SpringOutProperty;
        [SerializeField] private Vector3 _ScaleSpring = new Vector3(1.1f, 1.1f, 1.1f);

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _CircleTexture.fillAmount = 0;
            instance = RuntimeManager.CreateInstance("event:/SFX/MC/Special Features/BrushPlat_Cancel");
            instance.start();
            // COMMENT ON CANCEL ??instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            //instance.release();
        }

        public void IncrementCircle(float pValue)
        {
            _CircleTexture.fillAmount = pValue;
            if (_CircleTexture.fillAmount >= 1) RecallAnimation();
        }

        private void RecallAnimation() //TODO CHECK GDP JUICYNESS
        {
            if(HUD.Instance._PaintChargeFirst.isOn && HUD.Instance._PaintChargeFirst.isOn)
            {
                _DrawZone.CanDraw = false;
                Vibration.Vibrate(Vibration.PredefinedEffect.CLICK);

                transform.ScaleTo(new Vector3(.7f, .7f, .7f), _SpringOutProperty).finished += () =>
                {
                    gameObject.SetActive(false);
                    _DrawZone.CanDraw = true;

                    _CircleTexture.fillAmount = 0f;
                    _CircleTexture.color = Color.white;
                    transform.localScale = Vector3.one;
                };

                Tween lTween = new Tween(_FadeOutProperty);
                lTween.Start(_CircleTexture, "color", new Color(1f, 0f, 0f, 0f));
            }
            else
            {
                _DrawZone.CanDraw = false;
                Vibration.Vibrate(Vibration.PredefinedEffect.TICK);

                transform.ScaleTo(_ScaleSpring, _SpringOutProperty).finished += () =>
                {
                    gameObject.SetActive(false);
                    _DrawZone.CanDraw = true;

                    _CircleTexture.fillAmount = 0f;
                    _CircleTexture.color = Color.white;
                    transform.localScale = Vector3.one;
                };

                Tween lTween = new Tween(_FadeOutProperty);
                lTween.Start(_CircleTexture, "color", new Color(1f, 1f, 1f, 0f));
            }
        }
    }
}