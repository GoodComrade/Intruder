using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterAnimationsController : CharacterAnimationsController
{
    private static readonly int Attack = Animator.StringToHash("attack");


    public void TriggerAttack()
    {
        _networkAnimator.SetTrigger(Attack);
    }
}