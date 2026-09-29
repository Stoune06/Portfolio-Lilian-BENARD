using Tooling;
using UnityEngine.UI;
using UnityEngine;

public class NotifTween : MonoBehaviour
{
    [SerializeField] private TweenProperty _TweenProperty;
    private Tween _Tween;

    void Start()
    {
        Image lImage = GetComponent<Image>();

        _Tween = new Tween(_TweenProperty);
        Tween lTweenImage = new Tween(_TweenProperty);
        _Tween.Start(transform, nameof(transform.position), transform.position + Vector3.up * 10);
        lTweenImage.Start(lImage, nameof(lImage.color), new Color(1, 1, 1, 0));

        _Tween.finished += OnEnd;
    }

    private void OnEnd()
    {
        _Tween.finished -= OnEnd;
        Destroy(gameObject);
    }
}
