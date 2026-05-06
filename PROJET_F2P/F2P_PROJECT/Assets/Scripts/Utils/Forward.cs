using UnityEngine;

public class Forward : MonoBehaviour
{
    [SerializeField] private float _Speed = 5f;

    void Update()
    {
        transform.position += transform.forward * _Speed * Time.deltaTime;
    }
}
