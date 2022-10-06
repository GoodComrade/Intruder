using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownIndicatorController : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    public void SetCooldown(float progress)
    {
        _slider.value = progress;
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
