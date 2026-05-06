using Tooling;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTweenDelay : MonoBehaviour
{

    private Tween _Tween;
    [SerializeField] private TweenProperty _TweenProperty;

    //[SerializeField] private TweenProperty _TweenProperty;
    [SerializeField] Vector3 _StartScale = new Vector3(.8f, .8f, .8f);


    private void OnEnable()
    {
        _Tween = new Tween(_TweenProperty);
        _Tween.finished += FollowUp;
    }
    public void TweenStart()
    {
        transform.localScale = _StartScale;
        _Tween.Start(transform, nameof(transform.localScale), Vector3.one);
        GetComponent<Button>().interactable = false;
    }

    private void FollowUp()
    {
        GetComponent<HideUi>().Hide();
        GetComponent<LoadPrefab>().Load();
        GetComponent<Button>().interactable = true;
    }

    private void OnDisable()
    {
        _Tween.finished -= FollowUp;
    }
}
