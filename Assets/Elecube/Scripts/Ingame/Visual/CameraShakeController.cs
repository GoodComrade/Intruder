using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    private CinemachineBasicMultiChannelPerlin _shake;

    private bool _shaking = false;
    private float _shakeEnd;

    private void Awake()
    {
        _shake = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake()
    {
        _shaking = true;
        _shake.m_AmplitudeGain = 1f;
        _shakeEnd = Time.realtimeSinceStartup + 0.1f;
    }

    private void Update()
    {
        if (_shaking && Time.realtimeSinceStartup > _shakeEnd)
        {
            _shaking = false;
            _shake.m_AmplitudeGain = 0;
        }
    }
}
