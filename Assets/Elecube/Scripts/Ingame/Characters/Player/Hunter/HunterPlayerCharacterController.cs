using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HunterPlayerCharacterController : PlayerCharacterController
{
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
    
    public override bool IsEatable()
    {
        return IsDead() && !_fallenFromIsland;
    }

    public override void CharacterEaten(IntruderCharacterController eater)
    {
        eater.GetHealthController().Heal(GameplayConstants.HunterEatenHealFlat + GameplayConstants.HunterEatenHealPercentage * eater.GetHealthController().MaximumHealth);
        RPC_CharacterEaten();
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_CharacterEaten(RpcInfo info = default)
    {
        base.DieFinal();
    }
}
