using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class SceneChange : MonoBehaviour
    {
        [SerializeField] private string _TargetSceneName;
        [SerializeField] private string _TargetSceneAlt;
        [SerializeField] private Scene _CurrentScene;
        [SerializeField] private bool _Additive;
       
        private void Start()
        {
            _CurrentScene = SceneManager.GetActiveScene();
        }
        public void ChangeScene()
        {
            if (_Additive)
            {
                SceneManager.LoadScene(_TargetSceneName, LoadSceneMode.Additive); //If adding a scene
            }
            else
            {
                SceneManager.LoadScene(_TargetSceneName, LoadSceneMode.Single); //If wanting to change to a scene
            }
        }

        public void ChangeSceneIfLogged()
        {
            if (DataManager.Instance.IsSignedIn && _TargetSceneAlt != null) 
            {
                if (_Additive)
                {
                    SceneManager.LoadScene(_TargetSceneAlt, LoadSceneMode.Additive); //If adding a scene
                }
                else
                {
                    SceneManager.LoadScene(_TargetSceneAlt, LoadSceneMode.Single); //If wanting to change to a scene
                }
            }
            else
            {
                if (_Additive)
                {
                    SceneManager.LoadScene(_TargetSceneName, LoadSceneMode.Additive); //If adding a scene
                }
                else
                {
                    SceneManager.LoadScene(_TargetSceneName, LoadSceneMode.Single); //If wanting to change to a scene
                }
            }
        }
    }
}
