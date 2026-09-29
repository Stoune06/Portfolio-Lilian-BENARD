using System.Collections.Generic;
using UnityEngine;

public class UITilePreview : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _ScaleFactor = 200f;
    [SerializeField] private Vector3 _BaseRotation = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 _StackOffset = new Vector3(0f, 2f, 0f);

    [Header("Animation")]
    [SerializeField] private float _PulseSpeed = 5f;
    [SerializeField] private float _PulseIntensity = 0.1f;

    private List<GameObject> _Models = new List<GameObject>();
    private GameObject _CurrentModel;
    private Quaternion _TileDirection;

    private bool _IsSelected = false;
    private float _ElapsedTime = 0f;
    public void Show3DModel(GameObject pPrefab, Quaternion pDirectionRotation, int pQuantity)
    {
        _CurrentModel = pPrefab;
        _TileDirection = pDirectionRotation;

        SetQuantity(pQuantity);
    }


    public void SetSelected(bool pIsSelected)
    {
        _IsSelected = pIsSelected;
        if (!_IsSelected)
        {
            ResetScale();
        }
    }

    private void ResetScale()
    {
        foreach (GameObject lModel in _Models)
        {
            if (lModel != null) lModel.transform.localScale = Vector3.one * _ScaleFactor;
        }
    }

    public void SetQuantity(int pQuantity)
    {
        if (_CurrentModel == null) return;
        while (_Models.Count < pQuantity)
        {
            GameObject lClone = Instantiate(_CurrentModel, transform);
            Destroy(lClone.GetComponent<Tile>());
            Destroy(lClone.GetComponent<Rigidbody>());
            foreach (Collider lCollider in lClone.GetComponentsInChildren<Collider>())
            {
                Destroy(lCollider);
            }

            LayerUtils.SetLayerRecursively(lClone, LayerMask.NameToLayer("UI"));
            lClone.transform.localScale = Vector3.one * _ScaleFactor;

            _Models.Add(lClone);
        }

        for (int i = 0; i < _Models.Count; i++)
        {
            if (i < pQuantity)
            {
                GameObject lModel = _Models[i];
                lModel.SetActive(true);
                lModel.transform.localPosition = (_StackOffset * i) - (Vector3.forward * 0.5f * i);
            }
            else
            {
                _Models[i].SetActive(false);
            }
        }
    }

    private void Update()
    {
        _ElapsedTime += Time.deltaTime;
        if (_Models.Count > 0)
        {
            UpdateRotation();
            if (_IsSelected)
            {
                AnimatePulse();
            }
        }
        
    }

    private void AnimatePulse()
    {
        float lPulseFactor = 1f + Mathf.Sin(_ElapsedTime * _PulseSpeed) * _PulseIntensity;

        Vector3 lTargetScale = Vector3.one * _ScaleFactor * lPulseFactor;

        foreach (GameObject lModel in _Models)
        {
            if (lModel != null) lModel.transform.localScale = lTargetScale;
        }
    }

    private void UpdateRotation()
    {
        // --- MODIFICATION ICI ---
        // On essaie d'abord de r�cup�rer la cam�ra de jeu d�finie dans le GameManager
        Camera targetCamera = null;

        if (GameManager.instance != null && GameManager.instance.gameCamera != null)
        {
            targetCamera = GameManager.instance.gameCamera;
        }
        else
        {
            // Fallback si le GameManager n'est pas encore pr�t (ex: test unitaire)
            targetCamera = Camera.main;
        }

        // Si on a trouv� une cam�ra, on l'utilise pour la rotation
        if (targetCamera != null)
        {
            float lCameraY = targetCamera.transform.eulerAngles.y;
            Quaternion lRotationCorrection = Quaternion.Euler(0, -lCameraY, 0);
            Quaternion targetRotation = Quaternion.Euler(_BaseRotation) * _TileDirection * lRotationCorrection;

            foreach (GameObject model in _Models)
            {
                if (model != null) model.transform.localRotation = targetRotation;
            }
        }
    }
}
