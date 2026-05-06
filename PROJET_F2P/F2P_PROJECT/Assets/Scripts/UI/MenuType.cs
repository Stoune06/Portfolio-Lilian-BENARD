using UnityEngine;
using UnityEngine.Serialization;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.HealerSurvivor.Menus
{

    public class MenuType : MonoBehaviour
    {
        [FormerlySerializedAs("Type")]
        public EMenuType type;
    }
}