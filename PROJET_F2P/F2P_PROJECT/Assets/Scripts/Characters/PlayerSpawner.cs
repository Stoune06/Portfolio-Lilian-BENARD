using System.Collections;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.Gameplay;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Other
{
    
    public class PlayerSpawner : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [FormerlySerializedAs("playerPrefab")]
        [SerializeField] private GameObject _PlayerPrefab;

        public PlayerCamera playerCamera;

        public void SpawnPlayer()
        {
            if (Player.instance != null) 
                return;

            GameObject lPlayer = Instantiate(_PlayerPrefab, transform.position, Quaternion.identity);
            playerCamera.SetTarget(lPlayer.transform);
        }
    }
}