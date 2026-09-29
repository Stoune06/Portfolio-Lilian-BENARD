using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Other;
using UnityEngine;
using UnityEngine.Serialization;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.ProjectName
{
    
    public class PlayerMagnet : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private SphereCollider _MagnetCollider;

        private void OnTriggerEnter(Collider pOther) 
        {
            if (pOther.TryGetComponent(out Coin lCoin)) 
                lCoin.StartAttraction(transform.parent); 
            //
        }
    }
}