using UnityEngine;

public class Wobble : MonoBehaviour
{
    Renderer rend;
    Vector3 lastPos;
    Vector3 lastRot;

    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;

    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        // Initialisation pour éviter un gros "splash" au démarrage
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime == 0) return;

        // 1. Vitesse d'Inertie (World Space)
        // On calcule (Last - Current) pour avoir l'effet de "traînée" du liquide
        Vector3 worldVelocity = (lastPos - transform.position) / deltaTime;

        // 2. Vitesse Angulaire (Correction du bug 360°)
        Vector3 currentRot = transform.rotation.eulerAngles;
        Vector3 angularVelocity = new Vector3(
            Mathf.DeltaAngle(lastRot.x, currentRot.x),
            Mathf.DeltaAngle(lastRot.y, currentRot.y),
            Mathf.DeltaAngle(lastRot.z, currentRot.z)
        ) / deltaTime;

        // 3. Conversion en Local (C'est la correction clé !)
        // On convertit ces forces pour qu'elles s'appliquent sur les côtés du cube (X/Z) 
        // peu importe comment le cube est tourné dans la pièce.
        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);
        Vector3 localAngularVelocity = transform.InverseTransformDirection(angularVelocity);

        // 4. Calcul de la force de Wobble
        // La rotation sur Z fait bouger le liquide en X, et vice-versa.
        float wobbleXChange = (localVelocity.x + (localAngularVelocity.z * 0.2f)) * MaxWobble;
        float wobbleZChange = (localVelocity.z + (localAngularVelocity.x * 0.2f)) * MaxWobble;

        wobbleAmountToAddX += Mathf.Clamp(wobbleXChange, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp(wobbleZChange, -MaxWobble, MaxWobble);

        // Mise à jour des états passés
        lastPos = transform.position;
        lastRot = currentRot;

        // 5. Récupération (Amortissement)
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, deltaTime * Recovery);
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, deltaTime * Recovery);

        // 6. Application de l'onde (Sinus)
        pulse = 2 * Mathf.PI * WobbleSpeed;
        float wobbleX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        float wobbleZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        // Lissage final
        wobbleAmountX = Mathf.Lerp(wobbleAmountX, wobbleX, deltaTime * Recovery * 5f);
        wobbleAmountZ = Mathf.Lerp(wobbleAmountZ, wobbleZ, deltaTime * Recovery * 5f);

        // 7. Envoi au Shader
        if (rend != null)
        {
            rend.material.SetFloat("_WobbleX", wobbleAmountX);
            rend.material.SetFloat("_WobbleZ", wobbleAmountZ);
        }

        time += deltaTime;
    }
}