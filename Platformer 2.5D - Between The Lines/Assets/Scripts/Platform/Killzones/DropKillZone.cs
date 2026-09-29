using UnityEngine;

//Author: Sales Noé
namespace Platformer.Killzone
{
    public class DropKillZone : MobileKillzone
    {
        private new void Update()
        {
            base.Update();
            transform.position += _Direction.normalized * _Speed * Time.deltaTime;
        }

        protected override void OnCollision(Collider2D pCollider)
        {
            //se joue lorsque la goutte touche le sol
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/CORRUPTION/Mushrooms/Mushrooms_DropPoison", transform.position);

            //se joue en plus du son de goutte lorsque celle ci touche le joueur
            if (pCollider.CompareTag("CustomPlayer"))
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/CORRUPTION/Mushrooms/Mushrooms_Hit", transform.position);
            }
            Destroy(gameObject);
        }
    }
}