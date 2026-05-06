using UnityEngine;

public class LoadPrefab : MonoBehaviour
{
    [SerializeField] private GameObject _Prefab;

    public void Load()
    {
        Instantiate(_Prefab);
    }
}
