using UnityEngine;

namespace Tooling
{
    public class ShowPreview : PropertyAttribute
    {
        public float size;

        public ShowPreview(float pSize = 20f)
        {
            size = pSize;
        }
    }
}