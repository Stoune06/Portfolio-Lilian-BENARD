using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.Juiciness
{
    public class LevelUpEffect : MonoBehaviour
    {
        [SerializeField] private GameObject _LevelUpRingPrefab;

        private Player _Player;

        private void Start()
        {
            _Player = GetComponent<Player>();
            _Player.OnLevelUp += SpawnRing;
        }

        private void SpawnRing()
        {
            Instantiate(_LevelUpRingPrefab, transform.position, Quaternion.identity);
        }

        private void OnDestroy()
        {
            _Player.OnLevelUp -= SpawnRing;
        }
    }
}
