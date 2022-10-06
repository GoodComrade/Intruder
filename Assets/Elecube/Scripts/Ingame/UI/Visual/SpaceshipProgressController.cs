using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipProgressController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Slider _slider;

    public void UpdateVisual(HealthController healthController)
    {
        _text.text = IntruderHelper.FormatNumber((int) healthController.currentHealth) + "/" + IntruderHelper.FormatNumber((int) healthController.MaximumHealth);
        _slider.maxValue = healthController.MaximumHealth;
        _slider.value = healthController.currentHealth;
    }
}