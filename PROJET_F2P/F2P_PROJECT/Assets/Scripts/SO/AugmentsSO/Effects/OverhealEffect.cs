using Com.IsartDigital.HealerSurvivor.Gameplay;
using UnityEngine;

namespace Com.IsartDigital.HealerSurvivor.SO
{
    [System.Serializable]
    public class OverhealEffect : AugmentEffect
    {
        [SerializeField]
        private float _BonusRadius;
        [SerializeField]
        private float _BonusMultiplier;
        [SerializeField]
        private LayerMask _AllyLayer;

        private static readonly Collider[] _OverlapBuffer = new Collider[32];
        private IAugmentable _Player;

        public override void Apply(IAugmentable pPlayer)
        {
            _Player = pPlayer;
            _Player.OnHealAlly += OnHealAlly;
        }

        public override void Remove(IAugmentable pPlayer)
        {
            pPlayer.OnHealAlly -= OnHealAlly;
            _Player = null;
        }

        private void OnHealAlly(float pAmount, Transform pTarget)
        {
            if (!AreConditionsMet(_Player)) return;
            Character lTarget = pTarget.GetComponent<Character>();
            if (lTarget == null) return;

            float lOverHealAmount = (lTarget.health + pAmount) - lTarget.targetMaxHealth;
            if (lOverHealAmount <= 0f) return;

            float lBonusHeal = lOverHealAmount * _BonusMultiplier;
            int lCount = Physics.OverlapSphereNonAlloc(pTarget.position, _BonusRadius, _OverlapBuffer, _AllyLayer);
            for (int i = 0; i < lCount; i++)
            {
                Character lCharacter = _OverlapBuffer[i].GetComponent<Character>();
                if (lCharacter != null && lCharacter != lTarget)
                    lCharacter.Heal(lBonusHeal);
            }
        }

        public override string GetDescription()
        {
            return $"Overheal spreads to allies within {_BonusRadius}m (x{_BonusMultiplier})";
        }
    }
}