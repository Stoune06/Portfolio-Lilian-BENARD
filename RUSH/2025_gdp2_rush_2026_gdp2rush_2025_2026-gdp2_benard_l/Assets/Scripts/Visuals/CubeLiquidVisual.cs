using UnityEngine;

public class CubeLiquidVisual : MonoBehaviour
{
    [Header("Liquid Settings")]
    [SerializeField] private float _MaxWobble = 0.03f;
    [SerializeField] private float _WobbleSpeed = 1f;
    [SerializeField] private float _Recovery = 1f;
    [SerializeField] private Renderer _Renderer;

    private static readonly int _WobbleXProp = Shader.PropertyToID("_WobbleX");
    private static readonly int _WobbleZProp = Shader.PropertyToID("_WobbleZ");
    private static readonly int _FillColorProp = Shader.PropertyToID("_BottomColor");
    private static readonly int _TopColorProp = Shader.PropertyToID("_FoamColor");

    private Vector3 _LastPos;
    private Vector3 _Velocity;
    private Vector3 _LastRot;
    private Vector3 _AngularVelocity;

    private float _WobbleAmountX;
    private float _WobbleAmountZ;
    private float _WobbleAmountToAddX;
    private float _WobbleAmountToAddZ;
    private float _Pulse;
    private float _Time = 0.5f;
    private float _ElapsedTime = 0;

    private void Start()
    {
        _LastPos = transform.position;
        _LastRot = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        float lDeltaTime = Time.deltaTime;
        if (lDeltaTime == 0) return; // Sécurité si le temps est arrêté

        _ElapsedTime += lDeltaTime;

        // Calcul de la vitesse
        _Velocity = (_LastPos - transform.position) / lDeltaTime;

        // Correction pour la rotation (évite le bug du 360 -> 0)
        Vector3 currentRot = transform.rotation.eulerAngles;
        _AngularVelocity = new Vector3(
            Mathf.DeltaAngle(_LastRot.x, currentRot.x),
            Mathf.DeltaAngle(_LastRot.y, currentRot.y),
            Mathf.DeltaAngle(_LastRot.z, currentRot.z)
        );
        float frameCorrection = lDeltaTime / 0.0166f;
        _WobbleAmountToAddX += Mathf.Clamp((_Velocity.x + (_AngularVelocity.z * 0.2f)) * _MaxWobble, -_MaxWobble, _MaxWobble) * frameCorrection;
        _WobbleAmountToAddZ += Mathf.Clamp((_Velocity.z + (_AngularVelocity.x * 0.2f)) * _MaxWobble, -_MaxWobble, _MaxWobble) * frameCorrection;
        // ---------------------

        _LastPos = transform.position;
        _LastRot = currentRot;

        _WobbleAmountToAddX = Mathf.Lerp(_WobbleAmountToAddX, 0, lDeltaTime * _Recovery);
        _WobbleAmountToAddZ = Mathf.Lerp(_WobbleAmountToAddZ, 0, lDeltaTime * _Recovery);

        _Pulse = 2 * Mathf.PI * _WobbleSpeed;

        // Calcul final
        float finalWobbleX = _WobbleAmountZ + (_WobbleAmountToAddZ * Mathf.Sin(_Pulse * _ElapsedTime));
        float finalWobbleZ = _WobbleAmountX + (_WobbleAmountToAddX * Mathf.Sin(_Pulse * _ElapsedTime));

        // Lissage du résultat visuel
        _WobbleAmountX = Mathf.Lerp(_WobbleAmountX, finalWobbleX, lDeltaTime * _Recovery * 5f);
        _WobbleAmountZ = Mathf.Lerp(_WobbleAmountZ, finalWobbleZ, lDeltaTime * _Recovery * 5f);

        // Envoi au shader
        if (_Renderer != null)
        {
            _Renderer.material.SetFloat("_WobbleX", _WobbleAmountX);
            _Renderer.material.SetFloat("_WobbleZ", _WobbleAmountZ);
        }
    }

    public void SetLiquidColor(Color pColor)
    {
        _Renderer.material.SetColor(_FillColorProp, pColor);

        Color lLighterColor = Color.Lerp(pColor, Color.white, 0.4f);
        _Renderer.material.SetColor(_TopColorProp, lLighterColor);
    }

    public void SetLiquidGradient(Color topColor, Color bottomColor)
    {
        Debug.Log("Set Gradient");
        if (_Renderer == null) _Renderer = GetComponent<Renderer>();
        SetLiquidColor(bottomColor);
        _Renderer.material.SetColor("_TopColor", topColor);
    }
}