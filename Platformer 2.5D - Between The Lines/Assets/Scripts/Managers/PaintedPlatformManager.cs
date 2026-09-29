using Platformer.Player;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Platformer.Areas;

//Author : Sales Noé
namespace Platformer
{
    public class PaintedPlatformManager : MonoBehaviour
    {

        [Header("Stats")]
        [SerializeField] private int _MaxAmmo = 2;
        [Header("Parameters")]
        [SerializeField] private Vector2 _HorizontalMargin = Vector2.zero;
        [SerializeField] private Vector2 _VerticalMargin = Vector2.zero;
        [SerializeField] private float _InAirHorizontalMarginY = -3;
        [SerializeField] private float _InAirVerticalMarginY = 0;
        [Header("Objects")]
        [SerializeField] private Transform _Origin = null;
        [SerializeField] private Player.Player _Player = null;
        [SerializeField] private GameObject _PaintedPlatformPrefab;
        [SerializeField] private Transform _GameContainer;
        [SerializeField] private GameObject _VisPlatform;
        [SerializeField] private InkBall _InkBallPrefab;

        private List<PaintedPlaform> _ActivePlatform = new List<PaintedPlaform>();

        private const float MARGIN_X = 1, MARGIN_Y = 0.2f;
        private Vector2 _PlayerMargin;
        private int _ActualAmmo = 0;

        private void Start()
        {
            _VisPlatform = Instantiate(_VisPlatform);
            _VisPlatform.SetActive(false);
            PlatformRecall();
            GlobalPlatformPlacer.OnHorizontalPlatformPlace += SpawnHorizontalPlatform;
            GlobalPlatformPlacer.OnVerticalPlatformPlace += SpawnVerticalPlatform;
            GlobalPlatformPlacer.OnRecallPlatform += PlatformRecall;
            CheckPoint.CheckPointGetEvent += PlatformRecall;
            Player.Player.OnPlayerDeath += PlatformRecall;
            GlobalPlatformPlacer.OnHorizontalVisualization += VisualizeHorizontalPlatform;
            GlobalPlatformPlacer.OnVerticalVisualization += VisualizeVerticalPlatform;
            GlobalPlatformPlacer.OnRemoveVisualization += RemoveVisualization;
            InkBall.ammoRecall += OnAmmoRecalled;

            _PlayerMargin = _Player.GetComponent<BoxCollider2D>().size/2;
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetKeyDown(KeyCode.H))
            {
                //SpawnHorizontalPlatform();
                Debug.Log("Use your mouse!!");
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                //SpawnVerticalPlatform();
                Debug.Log("Use your mouse!!");
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                PlatformRecall();
            }
#endif
        }

        public void SpawnHorizontalPlatform(Vector3 pPos, float pDirection)
        {
            Vector2 lMargin = _HorizontalMargin;

            SpawnPlatform(Quaternion.AngleAxis(0, Vector3.forward), lMargin, pPos, pDirection);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/MC/Special Features/Brush_Stroke");
        }
        public void SpawnVerticalPlatform(Vector3 pPos, float pDirection)
        {
            Vector2 lMargin = _VerticalMargin;

            if (_Player.stateMachine.currentState is PlayerInAirState) lMargin.y = _InAirVerticalMarginY;

            SpawnPlatform(Quaternion.AngleAxis(90, Vector3.forward), lMargin, pPos, pDirection);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/MC/Special Features/Brush_Stroke");
        }

        private void SpawnPlatform(Quaternion pRotation, Vector2 pMargin, Vector3 pPos, float pDirection)
        {
            if (_ActualAmmo <= 0 || !_GameContainer) return;
            Vector3 lPos = pPos; //_Origin.position + (Vector3)pMargin;

            if(pRotation== Quaternion.AngleAxis(0, Vector3.forward))//Horizontal check
            {
                if (Mathf.Abs(lPos.x - _Player.transform.position.x) < MARGIN_X + _PlayerMargin.x &&
                    Mathf.Abs(lPos.y - _Player.transform.position.y) < MARGIN_Y + _PlayerMargin.y)
                {
                    Debug.Log("Illegal horizontal");
                    //Send out signal for failed platform?
                    return;
                }
            }
            else
            if (Mathf.Abs(lPos.x - _Player.transform.position.x) < MARGIN_Y + _PlayerMargin.x &&
                Mathf.Abs(lPos.y - _Player.transform.position.y) < MARGIN_X + _PlayerMargin.y)//Platform/player overlap check, basic
            {
                Debug.Log("Illegal vertical");
                //Send out signal for failed platform?
                return;
            }
            GameObject lPlatformObj = Instantiate(_PaintedPlatformPrefab, lPos, pRotation, _GameContainer);
            lPlatformObj.GetComponent<PaintedPlaform>().SetDirection(pDirection);
            

            _ActualAmmo--;

            /* Player based rotation, old
            if (Mathf.Abs(_Player.transform.eulerAngles.y - 180f) < 0.1f)
            {
                pMargin.x *= -1;
            }
            */

            
            RemoveVisualization();
            _ActivePlatform.Add(lPlatformObj.GetComponent<PaintedPlaform>());
            HUD.Instance.OnChargeUsed();
        }

        public void PlatformRecall()
        {
            if (_ActivePlatform.Count == 0)
            {
                _ActualAmmo = _MaxAmmo;
                HUD.Instance.OnChargeRecalled();
                return;
            }

            for (int i = 0; i < _ActivePlatform.Count; i++)
            {
                PaintedPlaform lPlat = _ActivePlatform[i];
                Vector3 lPlatPos = lPlat.transform.position;
                lPlat.Recall();

                Vector3 lTargetPos = HUD.Instance.GetChargeWorldPosition(i);
                InkBall lInkBall = Instantiate(_InkBallPrefab);
                lInkBall.transform.position = lPlatPos;
                lInkBall.SetTargetAndStart(lPlatPos, lTargetPos, i);
            }

            _ActivePlatform.Clear();
        }

        private void OnAmmoRecalled(int pIndex)
        {
            if (_ActualAmmo >= _MaxAmmo) return;
            _ActualAmmo++;
            HUD.Instance.OnSingleChargeRecalled(pIndex);
        }

        private void VisualizeHorizontalPlatform(Vector3 pPos)
        {
            Vector2 lMargin = _HorizontalMargin;

            Visualize(Quaternion.AngleAxis(0, Vector3.forward), lMargin, pPos);
        }

        private void VisualizeVerticalPlatform(Vector3 pPos)
        {
            Vector2 lMargin = _VerticalMargin;

            if (_Player.stateMachine.currentState is PlayerInAirState) lMargin.y = _InAirVerticalMarginY;

            Visualize(Quaternion.AngleAxis(90, Vector3.forward), lMargin, pPos);
        }

        private void Visualize(Quaternion pRotation, Vector2 pMargin, Vector3 pPos)
        {
            
            if (Mathf.Abs(_Player.transform.eulerAngles.y - 180f) < 0.1f)
            {
                pMargin.x *= -1;
            }

            Vector3 lPos = pPos;//_Origin.position + (Vector3)pMargin;
            _VisPlatform.transform.position = lPos;
            _VisPlatform.transform.rotation = pRotation;
            _VisPlatform.SetActive(true);

        }

        private void RemoveVisualization()
        {
            if(_VisPlatform)_VisPlatform.SetActive(false);
        }


        private void OnDisable()
        {            
            GlobalPlatformPlacer.OnHorizontalPlatformPlace -= SpawnHorizontalPlatform;
            GlobalPlatformPlacer.OnVerticalPlatformPlace -= SpawnVerticalPlatform;
            GlobalPlatformPlacer.OnRecallPlatform -= PlatformRecall;
            CheckPoint.CheckPointGetEvent -= PlatformRecall;
            Player.Player.OnPlayerDeath -= PlatformRecall;
            GlobalPlatformPlacer.OnHorizontalVisualization -= VisualizeHorizontalPlatform;
            GlobalPlatformPlacer.OnVerticalVisualization -= VisualizeVerticalPlatform;
            GlobalPlatformPlacer.OnRemoveVisualization -= RemoveVisualization;
            InkBall.ammoRecall -= OnAmmoRecalled;
        }

    }
}
