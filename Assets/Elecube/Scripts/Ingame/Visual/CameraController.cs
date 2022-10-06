using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CameraShakeController _shakeController;

    private void Awake()
    {
        _instance = this;
    }

    public static CameraController GetInstance()
    {
        return _instance;
    }

    public void SetTarget(Transform target)
    {
        _virtualCamera.Follow = target;
    }

    public void Shake()
    {
        _shakeController.Shake();
    }
}
