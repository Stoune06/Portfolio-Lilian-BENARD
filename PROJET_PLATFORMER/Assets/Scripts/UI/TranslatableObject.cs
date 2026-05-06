using Tooling;
using UnityEngine;
using UnityEngine.UI;

public class TranslatableObject : MonoBehaviour
{
    [SerializeField] public Translable<string> translation;

    [SerializeField] private TweenProperty _Property;
    [SerializeField] Vector3 _StartScale = new Vector3(.8f, .8f, .8f);

    private void Start()
    {
        GetComponent<Text>().text = translation.Value;
        Language.OnLanguageChanged += OnChange;
    }


    private void OnChange()
    {
        GetComponent<Text>().text = translation.Value;

        transform.localScale = _StartScale;
        transform.ScaleTo(Vector3.one, _Property);
    }

    private void OnDestroy()
    {
        Language.OnLanguageChanged -= OnChange;
    }

}
