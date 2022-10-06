using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuMatchingPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private Slider _progressSlider;
    public void Show(Stage stage)
    {
        this.gameObject.SetActive(stage != Stage.HIDDEN);
        switch (stage)
        {
            case Stage.STARTING_ROOM:
                _statusText.text = "Initiating";
                _progressSlider.value = 10;
                break;
            case Stage.WAITING_FOR_PLAYER:
                _statusText.text = "Looking for opponent";
                _progressSlider.value = 20;
                break;
            case Stage.STARTING:
                _statusText.text = "Starting match";
                _progressSlider.value = 75;
                break;
        }
    }

    public enum Stage
    {
        HIDDEN, STARTING_ROOM, WAITING_FOR_PLAYER, STARTING
    }
}
