using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class IntruderHitboxRoot : HitboxRoot, IPersonDisable, ICanDie, ICanChangeHealth, ICanSetCharacter
{
    private HitController _hitController;
    private Action _onDisable;
    
    private bool _dead = false;
    private bool _hitControllerResolved;

    private IntruderCharacterController _character;
    public void PersonDisable()
    {
        this.HitboxRootActive = false;
        if (_onDisable != null)
            _onDisable();
    }

    public void Hit(short damage, IntruderCharacterController source)
    {
        ResolveHitController();
        _hitController.Hit(damage, source);
        VibrationsManager.Instance.ResolveVibrateCharacterHit(source, _character);
    }

    public void PersonEnable()
    {
        if(_dead)
            return;

        this.HitboxRootActive = true;
    }

    public void Die()
    {
        PersonDisable();
        _dead = true;
    }

    public void AddOnDisable(Action action)
    {
        _onDisable += action;
    }
    
    public void RemoveOnDisable(Action action)
    {
        _onDisable -= action;
    }

    public void SetHitController(HitController hitController)
    {
        _hitController = hitController;
        _hitControllerResolved = true;
    }
    
    private void ResolveHitController()
    {
        if (_hitControllerResolved)
            return;
        _hitController = GetComponent<HitController>();
        _hitControllerResolved = true;
    }

    public void HealthChanged(HealthController healthController, float delta)
    {
        if (_hitControllerResolved)
            return;
        _hitController = healthController;
        _hitControllerResolved = true;
    }

    public void SetCharacter(IntruderCharacterController intruderCharacterController)
    {
        _character = intruderCharacterController;
    }

    public IntruderCharacterController GetCharacter()
    {
        return _character;
    }
    
}
