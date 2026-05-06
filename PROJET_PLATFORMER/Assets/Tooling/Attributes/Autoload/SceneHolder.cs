using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

//Author : MERFOUD Kelyan

public class SceneHolder : ScriptableObject
{
    public string path;
    public string sceneName = "default";
    public LoadSceneMode LoadSceneMode = 0;

    public SceneHolder(string pPath) => path = pPath;
    public SceneHolder(string pScenePath, LoadSceneMode pLoadSceneMode = 0)
    {
        path = pScenePath;
        sceneName = System.IO.Path.GetFileNameWithoutExtension(pScenePath);
        LoadSceneMode = pLoadSceneMode;
    }

    public SceneHolder(string pScenePath, string pObjectCreationPath, LoadSceneMode pLoadSceneMode = 0)
    {
        path = pScenePath;
        LoadSceneMode = pLoadSceneMode;
        sceneName = System.IO.Path.GetFileNameWithoutExtension(pScenePath);
    }

    public static void Instantiate(SceneHolder pHolder, LoadSceneMode pMode = LoadSceneMode.Single)
        => SceneManager.LoadScene(pHolder.path, pMode);

    public void Instantiate() => SceneManager.LoadScene(path, LoadSceneMode);
    public void Instantiate(LoadSceneMode pLoadSceneMode) => SceneManager.LoadScene(path, pLoadSceneMode);
    public override string ToString() => $"{path}, {LoadSceneMode}";
}
