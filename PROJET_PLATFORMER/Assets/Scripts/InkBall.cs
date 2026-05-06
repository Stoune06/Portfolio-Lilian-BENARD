using System;
using UnityEngine;

public class InkBall : MonoBehaviour
{
    [SerializeField]
    private float _Duration = 5f;

    public static Action<int> ammoRecall;
    private Vector3 _StartPos;
    private int _Index;
    private float _Progression;

    public void SetTargetAndStart(Vector3 pStartPos, Vector3 pEndPos, int pIndex)
    {
        _StartPos = pStartPos;
        _Index = pIndex;
        _Progression = 0f;
    }

    private void Update()
    {
        _Progression += Time.deltaTime / _Duration;

        if (_Progression >= 1f)
        {
            ammoRecall?.Invoke(_Index);
            Destroy(gameObject);
            return;
        }

        Vector3 lTarget = HUD.Instance.GetChargeWorldPosition(_Index);
        float lT = Mathf.SmoothStep(0f, 1f, _Progression);
        transform.position = Vector3.Lerp(_StartPos, lTarget, lT);
    }
}
