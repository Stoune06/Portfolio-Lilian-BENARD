using UnityEngine;

public class MapColorParameter : ScriptableObject
{
    public Gradient influenceGradient = new Gradient();

    public float minValue = -1f;
    public float maxValue = 1f;
    public float baseAlpha = 0.4f;

    public bool showContour = true;
}
