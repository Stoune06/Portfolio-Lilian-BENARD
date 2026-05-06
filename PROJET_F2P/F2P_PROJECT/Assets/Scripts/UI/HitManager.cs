using Com.IsartDigital.HealerSurvivor.Gameplay;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class HitManager : MonoBehaviour
{
    private List<HitAnimation> hitAnimations = new List<HitAnimation>();
    private List<Transform> transforms = new List<Transform>();

    [Header("HIT")]

    [SerializeField] private GameObject[] _ObjectsToInstantiateOnHit;
    [SerializeField] private HitAnimationStruct _HitAnimationOnHit;

    [Header("HEAL")]

    [SerializeField] private GameObject[] _ObjectsToInstantiateOnHeal;
    [SerializeField] private HitAnimationStruct _AnimationOnHeal;

    private void OnEnable()
    {
        AttackManager.OnAttackEvent += PlayHit;
        AttackManager.OnHealEvent += PlayHeal;
        HitAnimation.EndEvent += StopAnimation;
    }

    private void StopAnimation(HitAnimation pAnimation)
    {
        hitAnimations.Remove(pAnimation);
        transforms.Remove(pAnimation.transform);
    }

    private void PlayHit(Transform pTransform)
    {
        if (transforms.Contains(pTransform)) 
            hitAnimations[transforms.IndexOf(pTransform)].time = 0f;
        else
        {
            hitAnimations.Add(new HitAnimation(pTransform,_HitAnimationOnHit));
            transforms.Add(pTransform);
        }

        foreach (GameObject lObject in _ObjectsToInstantiateOnHit)
        {
            Instantiate(lObject, pTransform.position, lObject.transform.rotation);
        }
    }

    private void PlayHeal(Transform pTransform)
    {
        if (transforms.Contains(pTransform))
            hitAnimations[transforms.IndexOf(pTransform)].time = 0f;
        else
        {
            hitAnimations.Add(new HitAnimation(pTransform, _AnimationOnHeal));
            transforms.Add(pTransform);
        }

        foreach (GameObject lObject in _ObjectsToInstantiateOnHeal)
        {
            Instantiate(lObject, pTransform.position, lObject.transform.rotation);
        }
    }

    private void Update()
    {
        int lLength = hitAnimations.Count - 1;
        for (int i = lLength; i >= 0; i--)
        {
            hitAnimations[i].Update();
        }
    }

    private void OnDisable()
    {
        AttackManager.OnAttackEvent -= PlayHit;
        AttackManager.OnHealEvent -= PlayHeal;
        HitAnimation.EndEvent -= StopAnimation;
    }
}

[Serializable]
public struct HitAnimationStruct
{
    public float duration;
    public Color color;
    public float scaleFactor;
    public AnimationCurve curve;
}
