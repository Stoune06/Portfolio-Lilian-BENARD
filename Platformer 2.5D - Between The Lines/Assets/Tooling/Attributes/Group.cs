#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Tooling
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false), Obsolete]
    public class Group : PropertyAttribute
    {
        public string name;
        public Group(string pName) => name = pName;
    }
}
#endif