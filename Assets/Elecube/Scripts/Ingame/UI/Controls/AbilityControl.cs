using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityControl : MonoBehaviour
{
    [SerializeField] private GameObject _joystickObject;
    [SerializeField] private GameObject _buttonObject;
    [SerializeField] private CooldownIndicatorController _cooldown;

    private PlayerAimSettings _settings;
    private float _currentCooldown = -1f;

    private void Start()
    {
        SetCooldown(0);
    }

    private void ShowControl()
    {
        switch (_settings.type)
        {
            case PlayerAimType.BUTTON:
                _buttonObject.SetActive(true);
                break;
           default:
                _joystickObject.SetActive(true);
                break;
        }
    }

    private void HideControl()
    {
        _joystickObject.SetActive(false);
        _buttonObject.SetActive(false);
    }

    public void SetCooldown(float progress)
    {
        if(_currentCooldown == progress)
            return;
        if (progress >= 1f)
        {
            ShowControl();
            _cooldown.Hide();
        }
        else
        {
            if(_currentCooldown >= 1f)
                HideControl();
            _cooldown.SetCooldown(progress);
        }
        _currentCooldown = progress;
    }

    public void SetEnabled(bool enabled)
    {
        this.gameObject.SetActive(enabled);
    }
    

    public void Setup(PlayerAimSettings settings)
    {
        _settings = settings;
    }
}