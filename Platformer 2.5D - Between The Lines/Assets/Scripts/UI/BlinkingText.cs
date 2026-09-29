using Tooling;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer
{
    public class BlinkingText : MonoBehaviour
    {
        private Tween _Tween;
        [SerializeField] private TweenProperty _TweenProp;
        private Text _Text;
        [SerializeField] private Color _TargetCol;


        void Start()
        {
            _Tween = new Tween(_TweenProp);
            _Text = GetComponent<Text>();
            _Tween.Start(_Text, nameof(_Text.color), _TargetCol);
        }
        //Use tween in tooling
        /*private void Fade()
        {
            if (_Fading)
            {
                _Text.color = Color.Lerp(_Color, Color.clear, _Ratio);
            }
            else
            {
                _Text.color = Color.Lerp(Color.clear, _Color, _Ratio);
            }

            if (_Text.color == Color.clear) { _Fading = false; _BuiltTime = 0; }
            else if (_Text.color == _Color) { _Fading = true; _BuiltTime = 0; }

            _BuiltTime += Time.deltaTime;
            _Ratio = _BuiltTime / _FadeTime;
        }*/
    }
}