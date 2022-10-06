using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class PlayerLeapAbilityController : PlayerAimedAbilityController
{
    [Header("Leap:")] [SerializeField] private float _leapVelocity;
    [SerializeField] private float _leapHeight;

    private PlayerCharacterMovementController _movement;
    private bool _isLeaping;
    private Vector3 _leapStart;
    private float _leapStartTime;

    public override void Spawned()
    {
        base.Spawned();
        _movement = GetComponent<PlayerCharacterMovementController>();
    }

    public override void DoAbility()
    {
        base.DoAbility();
        StartLeap();
    }

    private void StartLeap()
    {
        _movement.PersonDisable();
        _isLeaping = true;
        _leapStart = _movement.ReadPosition();
        _leapStartTime = Runner.SimulationTime;
    }

    private void LeapUpdate()
    {
        float leapProgress = (Runner.SimulationTime - _leapStartTime) / GetLeapDuration();
        _movement.SetPosition(Vector3.Lerp(_leapStart, _aimTarget, leapProgress));
        if (leapProgress >= 1f)
        {
            StopLeap();
        }
    }

    private void StopLeap()
    {
        _movement.PersonEnable();
        _isLeaping = false;
    }

    
    protected override void AdjustAimTargetUsingCollision(Vector2 aim)
    {
        base.AdjustAimTargetUsingCollision(aim);
        while (!NavMesh.SamplePosition(_aimTarget, out var hit2, 0.5f, NavMesh.AllAreas))
        {
            _aimTarget = Vector3.MoveTowards(_aimTarget, transform.position, 0.5f);
            if(Vector3.Distance(_aimTarget, transform.position) < 0.5f)
                break;
        }
    }

    private float GetLeapDuration()
    {
        return new Vector3(_aimTarget.x - _leapStart.x, 0, _aimTarget.z - _aimTarget.z).magnitude / _leapVelocity;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (_isLeaping)
        {
            LeapUpdate();
        }
    }
}