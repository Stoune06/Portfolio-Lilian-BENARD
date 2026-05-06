using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(Collider2D))]
public class FMODTriggerEvent2D : MonoBehaviour
{
    [Header("FMOD Event")]
    [EventRef]
    [SerializeField] private string fmodEventPath;

    private Collider2D triggerCollider;

    // Tableau réutilisé pour éviter les allocations GC
    private readonly Collider2D[] results = new Collider2D[8];

    private bool hasPlayed = false;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    private void Update()
    {
        if (hasPlayed) return;

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(Physics2D.DefaultRaycastLayers);
        filter.useLayerMask = true;

        int count = triggerCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i].CompareTag("CustomPlayer"))
            {
                hasPlayed = true;
                PlayFMODEvent();
                break;
            }
        }
    }

    private void PlayFMODEvent()
    {
        if (!string.IsNullOrEmpty(fmodEventPath))
        {
            RuntimeManager.PlayOneShot(fmodEventPath, transform.position);
        }
        else
        {
            Debug.LogWarning("FMOD Event Path is empty on " + gameObject.name);
        }
    }
}
