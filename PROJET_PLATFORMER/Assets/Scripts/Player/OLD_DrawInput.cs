using Platformer;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    [RequireComponent(typeof(LineRenderer))]
    public class DrawInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("References")]
        [SerializeField] private Camera _UiCamera;
        [SerializeField] private LineRenderer _LineRenderer;
        [SerializeField] private RecallUI _RecallUIPrefab;

        [Header("Placement Settings")]
        [SerializeField] private float _MaxPosDif = 2f;
        [SerializeField] private float _DistanceMin = 2f;
        [SerializeField] private float _ZDepth = 10f;

        [Header("Input Sensitivity")]
        [SerializeField] private float _HoldTimeRequired = 0.5f;
        [SerializeField] private float _HoldMovementTolerance = 20f;
        [SerializeField] private float _DistanceMinDrag = 0.20f;

        private Vector3 _StartScreenPos;
        private Vector3 _WorldStartPos;
        private bool _IsDrawing;
        private Coroutine _HoldCoroutine;

        // Events
        public static event Action OnHorizontalPlatformPlace;
        public static event Action OnVerticalPlatformPlace;
        public static event Action OnHorizontalVisualization;
        public static event Action OnVerticalVisualization;
        public static event Action OnRemoveVisualization;
        public static event Action OnRecallPlatform;

        private void Awake()
        {
            if (_LineRenderer == null) _LineRenderer = GetComponent<LineRenderer>();
            _LineRenderer.enabled = false;
        }

        #region Interface Implementation

        public void OnPointerDown(PointerEventData pEventData)
        {
            _StartScreenPos = pEventData.position;

            StopHoldTimer();
            _HoldCoroutine = StartCoroutine(HoldTimerRoutine(pEventData.position));
        }

        public void OnPointerUp(PointerEventData pEventData) => StopHoldTimer();

        public void OnBeginDrag(PointerEventData pEventData) => StopHoldTimer();

        public void OnDrag(PointerEventData pEventData)
        {
            if (!_IsDrawing && Vector2.Distance(pEventData.position, _StartScreenPos) > _DistanceMinDrag)
            {
                StartDrawing();
            }

            if (_IsDrawing)
            {
                UpdateLine(pEventData.position);
            }
        }

        public void OnEndDrag(PointerEventData pEventData)
        {
            if (_IsDrawing) FinalizeDrawing(pEventData.position);
        }

        #endregion

        #region Drawing Logic

        private void StartDrawing()
        {
            _IsDrawing = true;
            _LineRenderer.enabled = true;
            _WorldStartPos = ScreenToWorld(_StartScreenPos);

            _LineRenderer.SetPosition(0, _WorldStartPos);
            _LineRenderer.SetPosition(1, _WorldStartPos);
        }

        private void UpdateLine(Vector2 pScreenPos)
        {
            Vector3 lWorldEndPos = ScreenToWorld(pScreenPos);
            _LineRenderer.SetPosition(1, lWorldEndPos);

            ProcessOrientation(lWorldEndPos,
                pOnVertical: () => OnVerticalVisualization?.Invoke(),
                pOnHorizontal: () => OnHorizontalVisualization?.Invoke(),
                pOnNone: () => OnRemoveVisualization?.Invoke());
        }

        private void FinalizeDrawing(Vector2 pScreenPos)
        {
            _LineRenderer.enabled = false;
            _IsDrawing = false;
            OnRemoveVisualization?.Invoke();

            Vector3 lWorldEndPos = ScreenToWorld(pScreenPos);

            ProcessOrientation(lWorldEndPos,
                pOnVertical: () => OnVerticalPlatformPlace?.Invoke(),
                pOnHorizontal: () => OnHorizontalPlatformPlace?.Invoke(),
                pOnNone: null);
        }

        private void ProcessOrientation(Vector3 pWorldEndPos, Action pOnVertical, Action pOnHorizontal, Action pOnNone)
        {
            float lDistance = Vector3.Distance(_WorldStartPos, pWorldEndPos);

            if (lDistance < _DistanceMin)
            {
                pOnNone?.Invoke();
                return;
            }

            if (Mathf.Abs(_WorldStartPos.x - pWorldEndPos.x) <= _MaxPosDif)
                pOnVertical?.Invoke();
            else if (Mathf.Abs(_WorldStartPos.y - pWorldEndPos.y) <= _MaxPosDif)
                pOnHorizontal?.Invoke();
            else
                pOnNone?.Invoke();
        }

        #endregion

        #region Helpers & Coroutines

        private IEnumerator HoldTimerRoutine(Vector2 pStartPos)
        {
            float lTimer = 0f;
            RecallUI lRecall = Instantiate(_RecallUIPrefab, pStartPos, Quaternion.identity, null);

            while (lTimer < _HoldTimeRequired)
            {
                lTimer += Time.deltaTime;

                lRecall.IncrementCircle(_HoldTimeRequired / lTimer);
                if (Vector2.Distance(Input.mousePosition, pStartPos) > _HoldMovementTolerance)
                {
                    Destroy(lRecall.gameObject);
                    yield break;
                }

                yield return null;
            }

            if (!_IsDrawing) OnRecallPlatform?.Invoke();
        }

        private void StopHoldTimer()
        {
            if (_HoldCoroutine != null)
            {
                StopCoroutine(_HoldCoroutine);
                _HoldCoroutine = null;
            }
        }

        private Vector3 ScreenToWorld(Vector2 pScreenPos)
            => _UiCamera.ScreenToWorldPoint(new Vector3(pScreenPos.x, pScreenPos.y, _ZDepth));

        #endregion
    }
}