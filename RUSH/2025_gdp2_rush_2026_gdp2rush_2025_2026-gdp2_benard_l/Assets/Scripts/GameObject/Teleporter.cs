using UnityEngine;

public class Teleporter : Tile
{
    [Header("Liaison")]

    [SerializeField]
    public Teleporter _Twin;

    public bool isTheTarget = false;

    [Header("Apparence")]
    [Tooltip("La couleur à appliquer au Shader (Spirale)")]
    [SerializeField]
    private Color _Color = Color.white;

    [Tooltip("Le renderer qui contient le matériau")]
    [SerializeField]
    private Renderer _Renderer;

    [Tooltip("Le nom de la propriété dans le Shader Graph (ex: _Tint ou _Color)")]
    [SerializeField]
    private string _ColorPropertyName = "_BaseColor";
    private MaterialPropertyBlock _PropBlock;

    protected override void Start()
    {
        stateToGive = new TeleportState(this, _Twin);
        UpdateColor();
    }
    private void OnValidate()
    {
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (_Renderer == null) _Renderer = GetComponentInChildren<Renderer>();
        if (_Renderer == null) return;
        if (_PropBlock == null) _PropBlock = new MaterialPropertyBlock();

        _Renderer.GetPropertyBlock(_PropBlock);
        _PropBlock.SetColor(_ColorPropertyName, _Color);
        _Renderer.SetPropertyBlock(_PropBlock);
    }
}