using UnityEngine;

public class LevelPreviewSetup : MonoBehaviour
{
    public Camera previewCamera;

    [Header("Qualité")]
    public float resolutionDensity = 512f;

    [HideInInspector] public RenderTexture previewTexture;
    public void GeneratePreview(Renderer pageSurface)
    {
        if (previewCamera == null) previewCamera = GetComponentInChildren<Camera>();
        if (previewCamera == null)
        {
            Debug.LogError($"[LevelPreviewSetup] Pas de caméra sur {gameObject.name}");
            return;
        }
        Vector3 surfaceSize = pageSurface.bounds.size;
        float worldWidth = surfaceSize.x;
        float worldHeight = Mathf.Max(surfaceSize.y, surfaceSize.z);

        int pixelWidth = Mathf.RoundToInt(worldWidth * resolutionDensity);
        int pixelHeight = Mathf.RoundToInt(worldHeight * resolutionDensity);

        if (pixelWidth < 64) pixelWidth = 64;
        if (pixelHeight < 64) pixelHeight = 64;

        if (previewTexture != null) previewTexture.Release();

        previewTexture = new RenderTexture(pixelWidth, pixelHeight, 24, RenderTextureFormat.ARGB32);
        previewTexture.name = $"{gameObject.name}_{pixelWidth}x{pixelHeight}";
        previewTexture.antiAliasing = 2;

        previewCamera.targetTexture = previewTexture;

        previewCamera.aspect = (float)pixelWidth / (float)pixelHeight;

        AudioListener listener = previewCamera.GetComponent<AudioListener>();
        if (listener != null) Destroy(listener);

        previewCamera.enabled = true;
    }

    void OnDestroy()
    {
        if (previewTexture != null) previewTexture.Release();
    }
}