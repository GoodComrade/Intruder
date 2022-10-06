using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CritterAttackController : NetworkBehaviour, ICanDie
{
    [SerializeField] [Range(0.25f, 20f)] private float _attackRange;
    [SerializeField] [Range(0.25f, 5f)] private float _attackInterval;
    [SerializeField] private float _damageModifier = 2f;
    [SerializeField] private float _workerDamageModifier = 0.3f;
    
    private CritterAnimationsController _animations;
    private CritterCharacterController _critterCharacter;
    private float _lastAttackUpdateTime;
    private bool _dead;
    protected void Awake()
    {
        _critterCharacter = GetComponent<CritterCharacterController>();
        _animations = GetComponent<CritterAnimationsController>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        
        if(_dead)
            return;
        if (_lastAttackUpdateTime + _attackInterval < Runner.SimulationTime)
        {
            _lastAttackUpdateTime = Runner.SimulationTime;
            if (_critterCharacter.HasTarget() && GetRangeToTarget() <= _attackRange)
            {
                Attack(_critterCharacter.GetTarget());
            }
        }
    }

    private float GetRangeToTarget()
    {
        return Vector3.Distance(_critterCharacter.GetNetworkTransform().ReadPosition(),
            _critterCharacter.GetTarget().GetNetworkTransform().ReadPosition());
    }

    private void Attack(IntruderCharacterController character)
    {
        _animations.TriggerAttack();
        float damage = _damageModifier * _critterCharacter.GetCharacter().CharacterDamage;
        if (character.GetCharacter().GetCharacterType() == CharacterType.WORKER)
            damage *= _workerDamageModifier;
        character.GetHealthController().Hit(damage, _critterCharacter);
        _critterCharacter.DoRevealAction();
    }
    
    public void Die()
    {
        _dead = true;
    }
}
