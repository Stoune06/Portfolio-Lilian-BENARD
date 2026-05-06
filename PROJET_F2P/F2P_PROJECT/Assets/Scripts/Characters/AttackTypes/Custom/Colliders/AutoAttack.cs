using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes;
using Com.IsartDigital.HealerSurvivor.Gameplay.AttackTypes.Custom;
using UnityEngine;

[RequireComponent(typeof(AttackType))]
public class AutoAttack : CustomAttack
{
    private AttackType _AttackType;

    private void Start()
    {
        SetAttack();
    }


    protected override void Update()
    {
        base.Update();
        _AttackType.TryAttack();
    }

    private void SetAttack()
    {
        _AttackType = GetComponent<AttackType>();
        _AttackType.OverrideProviders(m_stats.statProvider, m_stats.canal);
    }
}
