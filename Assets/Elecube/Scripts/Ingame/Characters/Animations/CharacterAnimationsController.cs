using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class CharacterAnimationsController : SimulationBehaviour, ICanDie, ICanSetCharacter, ISpawned
{
    protected static readonly int AnimatorSpeed = Animator.StringToHash("speed");
    private static readonly int AnimatorMining = Animator.StringToHash("mine");
    private static readonly int AnimatorDead = Animator.StringToHash("dead");
    
    [FormerlySerializedAs("animator")] [SerializeField] protected Animator _animator;
    [SerializeField] protected NetworkMecanimAnimator _networkAnimator;
    protected UnityEngine.AI.NavMeshAgent _navmeshAgent;

    private bool _hasNavmeshAgent;
    private float _currentRunSpeed;
    protected bool _isDisabled = true;

    protected virtual void Start()
    {
        _currentRunSpeed = _animator.GetFloat(AnimatorSpeed);
    }

    public void SetRunSpeed(float speed)
    {
        if(Math.Abs(speed - _currentRunSpeed) < 0.0001f)
            return;
        _currentRunSpeed = speed;
        _animator.SetFloat(AnimatorSpeed, speed);
    }
    
    protected virtual void Update()
    {
        if(_isDisabled || !Object.HasStateAuthority)
            return;
        SetRunSpeed(CalculateCurrentSpeed());
    }

    protected virtual float CalculateCurrentSpeed()
    {
        if (!_hasNavmeshAgent)
            return 0;
        return _navmeshAgent.desiredVelocity.magnitude;
    }

    public void SetMiningCrystal(bool on)
    {
        _animator.SetBool(AnimatorMining, on);
    }
    
    public void SetDead(bool on)
    {
        _animator.SetBool(AnimatorDead, on);
    }
    public virtual void SetCharacter(IntruderCharacterController intruderCharacterController)
    {
        _navmeshAgent = intruderCharacterController.GetComponent<NavMeshAgent>();
        _hasNavmeshAgent = _navmeshAgent != null;
    }

    public void Spawned()
    {
        _isDisabled = false;
    }

    public void Die()
    {
        _isDisabled = true;
    }
}
