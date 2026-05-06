using UnityEngine;

namespace Tooling
{
    public struct MyMath
    {
        public static Vector3Int VectorToInt(Vector3 pVector)
            => new Vector3Int(Mathf.RoundToInt(pVector.x), Mathf.RoundToInt(pVector.y), Mathf.RoundToInt(pVector.z));
        public static Vector2Int VectorToInt(Vector2 pVector)
            => new Vector2Int(Mathf.RoundToInt(pVector.x), Mathf.RoundToInt(pVector.y));
    }
}
