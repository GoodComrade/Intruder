using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[OrderBefore(typeof(HealthController))]
public class MonsterPlayerCharacterController : PlayerCharacterController
{
    private int _evolveStage = 0;
    private TickTimer _ongoingEvolveTimer;

    public MonsterPlayerWrapperController GetMonsterWrapper()
    {
        return playerWrapper as MonsterPlayerWrapperController;
    }

    private void ProcessInput()
    {
        if (!GetMonsterWrapper().IsReadyToEvolve())
            return;
        if (GetInput(out NetworkInputMaster data))
        {
            if (data.evolve)
            {
                ExecuteEvolve();
            }
        }
    }

    private void ProcessEvolve()
    {
        if (_ongoingEvolveTimer.Expired(Runner))
        {
            FinishEvolve();
        }
    }

    public override void FallFromIsland()
    {
        GetComponent<PlayerCharacterMovementController>().TeleportToClosestLand();
    }

    private void ExecuteEvolve()
    { 
        _ongoingEvolveTimer = TickTimer.CreateFromSeconds(Runner, GameplayConstants.EvolveDuration);
        GetMonsterWrapper().RemoveMeatEaten();
        GetMovement().SetMovementDisabled(true);
        SetAttackAndAbilityDisabled(true);
        GetMonsterAnimationsController().SetEvolving(true);
        
    }

    private void FinishEvolve()
    {
        _ongoingEvolveTimer = new TickTimer();
        GetMonsterWrapper().FinishEvolve();  
        GetMovement().SetMovementDisabled(false);
        SetAttackAndAbilityDisabled(false);
        GetMonsterAnimationsController().SetEvolving(false);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!_dead && Object.HasStateAuthority)
        {
            ProcessInput();
            ProcessEvolve();
        }
    }

    public void SetEvolveStage(int evolveStage)
    {
        Debug.Log("Setting evolve stage " + evolveStage);
        _evolveStage = evolveStage;
        foreach (var evolve in GetComponentsInChildren<ICanSetEvolveStage>(true))
        {
            evolve.SetEvolveStage(evolveStage);
        }
    }

    public override int GetPowerUps()
    {
        return _evolveStage;
    }

    public MonsterAnimationsController GetMonsterAnimationsController()
    {
        return (MonsterAnimationsController) _animationsController;
    }
}
