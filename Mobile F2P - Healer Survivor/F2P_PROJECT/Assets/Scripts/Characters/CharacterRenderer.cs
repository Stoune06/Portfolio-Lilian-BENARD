using UnityEngine;

[RequireComponent (typeof(Renderer))]
public class CharacterRenderer : MonoBehaviour
{
    public Renderer charaRenderer;

    private void Awake()
    {
        charaRenderer = GetComponent<Renderer>();

    }
}
