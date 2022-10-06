using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[OrderBefore(typeof(HealthController))]
public class DefenseTurretController : IntruderCharacterController
{
    public override void Spawned()
    {
        base.Spawned();
    }

    public override Character GetCharacter()
    {
        return Characters.GetInstance().GetDefenseTurret();
    }
}
