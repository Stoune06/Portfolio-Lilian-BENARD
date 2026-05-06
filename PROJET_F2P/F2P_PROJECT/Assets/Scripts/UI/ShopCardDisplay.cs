using Com.IsartDigital.HealerSurvivor.Enum;
using Com.IsartDigital.HealerSurvivor.Manager;
using Com.IsartDigital.HealerSurvivor.Other;
using Com.IsartDigital.HealerSurvivor.SO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.UI
{
    
    public class ShopCardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private TextMeshProUGUI _NameText;
        [SerializeField] private TextMeshProUGUI _PriceText;
        [SerializeField] private Image _Icon;
        [SerializeField] private Image _ShopBackground;
        [SerializeField] private Button _BuyButton;
        [SerializeField] private float _ScaleMultiplier = 1.2f;
        [SerializeField] private float _Duration = .5f;
        
        private Vector3 _OriginScale;
        
        private CardCharacterSO _Data;
        
        private ERarity _Rarity;
        
        private Color  _CardColorRarity;
        
        private Tween _CurrentTween;
        
        private DeckBuilderManager _DeckBuilderManager => DeckBuilderManager.Instance;
        private CurrencyManager _CurrencyManager => CurrencyManager.Instance;
        
        private void Awake() => _OriginScale = transform.localScale;
        
        public void Init(CardCharacterSO pData)
        {
            _Data = pData;
            _NameText.text = pData.CardName;
            _PriceText.text = pData.Price + " <sprite name=\"Gemstone\">";;
            _Icon.sprite = pData.CardIcon;
            _Rarity = pData.Rarity;
            
            _CardColorRarity = RaritySettings.GetColor(_Rarity);
            _ShopBackground.color = _CardColorRarity;

            _BuyButton.onClick.AddListener(BuyCard);
        }

        private void BuyCard()
        {
            if (_CurrencyManager.TrySpend(_Data.Price, ECurrencyType.GEMS))
            {
                _DeckBuilderManager.allCards.Add(_Data);
                _BuyButton.interactable = false;
                _PriceText.text = "SOLD OUT";
            }
            else
            {
                _PriceText.transform.DOShakePosition(.3f, new Vector3(5, 0, 0), 10, 0);
                _PriceText.DOColor(Color.red, .1f).OnComplete(() => _PriceText.DOColor(Color.white, .2f));
                Debug.Log("NOT ENOUGH AMOUNT!");
            }
        }

        private void OnEnable()
        {
            if (_BuyButton == null)
                return;

            _CurrentTween?.Kill();
            transform.localScale = _OriginScale;
        }

        private void OnDisable()
        {
            _CurrentTween?.Kill();
            transform.localScale = _OriginScale;
        }
        
        private void OnDestroy() => _BuyButton.onClick.RemoveAllListeners();
        
        public void OnPointerEnter(PointerEventData pEventData)
        {
            _CurrentTween?.Kill();

            _CurrentTween = transform.DOScale(_OriginScale * _ScaleMultiplier, _Duration)
                .SetEase(Ease.OutElastic);
        }

        public void OnPointerExit(PointerEventData pEventData)
        {
            _CurrentTween?.Kill();

            _CurrentTween = transform.DOScale(_OriginScale, _Duration)
                .SetEase(Ease.OutElastic);
        }
    }
}