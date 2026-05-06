using System;
using Platformer.Areas;
using UnityEngine;

namespace Platformer
{
    public class Collectible : Area_System
    {
        
        [SerializeField] private bool _IsBig = false;
        [SerializeField] private GameObject _CollectVFX;

        protected override void OnCollision()
        {
            OnCollect();
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/INTERRACTIONS/Collectible_Rare_Obtained");
        }
        
        //What happens on collecting
        private void OnCollect()
        {
            HUD.Instance.OnCollected(_IsBig);
            Instantiate(_CollectVFX,transform.position,Quaternion.identity);
            Destroy(gameObject);
        }
    }
}