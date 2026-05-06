using Platformer.Tools;
using System.Collections;
using UnityEngine;

//Author : Sales Noé
namespace Platformer.Killzone
{
    public class DropSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject DropPrefab;

        [SerializeField] private float _TimeBetweenSpawn = 0.5f;
        [SerializeField] private bool _Active = true;

        private Coroutine _SpawnCoroutine = null;

        private void Update()
        {
            if (_Active && _SpawnCoroutine == null)
            {
                _SpawnCoroutine = StartCoroutine(CustomTools.Timer(_TimeBetweenSpawn, SpawnDrop));
            }
        }

        private void SpawnDrop()
        {
            Instantiate(DropPrefab, transform.position, Quaternion.identity, transform);
            _SpawnCoroutine = null;
        }
    }
}