using Platformer.Player;
using UnityEngine;

//Author : Lilian Benard
namespace Platformer.Killzone
{
    public class Killzone : MonoBehaviour
    {
        [SerializeField]
        protected string _PlayerTag = "CustomPlayer";
        [SerializeField] private bool _DestroyOnCollid = false;

        protected void OnTriggerEnter2D(Collider2D pCollision)
        {
            if (pCollision.gameObject.CompareTag(_PlayerTag) && pCollision.TryGetComponent(out Player.Player lPlayer))
            {
                lPlayer.Die();
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/CORRUPTION/Mushrooms/Mushrooms_Hit");
            }
            if(_DestroyOnCollid) Destroy(gameObject);
        }
    }
}

