using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;

public class SwordAlly : Ally
{
    [SerializeField] private GameObject _BuffGameObject;
    [SerializeField] private float _BuffFactor;
    [SerializeField] private float _BuffDuration;
    private float _Time;
    private float _CurrentBuff = 1f;

    public override float GetStat(StatType pType)
    {
        float lBase = pType switch
        {
            StatType.MoveSpeed => baseStats.moveSpeed,
            StatType.MaxHP => baseStats.maxHealth,
            StatType.Damage => baseStats.AttackDamageFactor,
            StatType.AttackSpeed => baseStats.attackSpeedFactor * _CurrentBuff,
            StatType.Range => baseStats.additionalRange,
            StatType.PickupRange => baseStats.pickupRange,
            _ => 1f
        };
        return statsManager.GetFinalValue(pType, lBase);
    }

    public override void TakeDamage(float pAmount)
    {
        base.TakeDamage(pAmount);
        if (pAmount < 0 && health >= baseStats.maxHealth)
        {
            _Time = _BuffDuration;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (_Time <= 0) _CurrentBuff = 1f;
        else
        {
            _Time -= Time.deltaTime;
            _CurrentBuff = _BuffFactor;
        }
        _BuffGameObject.SetActive(_Time > 0);
    }
}
