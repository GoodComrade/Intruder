using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerAimedAbilityController : PlayerAbilityController
{
    [Header("Aimed ability:")] [SerializeField]
    protected PlayerAimIndicator _playerAbilityIndicator;

    private bool _isAiming = false;
    private bool _wasAimingDuringPress = false;
    protected Vector3 _aimTarget;
    private List<LagCompensatedHit> _quickFireTargets = new List<LagCompensatedHit>();

    protected override void ProcessInput(NetworkInputMaster data)
    {
        base.ProcessInput(data);
        if (!IsAbilityPressed(data))
        {
            StopAim();
        }
        else
        {
            if (data.aim.magnitude < 0.125f)
            {
                StopAim();
            }
            else
            {
                Aim(data.aim);
            }
        }
    }

    private void Aim(Vector2 aim)
    {
        if (!_isAiming)
        {
            _isAiming = true;
            _wasAimingDuringPress = true;
            ConfigureAbilityIndicator();
        }

        IndicateAim(aim);
    }

    protected virtual void ConfigureAbilityIndicator()
    {
        _playerAbilityIndicator.ConfigureAim(_aimSettings);
    }

    protected virtual void IndicateAim(Vector2 aim)
    {
        AdjustAimTargetUsingCollision(aim);
        _playerAbilityIndicator.AimAtTarget(_aimTarget, true);
    }

    protected virtual void AdjustAimTargetUsingCollision(Vector2 aim)
    {
        float length = _aimSettings.length;
        if (_aimSettings.type == PlayerAimType.CIRCLE)
        {
            length = Mathf.Lerp(0f, length, aim.magnitude);
        }

        var position = transform.position;
        if (Physics.Raycast(new Vector3(position.x, GameplayConstants.ProjectileAltitude, position.z),
            new Vector3(aim.normalized.x, 0, aim.normalized.y), out var hit, length, _aimSettings.collision.CollideLayer))
        {
            _aimTarget = hit.point;
        }
        else
        {
            AdjustAimTarget(aim);
        }
    }

    protected virtual void AdjustAimTarget(Vector2 aim)
    {
        float length = _aimSettings.length;
        var position = transform.position;
        _aimTarget = new Vector3(position.x, 0, position.z) +
                     (new Vector3(aim.x, 0, aim.y) * length);
    }

    protected override void AbilityPressed(bool on)
    {
        if (_isAbilityPressed == on)
            return;
        _isAbilityPressed = on;

        if (!on)
        {
            if (_wasAimingDuringPress)
            {
                _wasAimingDuringPress = false;
                if (_isAiming)
                {
                    DoAbility();
                }
            }
            else
            {
                Quickfire();
            }
        }
    }

    public void Quickfire()
    {
        if (_aimSettings.collision.DamageLayer == default)
            return;
        TryAutoAim(true);
        DoAbility();
    }

    /// <summary>
    /// Try to find visible target automatically and adjust the aim to it. Returns true if suitable target in range is found.
    /// </summary>
    public bool TryAutoAim(bool longRange)
    {
        if (CheckForAutoAimTarget(out Vector3 target, longRange))
        {
            _aimTarget = target;
            _playerAbilityIndicator.QuickAim(_aimTarget);
            return true;
        }

        Vector3 aim = (transform.position - _aimTarget).normalized;
        AdjustAimTargetUsingCollision(-new Vector2(aim.x, aim.z));
        _playerAbilityIndicator.QuickAim(_aimTarget);
        return false;
    }

    protected virtual bool CheckForAutoAimTarget(out Vector3 target, bool longRange)
    {
        var position = transform.position;
        int collisions = Runner.LagCompensation.OverlapSphere(position, _aimSettings.length * (longRange ? 2.5f : 1.05f),
            Object.InputAuthority, _quickFireTargets, _aimSettings.collision.DamageLayer);
        foreach (var t in IntruderHelper.GetSortedClosestLagCompensatedHits(position, _quickFireTargets))
        {
            if (CanSeeTarget(((IntruderHitboxRoot)t.Hitbox.Root).GetCharacter()))
            {
                target = t.Point;
                return true;
            }
        }
        target = Vector3.zero;
        return false;
    }

    protected virtual bool CanSeeTarget(IntruderCharacterController character)
    {
        if (character.IsHiddenFromCharacter(_intruderCharacterController))
            return false;
        return IntruderHelper.CanAimAtCharacter(_intruderCharacterController, character,
            _aimSettings.collision.CollideLayer);
    }

    private void StopAim()
    {
        if (_isAiming)
        {
            _isAiming = false;
            if (_isAbilityPressed)
                VibrationsManager.Instance.VibrateStopAiming();
            _playerAbilityIndicator.StopAim();
        }
    }
}