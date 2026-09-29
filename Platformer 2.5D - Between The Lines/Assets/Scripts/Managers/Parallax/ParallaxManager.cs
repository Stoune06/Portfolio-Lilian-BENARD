using Platformer.Player;
using UnityEngine;

namespace Platformer.Managers
{
    public class ParallaxManager : MonoBehaviour
    {
        private Transform _CameraTransform;
        [SerializeField] private ParallaxTransform[] _Layers;
        private Vector3 _LastCamPos;

        void Start()
        {
            if (Camera.main == null) return;

            _CameraTransform = Camera.main.transform;

            _LastCamPos = _CameraTransform.position;

            Transform[] lTransformArray;
            Transform lTransform;
            float lSpeed;
            float lFrame = 1f / 60f;

            for (int i = 0; i < _Layers.Length; i++)
            {
                lTransformArray = _Layers[i].transform.GetComponentsInChildren<Transform>();
                lSpeed = _Layers[i].speed;

                for (int j = 0; j < lTransformArray.Length; j++)
                {
                    lTransform = lTransformArray[j];
                    //if (TryGetComponent<SpriteRenderer>(out SpriteRenderer lSprite))
                    //    lTransform.position -= (_LastCamPos - lSprite.bounds.extents) * lSpeed * lFrame;
                    //else
                    //    lTransform.position -= (_LastCamPos - lTransform.position) * lSpeed * lFrame;

                    if (TryGetComponent<SpriteRenderer>(out SpriteRenderer lSprite))
                        lTransform.position -= new Vector3((_LastCamPos.x - lSprite.bounds.extents.x) * lSpeed * lFrame, 0f, 0f) ;
                    else
                        lTransform.position -= new Vector3((_LastCamPos.x - lTransform.position.x) * lSpeed * lFrame, 0f, 0f);

                }
            }
        }

        void LateUpdate()
        {
            if (_CameraTransform == null) return;

            Vector3 lTranslation = _CameraTransform.position - _LastCamPos;

            for (int i = 0; i < _Layers.Length; i++)
            {
                _Layers[i].Move(lTranslation);
            }

            _LastCamPos = _CameraTransform.position;
        }
    }
}