using System;
using UnityEngine;

public static class DragService
{
    public static event Action<int> onDragStarted;
    public static event Action onDragEnded;

    public static void StartDrag(int pIndex)
    {
        onDragStarted?.Invoke(pIndex);
    }

    public static void EndDrag()
    {
        onDragEnded?.Invoke();
    }
}

