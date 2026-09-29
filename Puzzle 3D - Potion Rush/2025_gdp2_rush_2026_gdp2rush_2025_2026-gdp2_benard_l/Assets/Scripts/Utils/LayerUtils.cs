using UnityEngine;

public static class LayerUtils
{
    public static void SetLayerRecursively(GameObject pObj, int pNewLayer)
    {
        pObj.layer = pNewLayer;
        foreach (Transform lChild in pObj.transform)
        {
            SetLayerRecursively(lChild.gameObject, pNewLayer);
        }
    }
}

