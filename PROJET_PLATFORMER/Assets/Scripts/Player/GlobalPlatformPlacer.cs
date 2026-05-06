using Player;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Platformer.Player
{
    public class GlobalPlatformPlacer : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("References")]
        [SerializeField] private Camera _UiCamera;
        [SerializeField] private RecallUI _RecallUI;
        [SerializeField] private Transform _Player;

        [Header("Placement Settings")]
        public bool CanDraw = true;
        [SerializeField] private float _MaxPosDif = 2f;
        [SerializeField] private float _DistanceMin = 2f;
        [SerializeField] private float _ZDepth = 10f;

        [Header("Brush Visuals")]
        [SerializeField] private Transform _StrokeLinePrefab;
        [SerializeField] private BrushCursor _BrushCursor;
        [SerializeField] private float _MinVertexDistance = 0.1f;

        [Header("Input Sensitivity")]
        [SerializeField] private float _HoldTimeRequired = 0.5f;
        [SerializeField] private float _HoldMovementTolerance = 20f;
        [SerializeField] private float _DistanceMinDrag = 0.20f;

        private Vector3 _StartScreenPos;
        private Vector3 _WorldStartPos;
        private bool _IsDrawing;
        private Coroutine _HoldCoroutine;
        private LineRenderer _CurrentStrokeRenderer;
        private int _StrokeVertexCount;
        private Vector3 _LastVertexWorldPos;
        
        public static event Action<Vector3, float> OnHorizontalPlatformPlace;
        public static event Action<Vector3, float> OnVerticalPlatformPlace;
        public static event Action<Vector3> OnHorizontalVisualization;
        public static event Action<Vector3> OnVerticalVisualization;
        public static event Action OnRemoveVisualization;
        public static event Action OnRecallPlatform;

        private const float OFFSET = 2f;
        private void Awake()
        {
            if (_BrushCursor != null)
                _BrushCursor.gameObject.SetActive(false);
            Player.OnPlayerDeath += OnPlayerDeath;
            Player.OnPlayerRespawn += OnPlayerRespawn;
        }

        public void OnPointerDown(PointerEventData pEventData)
        {
            if (!CanDraw) return;

            _StartScreenPos = pEventData.position;
            StopHoldTimer();
            _HoldCoroutine = StartCoroutine(HoldTimerRoutine(pEventData.position));
        }

        public void OnPointerUp(PointerEventData pEventData)
        {
            if(!CanDraw) return;
            StopHoldTimer();
        }

        /*public void OnBeginDrag(PointerEventData pEventData)
        {
            if (!CanDraw) return;
            StopHoldTimer();
        }*/

        private void StartDrawing()
        {
            _IsDrawing = true;
            _WorldStartPos = ScreenToWorld(_StartScreenPos);

            if (_StrokeLinePrefab != null)
            {
                Transform lStrokeObj = Instantiate(_StrokeLinePrefab,transform);
                _CurrentStrokeRenderer = lStrokeObj.GetComponent<LineRenderer>();
                _StrokeVertexCount = 0;
                AddStrokeVertex(_WorldStartPos);
            }

            if (_BrushCursor != null)
            {
                Vector3 lPlayerScreenPos = Camera.main.WorldToScreenPoint(_Player.position);
                Vector3 lPlayerInUIWorld = _UiCamera.ScreenToWorldPoint(
                    new Vector3(lPlayerScreenPos.x, lPlayerScreenPos.y, _ZDepth + 15));
                _BrushCursor.Activate(_StartScreenPos, _UiCamera, lPlayerInUIWorld);
            }
        }

        private void AddStrokeVertex(Vector3 pWorldPos)
        {
            _CurrentStrokeRenderer.positionCount = ++_StrokeVertexCount;
            _CurrentStrokeRenderer.SetPosition(_StrokeVertexCount - 1, pWorldPos);
            _LastVertexWorldPos = pWorldPos;
        }

        private void UpdateLine(Vector2 pScreenPos)
        {
            Vector3 lWorldEndPos = ScreenToWorld(pScreenPos);

            if (_CurrentStrokeRenderer != null
                && Vector3.Distance(_LastVertexWorldPos, lWorldEndPos) >= _MinVertexDistance)
            {
                AddStrokeVertex(lWorldEndPos);
            }

            if (_BrushCursor != null)
                _BrushCursor.UpdatePosition(pScreenPos);

            Vector3 lPos = LocalToWorld(lWorldEndPos - (lWorldEndPos - _WorldStartPos) / 2);

            ProcessOrientation(lWorldEndPos,
                pOnVertical: () => OnVerticalVisualization?.Invoke(lPos),
                pOnHorizontal: () => OnHorizontalVisualization?.Invoke(lPos),
                pOnNone: () => OnRemoveVisualization?.Invoke());
        }

        private void FinalizeDrawing(Vector2 pScreenPos)
        {
            _IsDrawing = false;
            OnRemoveVisualization?.Invoke();

            if (_BrushCursor != null)
            {
                Vector3 lPlayerScreenPos = Camera.main.WorldToScreenPoint(_Player.position);
                Vector3 lPlayerInUIWorld = _UiCamera.ScreenToWorldPoint(
                    new Vector3(lPlayerScreenPos.x, lPlayerScreenPos.y, _ZDepth + 15));
                _BrushCursor.DeactivateWithReturn(lPlayerInUIWorld);
            }

            if (_CurrentStrokeRenderer != null)
            {
                Destroy(_CurrentStrokeRenderer.gameObject);
                _CurrentStrokeRenderer = null;
            }

            Vector3 lWorldEndPos = pScreenPos;
            Vector3 lStart = LocalToWorld(_StartScreenPos);
            Vector3 lEnd = LocalToWorld(lWorldEndPos);
            Vector3 lPos;

            //If middle of line
            /*lPos = lEnd - ((lEnd - lStart) / 2);
            */

            //If start of line
            lPos = lStart - lEnd;
            Vector2 lCheck = Vector2.zero;
            if (lPos.x < 0)
            {
                lCheck.x = 1;
            }
            else lCheck.x = -1;
            if(lPos.y < 0)
            {
                lCheck.y = 1;
            }
            else lCheck.y = -1;

            lPos = lStart;
            lPos.z = 0;
            ProcessOrientation(lWorldEndPos,
                pOnVertical: () => OnVerticalPlatformPlace?.Invoke(lPos + (Vector3.up * OFFSET * lCheck.y), lCheck.y),
                pOnHorizontal: () => OnHorizontalPlatformPlace?.Invoke(lPos + (Vector3.right * OFFSET * lCheck.x), lCheck.x),
                pOnNone: null);
        }

        private void ProcessOrientation(Vector3 pWorldEndPos, Action pOnVertical, Action pOnHorizontal, Action pOnNone)
        {
            float lDistance = Vector3.Distance(_StartScreenPos, pWorldEndPos);

            if (lDistance < _DistanceMin)
            {
                
                pOnNone?.Invoke();
                return;
            }
            float lHorizontalDif = Mathf.Abs(_StartScreenPos.x - pWorldEndPos.x);
            float lVerticalDif = Mathf.Abs(_StartScreenPos.y - pWorldEndPos.y);
            /*if (Mathf.Abs(_StartScreenPos.x - pWorldEndPos.x) <= _MaxPosDif)
                pOnVertical?.Invoke();
            else if (Mathf.Abs(_StartScreenPos.y - pWorldEndPos.y) <= _MaxPosDif)
                pOnHorizontal?.Invoke();
            else
                pOnNone?.Invoke();*/
            //Debug.Log(lHorizontalDif + " " + lVerticalDif);
            if (lHorizontalDif > lVerticalDif && lVerticalDif <= _MaxPosDif)  pOnHorizontal?.Invoke();
            else if (lVerticalDif > lHorizontalDif && lHorizontalDif <= _MaxPosDif) pOnVertical?.Invoke();
            else pOnNone?.Invoke();
        }

        private IEnumerator HoldTimerRoutine(Vector2 pStartPos)
        {
            float lTimer = 0f;
            
            while (lTimer < _HoldTimeRequired)
            {
                lTimer += Time.deltaTime;

                if (lTimer > _HoldTimeRequired * 0.3f && _RecallUI.gameObject.active == false)
                {
                    _RecallUI.gameObject.SetActive(true);
                    _RecallUI.transform.position = ScreenToWorld(Input.mousePosition);
                }

                _RecallUI.IncrementCircle(lTimer / _HoldTimeRequired);
                if (Vector2.Distance(Input.mousePosition, pStartPos) > _HoldMovementTolerance)
                {
                    //Debug.Log(Vector2.Distance(Input.mousePosition, pStartPos) + " " + _HoldMovementTolerance);

                    _RecallUI.gameObject.SetActive(false);
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
                _RecallUI.gameObject.SetActive(false);
                _HoldCoroutine = null;
            }
        }

        public void OnDrag(PointerEventData pEventData)
        {
            if (!CanDraw) return;

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
            if (_IsDrawing && CanDraw)
            {
                FinalizeDrawing(pEventData.position);
            }
            else
            {
                _IsDrawing = false;
            }
        }

        private void OnPlayerDeath()
        {
            CanDraw = false;
            StopHoldTimer();

            if (_IsDrawing || _CurrentStrokeRenderer != null)
            {
                _IsDrawing = false;

                if (_CurrentStrokeRenderer != null)
                {
                    Destroy(_CurrentStrokeRenderer.gameObject);
                    _CurrentStrokeRenderer = null;
                }

                if (_BrushCursor != null)
                {
                    _BrushCursor.gameObject.SetActive(false);
                }

                OnRemoveVisualization?.Invoke();
            }
        }

        private void OnPlayerRespawn(int pValue) => CanDraw = true;

        private Vector3 ScreenToWorld(Vector2 pScreenPos)
            => _UiCamera.ScreenToWorldPoint(new Vector3(pScreenPos.x, pScreenPos.y, _ZDepth));

        private Vector3 LocalToWorld(Vector3 pPos)
        {
            if (Camera.main == null) return Vector3.zero;
            return Camera.main.ScreenToWorldPoint(pPos);
        }

        private void OnDestroy()
        {
            Player.OnPlayerDeath += OnPlayerDeath;
            Player.OnPlayerRespawn += OnPlayerRespawn;
        }
    }
}