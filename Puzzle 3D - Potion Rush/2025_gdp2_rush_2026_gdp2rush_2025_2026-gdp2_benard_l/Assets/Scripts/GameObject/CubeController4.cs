using System;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CubeController : MonoBehaviour
{
    [SerializeField]
    private float _RaycastOffset = 0.4f;

    [SerializeField]
    private string _GroundTag = "Ground";

    [SerializeField]
    private string _ActionTag = "Action";

    [SerializeField]
    private string _WallTag = "Wall";

    public float ratio;

    public float cubeSide = 1;
    private float _CubeDiagonal;
    private float _OffsetY;

    private RaycastHit _Hit;
    private float _RaycastDistance;

    private Action _DoAction;

    private Vector3 _FromPosition;
    private Vector3 _ToPosition;
    private Vector3 _PivotPoint;

    private Quaternion _FromRotation;
    private Quaternion _ToRotation;

    private Vector3 _MovementDirection;
    private Quaternion _MovementRotation;

    private int _TickCount = 0;

    void Start()
    {
        TickManager.onTick += OnTickEvent;


        _CubeDiagonal = Mathf.Sqrt(2) * cubeSide;
        _OffsetY = _CubeDiagonal * 0.5f - cubeSide * 0.5f;
        _RaycastDistance = cubeSide * 0.5f + _RaycastOffset;

        _MovementDirection = Vector3.forward;
        _MovementRotation = Quaternion.AngleAxis(90f, transform.right);

        _DoAction = DoActionVoid;
    }

    void Update()
    {
        UpdateRatio();
        _DoAction();
        
    }


    private void OnTickEvent()
    {
        //Debug.Log(transform.position);
        CheckCollision();
    }

    private void UpdateRatio()
    {
        ratio = TickManager.GetRatioBetweenTicks();
    }

    private void SetModeVoid()
    {
        _DoAction = DoActionVoid;
    }

    private void DoActionVoid()
    {

    }

    private void SetModeMove()
    {
        //transform.position = new Vector3(transform.position.x, _CubeSide *0.5f , transform.position.z);
        _FromPosition = transform.position;
        _ToPosition = _FromPosition + _MovementDirection;

        _FromRotation = transform.rotation;
        _ToRotation = _MovementRotation * _FromRotation;

        _PivotPoint = _FromPosition + _MovementDirection * cubeSide * 0.5f + Vector3.down * cubeSide * 0.5f;
        Debug.Log(_PivotPoint);
        _DoAction = DoActionMove;
    }

    private void DoActionMove()
    {
        transform.position = RotationFromPivotPoint(_FromPosition, Vector3.Cross(Vector3.up, _MovementDirection),
       Mathf.PI * 0.5f * ratio, _PivotPoint);

        transform.rotation = Quaternion.Lerp(_FromRotation, _ToRotation, ratio);
    }

    private void SetDirection(Vector3 pDirection)
    {
        _MovementDirection = pDirection;
        _MovementRotation = Quaternion.AngleAxis(90,Vector3.Cross(Vector3.up,pDirection));
    }

    private void SetModeFall()
    {
        _FromPosition = transform.position;
        _ToPosition = _FromPosition + Vector3.down;
        _DoAction = DoActionFall;
    }

    private void DoActionFall()
    {
        transform.position = Vector3.Lerp(_FromPosition, _ToPosition, ratio);
    }

    private void CheckCollision()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _Hit, _RaycastDistance))
        {
            GameObject lCollided = _Hit.collider.gameObject;
            //Debug.Log("Collider = " + lCollided.name);
            Debug.DrawRay(transform.position, Vector3.down * _RaycastDistance, Color.magenta, 30f);
            if(lCollided.CompareTag(_GroundTag))
            {
                SetModeMove();
            }
            if(lCollided.CompareTag(_ActionTag))
            {
                SetDirection(lCollided.transform.forward);
                SetModeMove();
                return;
            }
        }
        else
        {
            //Debug.Log("Fall");
            SetModeFall();
        }

        if (Physics.Raycast(transform.position,_MovementDirection,out _Hit, _RaycastDistance))
        {
            GameObject lCollided = _Hit.collider.gameObject;
            if (lCollided.CompareTag(_WallTag))
            {
                if (_TickCount < 2)
                {
                    //Debug.Log("Check Collision");
                    
                    SetModeVoid();
                    _TickCount++;
                }
                else
                {
                    
                    if (Physics.Raycast(transform.position, transform.right, out _Hit, _RaycastDistance)) SetDirection(-_MovementDirection);
                    else SetDirection(Quaternion.AngleAxis(90,Vector3.up) * _MovementDirection);
                    SetModeMove();
                    _TickCount = 0;
                }
            }
        }
        
    }

    private Vector3 RotationFromPivotPoint(Vector3 pX, Vector3 pAxis, float pAngle, Vector3 pPivot)
    {
        pAxis = pAxis.normalized;
        pX = pX - pPivot;

        return pPivot + (pX + Mathf.Sin(pAngle) * Vector3.Cross(pAxis, pX) + (1 - Mathf.Cos(pAngle)) * Vector3.Cross(pAxis, Vector3.Cross(pAxis, pX)));
    }
}
