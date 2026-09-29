using System.Collections;
using UnityEngine;

public class TileGhostVisuals : MonoBehaviour
{
    [Header("Ghost Settings")]
    [SerializeField] private float _GhostBaseScale = 0.7f;
    [SerializeField] private float _PulseSpeed = 5f;
    [SerializeField] private float _PulseAmount = 0.05f;

    [Header("Placement Animation")]
    [SerializeField] private float _AnimationDuration = 0.4f;

    public AnimationCurve scaleCurve = new AnimationCurve(
        new Keyframe(0, 0.7f),
        new Keyframe(0.7f, 1.2f),
        new Keyframe(1, 1)
    );
    public AnimationCurve jumpCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.5f, 0.5f),
        new Keyframe(1, 0)
    );

    private bool _IsPlaced = false;
    private Vector3 _FinalPosition;

    private void Start()
    {
        transform.localScale = Vector3.one * _GhostBaseScale;
    }

    private void Update()
    {
        if (!_IsPlaced)
        {
            AnimateGhost();
        }
    }

    private void AnimateGhost()
    {
        float scaleOffset = Mathf.Sin(Time.time * _PulseSpeed) * _PulseAmount;
        float currentScale = _GhostBaseScale + scaleOffset;

        transform.localScale = Vector3.one * currentScale;
    }

    public void TriggerPlacementAnimation()
    {
        _IsPlaced = true;
        _FinalPosition = transform.position;
        StartCoroutine(PlacementRoutine());
    }

    private IEnumerator PlacementRoutine()
    {
        float elapsed = 0f;

        while (elapsed < _AnimationDuration)
        {
            float ratio = elapsed / _AnimationDuration;

            float scaleValue = scaleCurve.Evaluate(ratio);
            transform.localScale = Vector3.one * scaleValue;

            float jumpHeight = jumpCurve.Evaluate(ratio);
            transform.position = _FinalPosition + Vector3.up * jumpHeight;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
        transform.position = _FinalPosition;
    }
}