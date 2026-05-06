using System.Collections.Generic;
using UnityEngine;
namespace Com.IsartDigital.HealerSurvivor.SO
{
    [CreateAssetMenu(fileName = "AugmentSO", menuName = "Scriptable Objects/AugmentSO", order = 0)]
    public class AugmentSO : ScriptableObject
    {
        [SerializeReference]
        public List<AugmentEffect> effects = new List<AugmentEffect>();
        
        public string description;
        public string upgradeName;
        public Sprite icon;
    }
}
