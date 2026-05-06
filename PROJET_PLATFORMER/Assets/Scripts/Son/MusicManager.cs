using UnityEngine;
using FMODUnity;
using System.Collections.Generic;

[RequireComponent(typeof(StudioEventEmitter))]
[RequireComponent(typeof(Collider2D))]
public class MusicManager : MonoBehaviour
{
    [Header("FMOD Settings")]
    public string parameterName = "TempleReached";

    private StudioEventEmitter emitter;
    private Collider2D zoneCollider;

    private ContactFilter2D filter;
    private List<Collider2D> results = new List<Collider2D>();

    private bool playerInside = false;

    private void Awake()
    {
        emitter = GetComponent<StudioEventEmitter>();
        zoneCollider = GetComponent<Collider2D>();

        zoneCollider.isTrigger = true;

        filter.useTriggers = true;
        filter.useLayerMask = false;

        // Initialiser à "No"
        emitter.EventInstance.setParameterByNameWithLabel(parameterName, "NO");
    }

    private void Update()
    {
        results.Clear();
        zoneCollider.Overlap(filter, results);

        bool foundPlayer = false;

        foreach (var col in results)
        {
            if (col.CompareTag("CustomPlayer"))
            {
                foundPlayer = true;
                break;
            }
        }

        // Entrée
        if (foundPlayer && !playerInside)
        {
            emitter.EventInstance.setParameterByNameWithLabel(parameterName, "Yes");
            Debug.Log("Yes");
            playerInside = true;
        }

        // Sortie
        if (!foundPlayer && playerInside)
        {
            emitter.EventInstance.setParameterByNameWithLabel(parameterName, "NO");
            Debug.Log("No");
            playerInside = false;
        }
    }
}

