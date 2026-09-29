using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OnTickBasedObject : MonoBehaviour
{
    protected int m_TickElapsed = 0;

    protected virtual void Start()
    {
        //if(isActiveAndEnabled)TickManager.onTick += OnTickEvent;
    }

    protected virtual void OnTickEvent()
    {
        //if(!isActiveAndEnabled) return;
        m_TickElapsed++;
    }

    private void OnDisable()
    {
        TickManager.onTick -= OnTickEvent;
    }

    private void OnEnable()
    {
        TickManager.onTick += OnTickEvent;
    }

    protected virtual void OnDestroy()
    {
        TickManager.onTick -= OnTickEvent;
    }
}
