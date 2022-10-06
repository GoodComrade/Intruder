using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AbilityController : NetworkBehaviour, ICanSetCharacter
{
    [Networked] protected byte ShotsFired { get; set; }
    
    [SerializeField] protected Projectile _projectilePrefab;
    protected IntruderCharacterController _intruderCharacterController;
 
    
    protected void SpawnNetworkProjectile(PlayerRef owner, Vector3 position, Quaternion rotation, Vector3 ownerVelocity, Vector3 aimTarget)
    {
        ShotsFired += 1;
        // Create a key that is unique to this shot on this client so that when we receive the actual NetworkObject
        // Fusion can match it against the predicted local bullet.
        var key = new NetworkObjectPredictionKey {Byte0 = (byte) owner.RawEncoded, Byte1 = ShotsFired, Byte2 = (byte) Runner.Simulation.Tick};
        Runner.Spawn(_projectilePrefab, position, rotation, owner, (runner, obj) =>
        {
            var projectile = obj.GetComponent<Projectile>();
            projectile.InitNetworkState(ownerVelocity, _intruderCharacterController, aimTarget);
        }, key );
    }

    public virtual void SetCharacter(IntruderCharacterController intruderCharacterController)
    {
        _intruderCharacterController = intruderCharacterController;
    }
    
    protected bool IsLocalPlayer()
    {
        return _intruderCharacterController.GetSide() == CharacterSide.PLAYER;
    }
}
