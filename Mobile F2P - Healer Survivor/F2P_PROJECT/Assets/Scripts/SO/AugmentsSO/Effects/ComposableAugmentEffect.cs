using System;
using System.Collections.Generic;
using Com.IsartDigital.HealerSurvivor.SO.Actions;
using Com.IsartDigital.HealerSurvivor.SO.Triggers;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO
{
    [Serializable]
    public class ComposableAugmentEffect : AugmentEffect
    {
        [SerializeReference]
        private AugmentTrigger _Trigger;
        [SerializeReference]
        private List<AugmentAction> _Actions = new List<AugmentAction>();
        [SerializeField]
        private float _Cooldown;

        private IAugmentable _Player;
        private float _LastProcTime;
        private bool _IsProcessing;

        public override void Apply(IAugmentable pPlayer)
        {
            _Player = pPlayer;
            _LastProcTime = -_Cooldown;
            if (_Trigger != null)
            {
                _Trigger.Init(pPlayer, OnTriggered);
                _Trigger.Subscribe();
            }
            foreach (AugmentAction lAction in _Actions)
                lAction.OnApply(pPlayer);
        }

        public override void Remove(IAugmentable pPlayer)
        {
            if (_Trigger != null) _Trigger.Unsubscribe();
            foreach (AugmentAction lAction in _Actions)
                lAction.OnRemove(pPlayer);
            _Player = null;
        }

        private void OnTriggered()
        {
            if (_IsProcessing) return;
            if (_Player == null) return;
            if (_Cooldown > 0f && Time.time - _LastProcTime < _Cooldown) return;
            if (!AreConditionsMet(_Player)) return;
            _LastProcTime = Time.time;
            _IsProcessing = true;
            try
            {
                foreach (AugmentAction lAction in _Actions)
                    lAction.Execute(_Player, _Trigger);
            }
            finally
            {
                _IsProcessing = false;
            }
        }

        public override string GetDescription()
        {
            string lDescription = "";
            for (int i = 0; i < _Actions.Count; i++)
            {
                if (i > 0) lDescription += ", ";
                lDescription += _Actions[i].GetDescription();
            }
            return lDescription;
        }
    }
}
