using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayTimerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Update()
    {
        _text.text = IntruderHelper.FormatTime(GameController.GetInstance().GetGameTimeController().GetGameTime());
    }
}
