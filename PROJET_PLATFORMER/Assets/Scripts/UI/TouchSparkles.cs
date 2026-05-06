using UnityEngine;

public class TouchSparkles : MonoBehaviour
{
    [SerializeField] private GameObject _TouchVFX;

    private bool _Mouse=true;
    [SerializeField] private Vector3 _PosOffset;

    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        _Mouse = true;
#endif
#if UNITY_IOS || UNITY_ANDROID //|| UNITY_EDITOR
        _Mouse = false;
#endif
    }

    void Update()
    {
        if (_Mouse && Input.GetMouseButtonUp(0))
        {
            Instantiate(_TouchVFX, Camera.main.ScreenToWorldPoint(Input.mousePosition)+ _PosOffset + Vector3.forward*10, Quaternion.identity);
        }
        else if (Input.touchCount > 0)
        {
            Instantiate(_TouchVFX, Input.GetTouch(0).position, Quaternion.identity);
        }
    }
}
