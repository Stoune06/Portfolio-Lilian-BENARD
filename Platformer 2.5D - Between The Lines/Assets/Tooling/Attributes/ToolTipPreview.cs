using System;
using UnityEngine;

namespace Tooling
{
    /// <summary>
    /// <see cref="TooltipPreview"></see> is an <see cref="Attribute"></see> that'll display a preview of your <see cref="UnityEngine.Object"></see>
    /// if the mouse is hovering your <see langword="property"/> or <see langword="field"/> in the <c>Inspector</c>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class TooltipPreview : PropertyAttribute
    {
        public float size;

        public TooltipPreview(float pSize = 50)
        {
            size = Mathf.Clamp(pSize, 50f, 900f);
        }
    }
}