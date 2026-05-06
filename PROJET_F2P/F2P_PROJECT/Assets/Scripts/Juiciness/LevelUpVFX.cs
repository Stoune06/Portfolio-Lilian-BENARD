using DG.Tweening;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Juiciness
{
    public class LevelUpVFX : MonoBehaviour
    {
        [SerializeField] private float _Duration = 0.8f;
        [SerializeField] private float _TargetScale = 5f;
        [SerializeField] private Ease _ScaleEase = Ease.OutQuad;

        private Material _Material;

        private void Start()
        {
            _Material = GetComponent<Renderer>().material;

            transform.localScale = Vector3.one * 0.5f;

            transform.DOScale(Vector3.one * _TargetScale, _Duration)
                .SetEase(_ScaleEase);

            DOTween.To(() => 1f, x => _Material.SetFloat("_Alpha", x), 0f, _Duration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            transform.DOKill();
            if (_Material != null) Destroy(_Material);
        }
    }
}
