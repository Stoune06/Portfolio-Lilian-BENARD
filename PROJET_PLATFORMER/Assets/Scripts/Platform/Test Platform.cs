using Platformer.Player;
using System;
using Tooling;
using UnityEngine;

[Obsolete]
public class TestPlatform : MonoBehaviour
{
    public Vector3 velocity = Vector3.up;
    Collider2D _Collider;
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        _Collider = GetComponent<Collider2D>();
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.bounds.max.y > _Collider.bounds.min.y)
    //        _Collider.isTrigger = false;
    //}
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.collider.bounds.max.y > _Collider.bounds.min.y)
    //        _Collider.isTrigger = true;
    //}
}
