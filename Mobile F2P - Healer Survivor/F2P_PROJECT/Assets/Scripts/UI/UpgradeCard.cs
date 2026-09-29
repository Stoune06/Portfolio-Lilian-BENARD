using System;
using Com.IsartDigital.HealerSurvivor.Manager;
using Com.IsartDigital.HealerSurvivor.SO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
namespace Com.IsartDigital.HealerSurvivor.Menus
{
    public class UpgradeCard : MonoBehaviour
    {
        [SerializeField]
        private Text _Description;
        [SerializeField]
        private Text _UpgradeName;
        [SerializeField]
        private Image _Image;
        [SerializeField]
        private Button _UpgradeButton;
        
        [SerializeField] private AudioClip _SwooshSound;

        private const float ENTRANCE_DURATION = 0.4f;
        private const float STAGGER_DELAY = 0.1f;
        private const float EXIT_DURATION = 0.25f;

        public static Action<AugmentSO> onUpgradeSelected;
        public static Action onCardsAnimatedOut;
        private AugmentSO _Augment;
        private int _CardIndex;
        private bool _IsAnimatingOut;

        public void SetCardIndex(int pIndex) => _CardIndex = pIndex;

        private void Start()
        {
            SoundManager.Instance.PlaySound(_SwooshSound, transform.position);
            _UpgradeButton.onClick.AddListener(OnButtonClick);

            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, ENTRANCE_DURATION)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .SetDelay(_CardIndex * STAGGER_DELAY);
        }

        public void UpdateCard(AugmentSO pAugment)
        {
            _Description.text =  pAugment.description;
            _UpgradeName.text =  pAugment.upgradeName;
            _Image.sprite = pAugment.icon;
            _Augment = pAugment;
        }

        private void OnButtonClick()
        {
            SoundManager.Instance.PlaySound(_SwooshSound, transform.position);
            _UpgradeButton.interactable = false;
            onUpgradeSelected?.Invoke(_Augment);

            foreach (Transform lChild in transform.parent)
            {
                if (lChild == transform) continue;
                UpgradeCard lCard = lChild.GetComponent<UpgradeCard>();
                if (lCard != null) lCard.AnimateOut();
            }

            _IsAnimatingOut = true;
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    transform.DOScale(Vector3.zero, EXIT_DURATION)
                        .SetEase(Ease.InBack)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            onCardsAnimatedOut?.Invoke();
                            Destroy(gameObject);
                        });
                });
        }

        public void AnimateOut()
        {
            if (_IsAnimatingOut) return;
            _IsAnimatingOut = true;
            _UpgradeButton.interactable = false;
            transform.DOScale(Vector3.zero, EXIT_DURATION)
                .SetEase(Ease.InBack)
                .SetUpdate(true)
                .OnComplete(() => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            transform.DOKill();
            _UpgradeButton.onClick.RemoveAllListeners();
        }
    }
}
