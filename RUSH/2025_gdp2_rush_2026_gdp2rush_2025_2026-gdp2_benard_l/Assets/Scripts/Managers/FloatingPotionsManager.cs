using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class FloatingPotionsManager : MonoBehaviour
{
    [Header("Réglages")]
    [SerializeField] private GameObject _PotionPrefab;
    [SerializeField] private Transform _AnchorsContainer;

    // Appelé quand on revient au menu
    public void RefreshPotions()
    {
        if (_AnchorsContainer != null) _AnchorsContainer.gameObject.SetActive(true);
        gameObject.SetActive(true);
        if (BookLevelSelector.instance == null) return;
        LevelData[] allLevels = BookLevelSelector.instance.levels;
        for (int i = 0; i < allLevels.Length; i++)
        {
            if (i >= _AnchorsContainer.childCount) break;

            LevelData level = allLevels[i];
            Transform anchor = _AnchorsContainer.GetChild(i);

            if (SessionManager.IsLevelCompleted(level.name) && anchor.childCount == 0)
            {
                SpawnFloatingPotion(anchor, level);
            }
        }
    }

    private void SpawnFloatingPotion(Transform anchor, LevelData level)
    {
        GameObject newPotion = Instantiate(_PotionPrefab, anchor);
        newPotion.transform.localPosition = Vector3.zero;
        newPotion.transform.localRotation = Quaternion.identity;

        // 1. Lévitation (inchangé)
        if (newPotion.GetComponent<LevitationEffect>() == null)
            newPotion.AddComponent<LevitationEffect>();

        // 2. Nettoyage (inchangé)
        PotionFillController fillCtrl = newPotion.GetComponent<PotionFillController>();
        if (fillCtrl != null) Destroy(fillCtrl);

        // --- 3. NOUVEAU : Application du dégradé ---
        CubeLiquidVisual visual = newPotion.GetComponent<CubeLiquidVisual>();
        if (visual != null)
        {
            // On envoie les couleurs définies dans le LevelData vers le visuel
            visual.SetLiquidGradient(level.PotionColorTop, level.PotionColorBottom);
        }
        // -------------------------------------------
    }


}