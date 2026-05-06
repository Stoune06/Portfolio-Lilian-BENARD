using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes;
using UnityEngine;

public class AutoScaleFromRange : MonoBehaviour
{
    [SerializeField] private AttackType attackType;

    private void Update()
    {
        transform.localScale = new Vector3(1f,0f,1f) * attackType.visibleRange;
    }
}
