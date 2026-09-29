using Platformer.Managers;
using Tooling;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HUD : Singleton<HUD>
{
    private GameManager _GameManager;

    [SerializeField] private Text _SmallCollectibleText;
    [SerializeField] private Text _LargeCollectibleText;
    [SerializeField] private GameObject _CollectNotificationPrefab;
    [SerializeField] public Toggle _PaintChargeFirst;
    [SerializeField] public Toggle _PaintChargeLast;

    [SerializeField] private TweenProperty _BellImageProperty;
    [SerializeField] private Image _BellImage;
    [SerializeField] private float _BellTilt = 30f;

    [Header("Tween")]
    [SerializeField] private TweenProperty _CollectibleProperty;
    [SerializeField] Vector3 _CollectibleStartScale = new Vector3(.8f, .8f, .8f);
    [SerializeField] private TweenProperty _CollectibleNotifProperty;


    [SerializeField] private TweenProperty _CollectibleDisableProperty;
    [SerializeField] Vector3 _CollectibleDisableScale = new Vector3(1.2f, 1.2f, 1.2f);

    private void Start()
    {
        _GameManager = GameManager.Instance;
        if (_GameManager != null) _LargeCollectibleText.text = _GameManager.GetCurrentData().collectibles.ToString();

        InkBall.ammoRecall += (p) => {

            if (_PaintChargeFirst == null) return;

            if (p == 0)
            {
                _PaintChargeFirst.transform.localScale = _CollectibleStartScale;
                _PaintChargeFirst.transform.ScaleTo(Vector3.one, _CollectibleProperty);
            }
            else if (p >= 1)
            {
                _PaintChargeLast.transform.localScale = _CollectibleStartScale;
                _PaintChargeLast.transform.ScaleTo(Vector3.one, _CollectibleProperty);
            }
        };
    }

    public void OnCollected(bool pBig)
    {
        if (_GameManager == null) return;

        SaveData lCurrentSave = _GameManager.GetCurrentData();
        _GameManager.SetCurrentData(lCurrentSave);
        lCurrentSave.collectibles += 1;
        _LargeCollectibleText.text = lCurrentSave.collectibles.ToString();

        
        _LargeCollectibleText.transform.localScale = _CollectibleStartScale;
        _LargeCollectibleText.transform.ScaleTo(Vector3.one, _CollectibleProperty);
        GameObject lNotif = Instantiate(_CollectNotificationPrefab,_LargeCollectibleText.transform.parent);
        lNotif.transform.position += Vector3.down * 5;

        //_BellImage.transform.RotateZTo(_BellTilt, _BellImageProperty);
    }

    public void OnChargeUsed()
    {
        if (_PaintChargeFirst.isOn)
        {
            _PaintChargeFirst.isOn = false;
            _PaintChargeFirst.transform.localScale = _CollectibleStartScale;
            _PaintChargeFirst.transform.ScaleTo(Vector3.one, _CollectibleProperty);


            _PaintChargeFirst.transform.localScale = _CollectibleStartScale;
            _PaintChargeFirst.transform.ScaleTo(Vector3.one, _CollectibleProperty);

        }
        else if (_PaintChargeLast.isOn)
        {
            _PaintChargeLast.isOn = false;

            _PaintChargeLast.transform.localScale = _CollectibleStartScale;
            _PaintChargeLast.transform.ScaleTo(Vector3.one, _CollectibleProperty);
        }
        else
        {
            _PaintChargeFirst.transform.localScale = _CollectibleDisableScale;
            _PaintChargeFirst.transform.ScaleTo(Vector3.one, _CollectibleDisableProperty);

            _PaintChargeLast.transform.localScale = _CollectibleDisableScale;
            _PaintChargeLast.transform.ScaleTo(Vector3.one, _CollectibleDisableProperty);
        }
    }

    public void OnChargeRecalled()
    {
        _PaintChargeFirst.isOn = true;
        _PaintChargeLast.isOn = true;

        _PaintChargeFirst.transform.localScale = _CollectibleStartScale;
        _PaintChargeFirst.transform.ScaleTo(Vector3.one, _CollectibleProperty);

        _PaintChargeLast.transform.localScale = _CollectibleStartScale;
        _PaintChargeLast.transform.ScaleTo(Vector3.one, _CollectibleProperty);
    }

    public void OnSingleChargeRecalled(int pIndex)
    {
        Toggle lTarget = pIndex == 0 ? _PaintChargeFirst : _PaintChargeLast;
        lTarget.isOn = true;
    }

    public Vector3 GetChargeWorldPosition(int pIndex)
    {
        Transform lToggle = pIndex == 0
            ? _PaintChargeFirst.transform
            : _PaintChargeLast.transform;

        Camera lUICamera = GetComponentInParent<Canvas>().worldCamera;
        Vector3 lScreenPos = lUICamera.WorldToScreenPoint(lToggle.position);
        lScreenPos.z = 10f;
        if(Camera.main) return Camera.main.ScreenToWorldPoint(lScreenPos);
        else return Vector3.one * 999;
    }
}
