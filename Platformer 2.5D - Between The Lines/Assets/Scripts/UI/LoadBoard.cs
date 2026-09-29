using UnityEngine;
using UnityEngine.UI;

public class LoadBoard : MonoBehaviour
{
    [SerializeField] private Text _TextUsername;
    [SerializeField] private Text _TextCollectibles;
    [SerializeField] private Text _TextLevel;
    [SerializeField] private Text _TextCheckpoint;
    public void Load(string pName, string pCollect, string pLevel, string pCheck)
    {
        if (_TextCheckpoint == null || _TextCollectibles == null || _TextLevel == null)
        {
            Debug.LogError("Failed to initialize Board");
            return;
        }
        _TextUsername.text = pName;
        _TextCollectibles.text = pCollect;
        _TextLevel.text = pLevel;
        _TextCheckpoint.text = pCheck;
    }

}
