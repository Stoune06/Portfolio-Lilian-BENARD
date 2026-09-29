using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField] private Character _Chara;
    [SerializeField] private Vector3 _BaseGap;
    [SerializeField] private Image _Image;

    public void SetTarget(Transform pTransform, Vector3 pBaseGap)
    {
        _Chara = pTransform.GetComponent<Character>();
        _BaseGap = pBaseGap;
    }

    void Update()
    {
        if( _Chara == null) return;
        _Image.fillAmount = _Chara.health/ _Chara.baseStats.maxHealth;
        transform.position = _Chara.transform.position + _BaseGap;
    }
}
