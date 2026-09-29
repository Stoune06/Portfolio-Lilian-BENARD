using UnityEngine;

public class Disconnect : MonoBehaviour
{
    public void DisconnectUser()
    {
        DataManager.Instance.SignOut();
    }
}
