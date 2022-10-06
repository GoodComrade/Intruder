using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandDegradationIndicatorController : MonoBehaviour, IIslandDegradationListener
{
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _visual;
    public void SetIslandDegradationPercentage(float percentage)
    {
        _slider.value = 1f - percentage;
    }

    public void SetMiningLocked(bool on)
    {
        _visual.SetActive(!on);
    }
    
    private void LateUpdate()
    {
        transform.forward = -CameraController.GetInstance().transform.forward;
    }
}
