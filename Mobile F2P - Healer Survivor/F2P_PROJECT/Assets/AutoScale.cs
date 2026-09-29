using System;
using UnityEngine;

public class AutoScale : MonoBehaviour
{
    [SerializeField] private float _Duration;
    private float _Time;

    [SerializeField] private AnimationCurve _Curve;

    private Action DoAction;

    private void OnEnable()
    {
        DoAction = AutoScaleOnDuration;
        _Time = 0f;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (DoAction != null) DoAction();
    }

    private void AutoScaleOnDuration()
    {
        _Time += Time.deltaTime;
        if (_Time >= _Duration)
        {
            _Time = _Duration;
            DoAction = null;
        }
        transform.localScale = Vector3.Lerp(Vector3.zero,Vector3.one,_Curve.Evaluate(_Time/_Duration));
    }

    private void OnDisable()
    {
        DoAction = null;
    }
}
