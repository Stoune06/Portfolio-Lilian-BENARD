using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;

    [SerializeField] private CanvasGroup _CanvasGroup;
    [SerializeField] private float _FadeDuration = 0.5f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        if (_CanvasGroup != null)
        {
            _CanvasGroup.alpha = 0f;
            _CanvasGroup.blocksRaycasts = false;
        }
    }

    public IEnumerator FadeOut()
    {
        if (_CanvasGroup == null) yield break;

        _CanvasGroup.blocksRaycasts = true; // Bloque les clics pendant la transition
        float lElapsed = 0f;

        while (lElapsed < _FadeDuration)
        {
            lElapsed += Time.deltaTime;
            _CanvasGroup.alpha = Mathf.Clamp01(lElapsed / _FadeDuration);
            yield return null;
        }
        _CanvasGroup.alpha = 1f;
    }

    public IEnumerator FadeIn()
    {
        if (_CanvasGroup == null) yield break;

        float lElapsed = 0f;

        while (lElapsed < _FadeDuration)
        {
            lElapsed += Time.deltaTime;
            _CanvasGroup.alpha = 1f - Mathf.Clamp01(lElapsed / _FadeDuration);
            yield return null;
        }
        _CanvasGroup.alpha = 0f;
        _CanvasGroup.blocksRaycasts = false; // Rend la main au joueur
    }
}
