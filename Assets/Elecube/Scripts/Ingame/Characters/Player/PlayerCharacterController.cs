using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerCharacterController : IntruderCharacterController
{
    [Networked] protected PlayerWrapperController playerWrapper { get; set; }

    public void InitNetworkState(PlayerWrapperController player)
    {
        playerWrapper = player;
    }

    public override void InvokeDeath(PlayerRef source)
    {
        base.InvokeDeath(source);
        playerWrapper.CharacterDied();
    }

    public override Character GetCharacter()
    {
        return playerWrapper.GetCharacter();
    }

    public override PlayerRef GetControllingPlayer()
    {
        return playerWrapper.GetControllingPlayer();
    }
}
