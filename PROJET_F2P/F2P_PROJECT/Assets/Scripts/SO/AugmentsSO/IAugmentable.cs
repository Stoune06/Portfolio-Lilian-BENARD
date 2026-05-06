using System;
using System.Collections;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using Managers;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO
{
    public interface IAugmentable
    {
    public float GetStat(StatType pType);
    public PlayerStatsManager statsManager { get; }
    
    public float health { get; }
    public float targetMaxHealth { get; }
    public void Heal(float pAmount);
    
    public event Action<float, Transform> OnHealAlly;
    public event Action<int> OnWaveStart;
    public event Action OnHealCast;
    public event Action OnXPPickup;

    public void InvokeOnHealAlly(float pAmount, Transform pAlly);
    public void InvokeHealCast();
    

    public bool isStationary { get; }
    public Transform transform { get; }

    public Coroutine StartCoroutine(IEnumerator routine);
    public void StopCoroutine(Coroutine routine);
    

    }
}
