using Com.IsartDigital.HealerSurvivor.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HitAnimation
{
    public Transform transform;
    public Renderer renderer;
    public Vector3 scale;
    public Color baseColor;
    public float time;
    public HitAnimationStruct animationStruct;

    public static event Action<HitAnimation> EndEvent;

    public HitAnimation(Transform pTransform, HitAnimationStruct pAnimationStruct)
    {
        transform = pTransform;
        renderer = pTransform.GetComponentInChildren<CharacterRenderer>().charaRenderer;
        baseColor = renderer.material.color;
        scale = renderer.transform.localScale;
        animationStruct = pAnimationStruct;
        time = 0f;
    }

    public void Update()
    {
        time += Time.deltaTime;
        if (transform == null || time > animationStruct.duration)
        {
            if (transform != null) SetAnimation(0f);
            EndEvent?.Invoke(this);
            return;
        }

        float lRatio;
        float lHalfDuration = animationStruct.duration * 0.5f;

        if (time > lHalfDuration) lRatio = time / lHalfDuration;
        else lRatio = 1f - (time + lHalfDuration) / animationStruct.duration;

        SetAnimation(lRatio);
    }

    private void SetAnimation(float pRatio)
    {
        float lAnimationRation = animationStruct.curve.Evaluate(pRatio);
        renderer.material.color = Color.Lerp(baseColor, animationStruct.color, lAnimationRation);
        renderer.transform.localScale = Vector3.Lerp(scale, scale * animationStruct.scaleFactor, lAnimationRation);
    }
}
