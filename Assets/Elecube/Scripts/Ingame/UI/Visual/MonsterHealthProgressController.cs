using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthProgressController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Slider _slider;

    public void SetMonsterHealth(int current, int max)
    {
        _text.text = current + "/"+ max;
        _slider.maxValue = max;
        _slider.value = current;
    }
}
