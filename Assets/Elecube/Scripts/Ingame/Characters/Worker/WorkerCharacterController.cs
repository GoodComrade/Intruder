using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class WorkerCharacterController : IntruderCharacterController
{
    protected WorkerState CurrentState { get; private set; } = WorkerState.Idle;
    [SerializeField] private Transform _movingObject;

    public Transform GetMovingTransform()
    {
        return _movingObject.transform;
    }
    
    protected virtual void SetCurrentState(WorkerState state)
    {
        if(CurrentState == WorkerState.Dead)
            return;
        CurrentState = state;
    }
    
    public override Character GetCharacter()
    {
        return Characters.GetInstance().GetWorker();
    }

    public override bool IsEatable()
    {
        return IsDead() && !_fallenFromIsland;
    }

    public override void CharacterEaten(IntruderCharacterController eater)
    {
        RPC_CharacterEaten();
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_CharacterEaten(RpcInfo info = default)
    {
        base.DieFinal();
    }

    public override void Die()
    {
        base.Die();
        SetCurrentState(WorkerState.Dead);
    }

    protected override void DieFinal()
    {
        if (_fallenFromIsland)
        {
            base.DieFinal();
        }
    }

    public override void FallFromIsland()
    {
        if (_dead)
        {
            _fallenFromIsland = true;
            DieFinal();
        }
        else
        {
            base.FallFromIsland();   
        }
    }

    protected enum WorkerState
    {
        Idle,
        ComingToCrystal,
        MiningCrystal,
        Scared,
        Dead,
        Recon,
        Fight
    }
}
