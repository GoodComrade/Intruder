using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : SimulationBehaviour, IPersonDisable, ICanChangeHealth, ICanSetCharacter, ICanSetEvolveStage, IBushHide, ICanDie
{
    [SerializeField] private TextMeshProUGUI _meatCountText;
    
    [SerializeField] private GameObject _visualObject;
    [SerializeField] private GameObject _powerupVisualObject;

    [SerializeField] private float _hideDelay = 4.0f;
    [SerializeField] private Animator _animator;
    
    [Header("Health:")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private GameObject _healthPlayerObject;
    [SerializeField] private GameObject _healthAllyObject;
    [SerializeField] private GameObject _healthEnemyObject;
    
    [Header("Charges:")]
    [SerializeField] private GameObject _chargesVisualObject;
    [SerializeField] private Slider _chargeSlider;
    [SerializeField] private Image _chargeFrameImage;
    [SerializeField] private Sprite _charges2Sprite;
    [SerializeField] private Sprite _charges3Sprite;
    [SerializeField] private Sprite _charges4Sprite;

    private bool _alwaysShow;
    private PlayerWeaponController _weaponController;
    
    private bool _showingCharges = false;
    private bool _hidden = false;
    private float _lastHealthChange;
    private static readonly int Hidden = Animator.StringToHash("hidden");
    private static readonly int HidePercentage = Animator.StringToHash("hidePercentage");

    private void Start()
    {
        _powerupVisualObject.SetActive(false);
    }

    private void SetShowingCharges(bool on)
    {
        _showingCharges = on;
        _chargesVisualObject.SetActive(on);
        if (on)
        {
            _chargeSlider.maxValue = _weaponController.maxCharges;
            switch (_weaponController.maxCharges)
            {
                case 2:
                    _chargeFrameImage.sprite = _charges2Sprite;
                    break;
                case 3:
                    _chargeFrameImage.sprite = _charges3Sprite;
                    break;
                case 4:
                    _chargeFrameImage.sprite = _charges4Sprite;
                    break;
            }
        }
    }
    
    public override void Render()
    {
        base.Render();
        if (IsShowingCharges())
        {
            ChargeUpdate();
        }
    }

    private bool IsShowingCharges()
    {
        return _showingCharges;
    }
    
    private void LateUpdate()
    {
        transform.forward = -CameraController.GetInstance().transform.forward;

        if (!_hidden && !_alwaysShow && Time.timeSinceLevelLoad - _lastHealthChange > _hideDelay)
        {
            SetHidden(true);
        }
    }
    
    private void ChargeUpdate()
    {
        _chargeSlider.value = _weaponController.currentCharge;
    }

    private void ChangeHealth(HealthController healthController, float delta)
    {
        _healthSlider.value = (float)healthController.currentHealth / healthController.MaximumHealth;
        if (delta != 0 && _hidden)
        {
            _lastHealthChange = Time.timeSinceLevelLoad;
            SetHidden(false);
        }
    }

    public void HealthChanged(HealthController healthController, float delta)
    {
        ChangeHealth(healthController, delta);
    }
    
    public void PersonDisable()
    {
        _visualObject.SetActive(false);
    }

    public void PersonEnable()
    {
        _visualObject.SetActive(true);
    }

    public void SetCharacter(IntruderCharacterController intruderCharacterController)
    {
        if (intruderCharacterController.GetSide() != CharacterSide.PLAYER)
        {
            _alwaysShow = false;
            SetHidden(true);
            SetShowingCharges(false);
        }
        else
        {
            _alwaysShow = true;
            SetHidden(false);
            _weaponController = intruderCharacterController.GetWeaponController();
            SetShowingCharges(_weaponController != null);
        }
        
        _healthPlayerObject.SetActive(intruderCharacterController.GetSide() == CharacterSide.PLAYER);
        _healthAllyObject.SetActive(intruderCharacterController.GetSide() == CharacterSide.ALLY);
        _healthEnemyObject.SetActive(intruderCharacterController.GetSide() == CharacterSide.ENEMY);
    }

    private void SetHidden(bool on)
    {
        _hidden = on;
        _animator.SetBool(Hidden, on);
    }

    public void SetEvolveStage(int power)
    {
        _meatCountText.text = power.ToString();
        
        if(_powerupVisualObject.activeSelf)
            return;
        _powerupVisualObject.SetActive(true);
    }

    public void SetHidePercentage(float percentage)
    {
        if(_alwaysShow)
            return;
        _animator.SetFloat(HidePercentage, percentage);
    }

    public void Die()
    {
        SetHidden(true);
    }
}
