using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(HealthController))]
public class HealthRegenController : SimulationBehaviour, ICanChangeHealth, ICanDie
{
    [SerializeField] private float _regenTimeout = 4;
    [SerializeField] private float _regenInterval = 1;
    [SerializeField] [Range(0,0.5f)] private float _regenPercentage = 0.2f;
    private HealthController _healthController;

    private TickTimer _nextRegenTime;
    private bool _disabled;

    private void Awake()
    {
        _healthController = GetComponent<HealthController>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!_disabled && Object.HasStateAuthority && _healthController.currentHealth < _healthController.MaximumHealth && _nextRegenTime.ExpiredOrNotRunning(Runner))
        {
            DoRegen();
        }
    }

    private void DoRegen()
    {
        _healthController.Heal((short)(_healthController.MaximumHealth * _regenPercentage));
        _nextRegenTime = TickTimer.CreateFromSeconds(Runner, _regenInterval);
    }

    private void ResetTimeout()
    {
        _nextRegenTime = TickTimer.CreateFromSeconds(Runner, _regenTimeout);
    }

    public void HealthChanged(HealthController healthController, float delta)
    {
        if(delta < 0)
            ResetTimeout();
    }

    public void Die()
    {
        _disabled = true;
    }
}
