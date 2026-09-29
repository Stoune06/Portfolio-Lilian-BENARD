using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tooling
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ReadOnly : PropertyAttribute
    {

    }
}