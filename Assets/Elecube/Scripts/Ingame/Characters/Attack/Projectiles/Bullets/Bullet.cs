using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Bullet uses predictive spawning to provide immediate feedback on the client
/// firing the bullet even when this does not have state authority (hosted mode).
/// </summary>
[OrderAfter(typeof(HitboxManager))]
public abstract class Bullet : Projectile, ICanSetCharacter
{
    [Header("Bullet settings:")]
    [SerializeField] protected Transform _bulletVisualParent;
    [SerializeField] protected float _timeToLive = 1.5f;
    [SerializeField] protected CollisionConfiguration _collisionConfiguration;
    [SerializeField] private List<OnHitEffect> _onHitEffects = new List<OnHitEffect>();

    [Networked] protected IntruderCharacterController IntruderCharacterController { get; set; }
    
    [Networked] private int networkedStartTick { get; set; }
    private Tick _predictedStartTick;
    protected Tick startTick
    {
        get => Object.IsPredictedSpawn ? _predictedStartTick : networkedStartTick;
        set
        {
            if (Object.IsPredictedSpawn) _predictedStartTick = value;
            else networkedStartTick = value;
        }
    }

    [Networked] protected Vector2 startPosition { get; set; }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (IsTimeouted())
        {
            Runner.Despawn(Object);
        }
    }

    public override void InitNetworkState(Vector3 ownerVelocity, IntruderCharacterController intruderCharacter, Vector3 aimTarget)
    {
        startPosition = new Vector2(transform.position.x, transform.position.z);
        startTick = Runner.Tick;
        SetCharacter(intruderCharacter);
    }

    protected virtual bool IsTimeouted()
    {
        return Runner.Tick > _timeToLive / Runner.DeltaTime  + startTick;
    }

    public override void Spawned()
    {
        base.Spawned();
        SetCharacter(IntruderCharacterController);
    }

    protected virtual void ApplyDamage(IntruderHitboxRoot hitObject, short damage)
    {
        hitObject.Hit(damage, IntruderCharacterController);
        foreach (var effect in _onHitEffects)
        {
            effect.ApplyEffect(this, hitObject);
        }
    }
    public virtual void SetCharacter(IntruderCharacterController intruderCharacterController)
    {
        IntruderCharacterController = intruderCharacterController;
    }
}