using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(MonsterPlayerCharacterController))]
public class MeatCollectionController : NetworkBehaviour, IPersonDisable, ICanDie, ICanSetCharacter
{
    private readonly float _eatingRange = 1.75f;
    private readonly float _eatingTargetUpdateInterval = 0.3f;
    private readonly float _eatingTime = 2f;
    private float _lastEatingTargetUpdate;
    private float _eatingStartedTime;
    private MonsterPlayerCharacterController _characterController;
    private ICharacterMovement _movement;
    private bool _disabled = false;
    
    private bool _eating;
    private IntruderCharacterController _eatingTarget;
    
    protected virtual void Awake()
    {
        _movement = GetComponent<ICharacterMovement>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (Object.HasStateAuthority && !_disabled)
        {
            if (!_eating && !_movement.IsMoving())
            {
                FindTargetUpdate();
            }
            else if (_eating)
            {
                if (_movement.IsMoving() || _eatingTarget == null || !_eatingTarget.IsEatable())
                {
                    StopEating();
                }
                else
                {
                    EatingUpdate();
                }
            }
        }
    }

    private void EatingUpdate()
    {
        if (_eatingStartedTime + _eatingTime <= Runner.SimulationTime)
        {
            FinishEating();
        }
    }

    private void StartEating()
    {
        _eating = true;
        _eatingStartedTime = Runner.SimulationTime;
        _characterController.GetMonsterAnimationsController().SetEating(true);
    }

    private void StopEating()
    {
        _eating = false;
        _characterController.GetMonsterAnimationsController().SetEating(false);
    }

    private void FindTargetUpdate()
    {
        if(Runner.SimulationTime < _lastEatingTargetUpdate + _eatingTargetUpdateInterval)
            return;
        _lastEatingTargetUpdate = Runner.SimulationTime;
        if (FindEatingTarget(out _eatingTarget))
        {
            StartEating();
        }
    }

    private bool FindEatingTarget(out IntruderCharacterController target)
    {
        foreach (IntruderCharacterController character in CharactersController.GetInstance().GetAllCharacters())
        {
            if (character.IsEatable() && IsCloseEnoughToEatCorpse(character))
            {
                target = character;
                return true;
            }
        }
        target = null;
        return false;
    }

    private bool IsCloseEnoughToEatCorpse(IntruderCharacterController worker)
    {
        if (Vector3.Distance(_characterController.GetNetworkTransform().ReadPosition(), worker.GetNetworkTransform().ReadPosition()) <= _eatingRange)
        {
            return true;
        }
        return false;
    }

    private void FinishEating()
    {
        _eatingTarget.CharacterEaten(_characterController);
        _characterController.GetMonsterWrapper().AddMeatEaten();
        StopEating();
    }

    public void PersonDisable()
    {
        _disabled = true;
    }

    public void PersonEnable()
    {
        _disabled = false;
    }

    public void Die()
    {
        _disabled = false;
    }

    public void SetCharacter(IntruderCharacterController intruderCharacterController)
    {
        _characterController = intruderCharacterController as MonsterPlayerCharacterController;
    }
}