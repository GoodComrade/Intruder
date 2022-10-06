using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class AIPlayerController : SimulationBehaviour, ICanDie, ICanSetCharacter, ISpawned
{
    protected IntruderCharacterController _intruderCharacter;
    private PlayerCharacterMovementController _playerCharacterMovement;
    private PlayerWeaponController _playerWeapon;
    private TickTimer _decisionTimer;
    private TickTimer _movementDecisionTimer;
    protected bool _disabled;

    
    public void Spawned()
    {
        if(Runner.GameMode != GameMode.Single)
            Disable();
    }

    public override void FixedUpdateNetwork()
    {
        if (_disabled)
            return;
        base.FixedUpdateNetwork();
        if (_decisionTimer.ExpiredOrNotRunning(Runner))
        {
            DoDecision();
        }
        if (_movementDecisionTimer.ExpiredOrNotRunning(Runner))
        {
            DoMovementDecision();
        }
    }

    protected virtual void DoDecision()
    {
        ScheduleDecision();
        TryShootWeapon();
    }

    protected virtual void DoMovementDecision()
    {
        ScheduleMovementDecision();
    }

    private void TryShootWeapon()
    {
        if (_playerWeapon.CanStartFire() && _playerWeapon.TryAutoAim(false))
        {
            _playerWeapon.DoAbility();
        }
    }

    protected virtual void ScheduleDecision()
    {
        _decisionTimer = TickTimer.CreateFromSeconds(Runner, 0.61f);
    }
    
    protected virtual void ScheduleMovementDecision()
    {
        _movementDecisionTimer = TickTimer.CreateFromSeconds(Runner, 1.55f);
    }

    protected virtual void SetDestination(Vector3 destination)
    {
        _playerCharacterMovement.SetAiDestination(destination);
    }

    protected abstract List<PlayerWrapperController> GetEnemyPlayers();
    
    public void Die()
    {
        Disable();
    }

    private void Disable()
    {
        _disabled = true;
    }

    public void SetCharacter(IntruderCharacterController intruderCharacterController)
    {
        _intruderCharacter = intruderCharacterController;
        _playerCharacterMovement = _intruderCharacter.GetComponent<PlayerCharacterMovementController>();
        _playerWeapon = _intruderCharacter.GetComponent<PlayerWeaponController>();
        if (_intruderCharacter.GetSide() == CharacterSide.PLAYER)
        {
            Disable();
        }
    }
}
