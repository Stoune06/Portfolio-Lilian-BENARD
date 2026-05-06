using System;
using UnityEngine;
namespace Platformer
{
    public class CancelDrawInput : MonoBehaviour
    {
        public static event Action<bool> OnCancelDrawInput;

        public void InvokeOnCancelDrawInput(bool pIsAllowedToDraw)
        {
            OnCancelDrawInput?.Invoke(pIsAllowedToDraw);
        }
    }
}
