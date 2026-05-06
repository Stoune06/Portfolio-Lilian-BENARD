using UnityEngine;

public class ClosePrefab : MonoBehaviour
{
    public void OnLeave()
    {
        Destroy(gameObject);
    }
}
