using UnityEngine;
using System.Collections.Generic;

public static class SessionManager
{
    // Cette liste garde en mémoire les noms des niveaux finis PENDANT cette session.
    private static HashSet<string> _CompletedLevels = new HashSet<string>();

    public static void MarkLevelComplete(string levelName)
    {
        if (!_CompletedLevels.Contains(levelName))
        {
            _CompletedLevels.Add(levelName);
        }
    }

    public static bool IsLevelCompleted(string levelName)
    {
        return _CompletedLevels.Contains(levelName);
    }
}