using UnityEngine;
using FMODUnity;

public class Animation_SoundEvent : MonoBehaviour
{
    public void PlaySound(string path)
    {
        //Debug.Log("start");

        if (path == null || !GetComponent<Transform>()) return;
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
        //Debug.Log("end");
    }
}