using Com.IsartDigital.HealerSurvivor.Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    [SerializeField] private Image image; // Public camelCase
    [SerializeField] private Image[] images; // Public camelCase
    [SerializeField] private Vector2 maxDimensions; // Changé float -> Vector2 (x = rayon X, y = rayon Z)

    private Transform _Target; // Privée _PascalCase
    private Transform _Center;

    private Character _Chara;

    // Vérifie si la cible est en dehors de l'ellipse
    private bool IsTargetOutsideEllipse()
    {
        if (_Target == null || _Center == null) return false;

        // Équation de l'ellipse : (x²/a²) + (z²/b²) > 1
        float lDX = _Target.position.x - _Center.position.x;
        float lDZ = _Target.position.z - _Center.position.z;

        // On évite la division par zéro si les dimensions sont à 0
        float lValX = maxDimensions.x != 0 ? (lDX * lDX) / (maxDimensions.x * maxDimensions.x) : 0;
        float lValZ = maxDimensions.y != 0 ? (lDZ * lDZ) / (maxDimensions.y * maxDimensions.y) : 0;

        return (lValX + lValZ) > 1;
    }

    public void SetValue(Transform pTarget, Transform pCenter) // Paramètres pPascalCase
    {
        _Target = pTarget;
        _Center = pCenter;
        _Chara = _Target.GetComponent<Character>();
    }

    private void Update()
    {
        bool lIsOutside = IsTargetOutsideEllipse();

        foreach(Image lImage in images)
        {
            lImage.enabled = lIsOutside;
        }


        if (lIsOutside)
        {
            SetPositionAndRotationFromCenter();
        }

        image.fillAmount = (_Chara.baseStats.maxHealth - _Chara.health) / _Chara.baseStats.maxHealth;
    }

    private void SetPositionAndRotationFromCenter()
    {
        // 1. Calculer la direction sur le plan horizontal
        Vector3 lDirection = (_Target.position - _Center.position);
        lDirection.y = 0;
        lDirection.Normalize();

        // 2. Calculer le point sur le bord de l'ellipse
        // On projette la direction sur les axes de l'ellipse
        Vector3 lEllipseOffset = new Vector3(
            lDirection.x * maxDimensions.x,
            0f,
            lDirection.z * maxDimensions.y
        );

        // 3. Positionner l'indicateur
        transform.position = _Center.position + lEllipseOffset;

        // On s'assure que l'indicateur reste à la même hauteur que le centre
        transform.position = new Vector3(transform.position.x, _Center.position.y, transform.position.z);

        // 4. Rotation vers la cible (Z forward vers la target)
        Vector3 lLookDir = _Target.position - transform.position;
        lLookDir.y = 0;
        if (lLookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(-lLookDir, Vector3.up);
        }
    }
}