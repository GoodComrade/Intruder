using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassRotationController : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationOffset;
    private void Start()
    {
        transform.forward = -CameraController.GetInstance().transform.forward;
        transform.localEulerAngles += _rotationOffset;
    }
}
