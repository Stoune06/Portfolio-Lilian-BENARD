using Com.IsartDigital.HealerSurvivor.Manager;
using Com.IsartDigital.HealerSurvivor.SO;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 13/04/2026 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.UI
{
    
    public class DeckDisplay : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Transform _ActiveDeckParent;
        [SerializeField] private Transform _CollectionDeckParent;
        [SerializeField] private Transform _DescriptionContainer;
        [SerializeField] private GameObject _CardPrefab;
        [SerializeField] private AudioClip _SoundMoveCard;

        [SerializeField] private Vector2 _CellSize = new Vector2(150f, 200f);
        [SerializeField] private Vector2 _Spacing = new Vector2(100f, 100f);
        
        private readonly Dictionary<CardCharacterSO, RectTransform> _CardVisuals = new Dictionary<CardCharacterSO, RectTransform>();

        private const int MAX_NUMBER_CARD = 4;
        private const int RANDOMNESS_SHAKE = 15;
        private const int GRID_SIZE = 4;

        private const float TWEEN_DURATION = .4f;

        private DeckBuilderManager _DeckBuilderManager => DeckBuilderManager.Instance;
        private GameManager _GameManager => GameManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start() => RefreshDisplay();

        private void RefreshDisplay()
        {
            ResetParents(_ActiveDeckParent, _CollectionDeckParent);
            
            List<CardCharacterSO> lSortedCards = new List<CardCharacterSO>(_DeckBuilderManager.allCards);
            
            lSortedCards.Sort(SortCardOnRarity);

            for (int i = 0; i < _DeckBuilderManager.activeCurrentCards.Count; i++)
            {
                CardCharacterSO lCard = _DeckBuilderManager.activeCurrentCards[i];
                RectTransform lRT = InstantiateCard(lCard, _ActiveDeckParent);
                lRT.localPosition = GetPositionFromIndex(i);
            }

            int lColIndex = 0;
            foreach (CardCharacterSO lCard in lSortedCards)
            {
                if (_DeckBuilderManager.activeCurrentCards.Contains(lCard))
                    continue;
                
                RectTransform lRT = InstantiateCard(lCard, _CollectionDeckParent);
                lRT.localPosition = GetPositionFromIndex(lColIndex);
                lColIndex++;
            }
        }

        private RectTransform InstantiateCard(CardCharacterSO pData, Transform pParent)
        {
            GameObject lGo = Instantiate(_CardPrefab, pParent);
            CardDisplay lDisplay = lGo.GetComponent<CardDisplay>();
            RectTransform lRect = lGo.GetComponent<RectTransform>();

            lDisplay.SetCard(pData, _DescriptionContainer);

            _CardVisuals[pData] = lRect;

            lGo.GetComponentInChildren<Button>().onClick.AddListener(() => ManageDeck(pData, lRect));

            return lRect;
        }

        private void ManageDeck(CardCharacterSO pData, RectTransform pCardVisual)
        {
            bool lCurrentlyInDeck = _DeckBuilderManager.activeCurrentCards.Contains(pData); 
            Transform lTargetParent = lCurrentlyInDeck ? _CollectionDeckParent : _ActiveDeckParent;

            switch (lCurrentlyInDeck)
            {
                case false when _DeckBuilderManager.activeCurrentCards.Count >= MAX_NUMBER_CARD:
                    pCardVisual.DOShakeAnchorPos(TWEEN_DURATION, RANDOMNESS_SHAKE);
                    return;
                case true:
                    _DeckBuilderManager.RemoveCardFromDeck(pData);
                    SoundManager.Instance.PlaySound(_SoundMoveCard, transform.position);
                    break;
                default:
                    _DeckBuilderManager.AddCardToDeck(pData);
                    SoundManager.Instance.PlaySound(_SoundMoveCard, transform.position);
                    break;
            }
            
            _GameManager.onSwitchCardFromDeck?.Invoke();
            AnimateCardSwitch(pCardVisual, lTargetParent, !lCurrentlyInDeck);
            UpdateAllPositions();
        }

        private void UpdateAllPositions()
        {
            for (int i = 0; i < _DeckBuilderManager.activeCurrentCards.Count; i++)
            {
                CardCharacterSO lData = _DeckBuilderManager.activeCurrentCards[i];
                AnimateToCorrectSlot(_ActiveDeckParent, lData, i);
            }

            List<CardCharacterSO> lCollection = new List<CardCharacterSO>();
            foreach (CardCharacterSO lCard in _DeckBuilderManager.allCards)
                if (!_DeckBuilderManager.activeCurrentCards.Contains(lCard)) 
                    lCollection.Add(lCard);

            lCollection.Sort(SortCardOnRarity);
            
            for (int i = 0; i < lCollection.Count; i++)
                AnimateToCorrectSlot(_CollectionDeckParent, lCollection[i], i);
            
        }

        private void AnimateToCorrectSlot(Transform pParent, CardCharacterSO pData, int pIndex)
        {
            if (!_CardVisuals.TryGetValue(pData, out RectTransform lRT))
                return;
            
            lRT.DOKill();
            lRT.SetParent(pParent, true);
            Vector3 lTargetPos = GetPositionFromIndex(pIndex);
        
            lRT.DOLocalMove(lTargetPos, TWEEN_DURATION).SetEase(Ease.OutQuint);
        }

        private void ResetParents(params Transform[] pParents)
        {
            _CardVisuals.Clear();

            foreach (Transform lParent in pParents)
            {
                foreach (Transform lChild in lParent)
                    Destroy(lChild.gameObject);
            }
        }

        private void AnimateCardSwitch(RectTransform pCard, Transform pTargetParent, bool pMovingToDeck)
        {
            pCard.DOKill();
            int lTargetIndex;

            if (pMovingToDeck)
                lTargetIndex = _DeckBuilderManager.activeCurrentCards.Count - 1;
            else
            {
                List<CardCharacterSO> lCollection = new List<CardCharacterSO>();

                foreach (CardCharacterSO lCard in _DeckBuilderManager.allCards)
                    if (!_DeckBuilderManager.activeCurrentCards.Contains(lCard))
                        lCollection.Add(lCard);
                
                lTargetIndex = lCollection.IndexOf(pCard.GetComponent<CardDisplay>().GetCardData());
            }

            if (lTargetIndex < 0) 
                lTargetIndex = 0;

            Vector3 lTargetLocalPos = GetPositionFromIndex(lTargetIndex);

            pCard.SetParent(pTargetParent, true);
            pCard.DOLocalMove(lTargetLocalPos, TWEEN_DURATION).SetEase(Ease.OutQuint);
        }

        private Vector3 GetPositionFromIndex(int pIndex)
        {
            int lCol = pIndex % GRID_SIZE;
            int lRow = pIndex / GRID_SIZE;

            float lPosX = lCol * (_CellSize.x + _Spacing.x);
            float lPosY = lRow * (_CellSize.y + _Spacing.y);

            return new Vector3(lPosX, -lPosY, 0);
        }

        private int SortCardOnRarity(CardCharacterSO pFirstCard, CardCharacterSO pSecondCard)
        {
            int lSort = ((int)pFirstCard.Rarity).CompareTo((int)pSecondCard.Rarity);

            if (lSort == 0)
                lSort = pFirstCard.name.CompareTo(pSecondCard.name);

            return lSort;
        }
    }
}