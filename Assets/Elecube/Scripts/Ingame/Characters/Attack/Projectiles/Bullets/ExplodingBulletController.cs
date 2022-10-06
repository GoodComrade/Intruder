using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Object = UnityEngine.Object;

public class ExplodingBulletController : BasicBulletController
{
    [Header("Exploding bullet:")] [SerializeField]
    private float _explosionDamageModifier;
    [Networked] private short ExplosionDamage { get; set; }
    [SerializeField] private float _blastRadius;


    public override void InitNetworkState(Vector3 ownerVelocity, IntruderCharacterController intruderCharacter, Vector3 aimTarget)
    {
        base.InitNetworkState(ownerVelocity, intruderCharacter, aimTarget);
        
        ExplosionDamage = (short) (GameplayConstants.GetStatForLevel(_explosionDamageModifier * intruderCharacter.GetCharacter().CharacterDamage, 1) *
                                   GameplayConstants.GetPowerStatMultiplier(0, intruderCharacter.GetPowerUps()));
    }

    protected override void Detonate(Vector3 hitPoint)
    {
        base.Detonate(hitPoint);

        if (_blastRadius > 0)
        {
            ApplyAreaDamage(hitPoint);
        }
    }

    private void ApplyAreaDamage(Vector3 hitPoint)
    {
        int cnt = Runner.LagCompensation.OverlapSphere(hitPoint, _blastRadius, Object.InputAuthority, _areaHits, _collisionConfiguration.DamageLayer,
            HitOptions.None);
        if (cnt > 0)
        {
            for (int i = 0; i < cnt; i++)
            {
                IntruderHitboxRoot other = _areaHits[i].Hitbox.Root as IntruderHitboxRoot;
                if (other)
                {
                    ApplyDamage(other, ExplosionDamage);
                }
            }
        }
    }
}
