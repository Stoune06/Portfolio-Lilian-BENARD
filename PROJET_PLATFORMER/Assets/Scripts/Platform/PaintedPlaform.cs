using Platformer.Platforms;
using System;
using Tooling;
using UnityEngine;

//Author : Sales Noé
namespace Platformer
{
    public class PaintedPlaform : MonoBehaviour
    {
        [SerializeField] private float _SpawnDuration = 0.3f;

        private Material _Material;

        private float _ElapsedTime = 0f;

        private bool _JustSpawned = false;
        private float _Direction = 1f;

        public void SetDirection(float pDirection)
        {
            _Direction = pDirection;
        }

        private void Start()
        {
            _JustSpawned = true;
            _Material = GetComponentInChildren<SpriteRenderer>().material;
            _Material.SetFloat("_Direction", _Direction);
            _Material.SetFloat("_CutoffHeight", -2.6f);
            SetToOneWay();
        }

        private void Update()
        {
            if (_ElapsedTime <= _SpawnDuration && _JustSpawned)
            {
                _ElapsedTime += Time.deltaTime;
                float lDissolveValue = Mathf.Lerp(-2.6f, 2.6f, _ElapsedTime / _SpawnDuration);

                _Material.SetFloat("_CutoffHeight", lDissolveValue);
            }
            else
            {
                _JustSpawned = false;
                _ElapsedTime = 0f;
            }
        }

        private void SetToOneWay()
        {
            if (transform.eulerAngles.z == 0f)
            {
                gameObject.AddComponent<OneWayPlatform>();
                Destroy(GetComponent<PlatformCollision>());
            }
        }

        public void Recall()
        {
            Destroy(gameObject);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/MC/Special Features/BrushPlat_CancelFULL");
        }
    }
}