using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HunterPlayerWrapperController : PlayerWrapperController
{
    public override void Spawned()
    {
        base.Spawned();
        if (playerCharacterController.GetSide() == CharacterSide.PLAYER)
        {
            GameplayUiController.GetInstance().GetControls().SetEvolveEnabled(false);
        }
    }
}
