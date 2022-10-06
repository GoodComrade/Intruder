using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationsController : PlayerAnimationsController
{
    private static readonly int AnimatorEating = Animator.StringToHash("eating");
    private static readonly int AnimatorEvolving = Animator.StringToHash("evolving");
    
    public void SetEating(bool on)
    {
        _animator.SetBool(AnimatorEating, on);
    }
    
    public void SetEvolving(bool on)
    {
        _animator.SetBool(AnimatorEvolving, on);
    }
}
