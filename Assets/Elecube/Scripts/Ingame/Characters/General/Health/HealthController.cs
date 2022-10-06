using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

[OrderAfter(typeof(IntruderCharacterController))]
[DisallowMultipleComponent]
public class HealthController : HitController, ICanDie, ICanSetEvolveStage
{
    [Networked(OnChanged = nameof(OnHealthChanged)), Accuracy(0.01)]
    private float maximumHealth { get; set; }

    [Networked(OnChanged = nameof(OnHealthChanged)), Accuracy(0.01)]
    private float health { get; set; }

    private float _localHealth;
    [Networked] private PlayerRef lastHitSource { get; set; }

    private IntruderCharacterController _characterController;
    private List<ICanChangeHealth> _healthChangeListeners = new List<ICanChangeHealth>();
    private bool _isDead;
    private bool _isDisabled;
    private int _currentPower;

    public float currentHealth => health;
    public float MaximumHealth => maximumHealth;


    private void Awake()
    {
        Debug.Log("Init health Awake -" + gameObject.name);
        _characterController = GetComponent<IntruderCharacterController>();
    }

    public override void Spawned()
    {
        base.Spawned();
        _healthChangeListeners = GetComponentsInChildren<ICanChangeHealth>(true).ToList();
        InitHealth();
    }

    public void AddHealthChangeListener(ICanChangeHealth listener)
    {
        _healthChangeListeners.Add(listener);
    }

    public override void Hit(float damage, IntruderCharacterController source)
    {
        if (_isDead || _isDisabled)
            return;
        ChangeHealth(-damage, source.GetControllingPlayer());
    }

    public void Heal(float amount)
    {
        if (_isDead || _isDisabled)
            return;
        ChangeHealth(amount, Object.InputAuthority);
    }

    private void ChangeHealth(float delta, PlayerRef source)
    {
        if (health == 0 && delta <= 0)
            return;

        lastHitSource = source;

        if (health + delta > MaximumHealth)
        {
            health = MaximumHealth;
        }
        else if (health <= -delta)
        {
            health = 0;
        }
        else
        {
            health = (ushort) (health + delta);
        }
    }

    public static void OnHealthChanged(Changed<HealthController> changed)
    {
        changed.Behaviour.HealthChanged();
    }

    private void HealthChanged()
    {
        float oldHealth = _localHealth;
        _localHealth = health;
        foreach (var listener in _healthChangeListeners)
        {
            listener.HealthChanged(this, health - oldHealth);
        }

        if (!_isDead && health <= 0)
        {
            _characterController.InvokeDeath(lastHitSource);
        }
    }

    public void Kill(PlayerRef source)
    {
        _isDisabled = true;
        lastHitSource = source;
        health = 0;
    }

    public void Die()
    {
        _isDead = true;
    }

    private void InitHealth()
    {
        Debug.Log("Init health - " + gameObject.name);
        Debug.Log("Init health char - " + _characterController);
        Debug.Log("Init health cha2 - " + _characterController.GetCharacter());
        maximumHealth = GameplayConstants.GetStatForLevel(_characterController.GetCharacter().CharacterHealth, 1);
        health = maximumHealth;
    }

    public void SetEvolveStage(int power)
    {
        float multiplier = GameplayConstants.GetPowerStatMultiplier(_currentPower, power);
        _currentPower = power;
        maximumHealth = (ushort) (maximumHealth * multiplier);
        health = (ushort) (health * multiplier);
    }
}