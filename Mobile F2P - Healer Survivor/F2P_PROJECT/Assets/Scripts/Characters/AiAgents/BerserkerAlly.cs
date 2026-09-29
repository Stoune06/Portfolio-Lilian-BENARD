using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;

public class BerserkerAlly : Ally
{
    [SerializeField] private GameObject _BuffGameObject;
    [SerializeField] private float _BuffFactor;

    private float _CurrentBuff = 1f;

    public override float GetStat(StatType pType)
    {
        float lBase = pType switch
        {
            StatType.MoveSpeed => baseStats.moveSpeed,
            StatType.MaxHP => baseStats.maxHealth,
            StatType.Damage => baseStats.AttackDamageFactor * _CurrentBuff,
            StatType.AttackSpeed => baseStats.attackSpeedFactor,
            StatType.Range => baseStats.additionalRange,
            StatType.PickupRange => baseStats.pickupRange,
            _ => 1f
        };
        return statsManager.GetFinalValue(pType, lBase);
    }

    protected override void Update()
    {
        base.Update();
        if (health > baseStats.maxHealth * 0.5f) _CurrentBuff = 1f;
        else _CurrentBuff = _BuffFactor;
        _BuffGameObject.SetActive(health < baseStats.maxHealth * 0.5f);
    }
}
