using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimIndicator : MonoBehaviour
{
    [SerializeField] private Transform _aimTransform;

    [SerializeField] private Transform _boxTransform;

    [SerializeField] private Transform _circleTransform;

    [SerializeField] private Transform _gunExit;

    private MeshRenderer _meshRenderer;
    private Quaternion _currentAimRotation;
    private bool _canShoot = true;
    private PlayerAimSettings _aimSettings;

    private void Start()
    {
        _aimTransform.gameObject.SetActive(false);
    }

    private void Aim(Vector2 aim, bool canShoot)
    {
        if (!_aimTransform.gameObject.activeSelf)
            _aimTransform.gameObject.SetActive(true);
        RotateAim(aim);
        SetCanShootIndication(canShoot);
    }

    private void RotateAim(Vector2 aim)
    {
        _currentAimRotation = Quaternion.LookRotation(new Vector3(aim.x, 0, aim.y));
    }

    public void AimAtTarget(Vector3 target, bool canShoot)
    {
        var gunExitPosition = _gunExit.position;
        Aim(new Vector2(target.x - gunExitPosition.x, target.z - gunExitPosition.z).normalized, canShoot);
        SetAimLength(Vector3.Distance(target, gunExitPosition));
    }

    public void QuickAim(Vector3 target)
    {
        var gunExitPosition = _gunExit.position;
        RotateAim(new Vector2(target.x - gunExitPosition.x, target.z - gunExitPosition.z).normalized);
        _aimTransform.rotation = _currentAimRotation;
    }

    private void SetCanShootIndication(bool canShoot)
    {
        if (canShoot == _canShoot)
            return;

        _canShoot = canShoot;
        _meshRenderer.material.color = canShoot ? new Color(1f, 1f, 1f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
    }

    public void ConfigureAim(PlayerAimSettings aimSettings)
    {
        _aimSettings = aimSettings;
        switch (aimSettings.type)
        {
            case PlayerAimType.BOX:
                _boxTransform.gameObject.SetActive(true);
                _circleTransform.gameObject.SetActive(false);
                SetBoxDimensions(aimSettings.width);
                _meshRenderer = _boxTransform.GetComponentInChildren<MeshRenderer>();
                break;
            case PlayerAimType.CIRCLE:
                _circleTransform.gameObject.SetActive(true);
                _boxTransform.gameObject.SetActive(false);
                SetCircleDimensions(aimSettings.width);
                _meshRenderer = _circleTransform.GetComponentInChildren<MeshRenderer>();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void SetBoxDimensions(float width)
    {
        _boxTransform.localScale = new Vector3(width, _boxTransform.localScale.y, 1);
    }
    
    private void SetCircleDimensions(float width)
    {
        _circleTransform.localScale = new Vector3(width, width, 1);
    }

    private void SetAimLength(float length)
    {
        switch (_aimSettings.type)
        {
            case PlayerAimType.BOX:
                _boxTransform.localScale = new Vector3(_boxTransform.localScale.x, length, 1);
                _boxTransform.localPosition = new Vector3(0, 0, length / 2);
                break;
            case PlayerAimType.CIRCLE:
                _circleTransform.localPosition = new Vector3(0, 0, length);
                break;
            default:
                throw new NotImplementedException();
        }
    }
    
    private void LateUpdate()
    {
        RotateAim();
    }

    private void RotateAim()
    {
        if (_aimTransform.rotation != _currentAimRotation)
        {
            _aimTransform.rotation = _currentAimRotation;
        }
    }

    public void StopAim()
    {
        if (_aimTransform.gameObject.activeSelf)
            _aimTransform.gameObject.SetActive(false);
    }
}