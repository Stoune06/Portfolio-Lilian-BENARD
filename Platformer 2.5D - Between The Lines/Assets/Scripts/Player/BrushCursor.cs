using UnityEngine;
using UnityEngine.UIElements;

namespace Player
{
    public class BrushCursor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator _Animator;

        [Header("Settings")]
        [SerializeField] private float _FollowSmoothing = 0.5f;
        [SerializeField] private float _AnimatorSmoothing = 0.07f;
        [SerializeField] private float _ZDepth = 10f;
        [SerializeField] private float _TimeToMove = 1f;

        private Camera _Camera;
        private Vector2 _PreviousScreenPos;
        private float _Progression;
        private Vector3 _StartPos;
        private Vector3 _TargetPos;
        private bool _IsMoving;
        private bool _IsReturning;
        private FMOD.Studio.EventInstance Brush_Action;

        public void Activate(Vector2 pScreenPos, Camera pCamera, Vector3 pPlayerPos)
        {
            _IsMoving = true;
            _IsReturning = false;
            _Progression = 0f;
            _Camera = pCamera;
            transform.position = pPlayerPos;
            _StartPos = pPlayerPos;
            _TargetPos = _Camera.ScreenToWorldPoint(new Vector3(pScreenPos.x, pScreenPos.y, _ZDepth));
            gameObject.SetActive(true);
            _PreviousScreenPos = pScreenPos;
            Brush_Action = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/MC/Special Features/Brush_Action");
            Brush_Action.start();
            Brush_Action.setParameterByName("Speed", 0f);
        }

        private void Update()
        {
            if (_Progression < 1f)
            {
                _Progression += Time.deltaTime / _TimeToMove;
                transform.position = Vector3.Slerp(_StartPos, _TargetPos, _Progression);
            }
            else if (_IsMoving)
            {
                _IsMoving = false;
                if (_IsReturning) Deactivate();
            }
        }

        public void DeactivateWithReturn(Vector3 pPlayerPos)
        {
            _IsMoving = true;
            _IsReturning = true;
            _Progression = 0f;
            _StartPos = transform.position;
            _TargetPos = pPlayerPos;

            if (_Animator != null)
                _Animator.SetBool("isDrawing", false);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            if (_Animator != null)
            {
                _Animator.SetFloat("X", 0f);
                _Animator.SetFloat("Y", 0f);
                _Animator.SetBool("isDrawing", false);
                Brush_Action.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        public void UpdatePosition(Vector2 pScreenPos)
        {
            if (_Camera == null) return;

            Vector3 lTargetWorld = _Camera.ScreenToWorldPoint(
                new Vector3(pScreenPos.x, pScreenPos.y, _ZDepth));
            transform.position = Vector3.Lerp(
                transform.position, lTargetWorld, _FollowSmoothing);

            if (_Animator != null)
            {
                Vector2 lDelta = pScreenPos - _PreviousScreenPos;
                _Animator.SetFloat("X",
                    Mathf.Lerp(_Animator.GetFloat("X"), lDelta.x, _AnimatorSmoothing));
                _Animator.SetFloat("Y",
                    Mathf.Lerp(_Animator.GetFloat("Y"), lDelta.y, _AnimatorSmoothing));
                _Animator.SetBool("isDrawing", true);
                Brush_Action.setParameterByName("Angle", Vector2.Angle(_PreviousScreenPos, pScreenPos) / 360f);
                float lSpeed = lDelta.magnitude;
                if (lSpeed < 0.5f) lSpeed = 0f;
                Brush_Action.setParameterByName("Speed", Mathf.Clamp(lSpeed, 0, 1));



            }
            _PreviousScreenPos = pScreenPos;
        }
    }
}
