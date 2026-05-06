using System;
using Com.IsartDigital.HealerSurvivor.Manager;
using Com.IsartDigital.HealerSurvivor.Menus;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.Gameplay
{
    public class XP : MonoBehaviour
    { 
        public float _XpValue;
        [SerializeField]
        private float _Speed = 5f;
        
        private Player _PlayerInstance;
        private bool _IsInRange;

        private void Start()
        {
            _PlayerInstance = Player.instance;
            GameManager.Instance.onGameOver += StopUpdate;
        }

        private void Update()
        {
            if (!_PlayerInstance) 
                return;

            if (!(Vector3.Distance(_PlayerInstance.transform.position, transform.position) <= _PlayerInstance.GetStat(StatType.PickupRange))) 
                return;
            //Debug.Log("Xp in range");
            _IsInRange = true;
            transform.position += (Player.instance.transform.position - transform.position).normalized * (_Speed * Time.deltaTime);
            if (Vector3.Distance(_PlayerInstance.transform.position, transform.position) <= 0.1f)
                CollectXP();
            
        }

        private void StopUpdate(EMenuType pType)
        {
            gameObject.SetActive(false);
        }

        private void CollectXP()
        {
            _PlayerInstance.AddXp(_XpValue);
            Destroy(gameObject);
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.onGameOver -= StopUpdate;
        }
    }
}
