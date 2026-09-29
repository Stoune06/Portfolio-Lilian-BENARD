using Tooling;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.UI
{
    public class BouncyButton : MonoBehaviour
    {
        [SerializeField] private TweenProperty _Property;
        [SerializeField] Vector3 _StartScale = new Vector3(.8f, .8f, .8f);

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                transform.localScale = _StartScale;
                transform.ScaleTo(Vector3.one, _Property);
            });
        }
    }
}
