using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsController : CharacterAnimationsController
{
    private PlayerCharacterMovementController _playerCharacterMovement;

    protected override void Start()
    {
        base.Start();
        _playerCharacterMovement = GetComponent<PlayerCharacterMovementController>();
    }

    protected override float CalculateCurrentSpeed()
    {
        return Mathf.Max(base.CalculateCurrentSpeed(), _playerCharacterMovement.Velocity.magnitude);
    }
}
