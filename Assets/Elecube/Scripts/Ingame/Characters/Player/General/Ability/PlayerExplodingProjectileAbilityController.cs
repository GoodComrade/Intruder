using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExplodingProjectileAbilityController : PlayerAimedAbilityController
{
    [SerializeField] private Transform _gunExit;
    [SerializeField] private PlayerCharacterMovementController _movementController;
    public override void DoAbility()
    {
        base.DoAbility();
        SpawnNetworkProjectile(Runner.LocalPlayer, new Vector3(_gunExit.position.x, GameplayConstants.ProjectileAltitude, _gunExit.position.z),  _gunExit.rotation, 
            _movementController.Velocity, _aimTarget); 
    }
}
