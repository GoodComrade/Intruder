using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// AreaInstantBullet is the instant-hit alternative to a moving bullet.
/// The point of representing this as a NetworkObject is that it allow it to work the same
/// in both hosted and shared mode. If it had been done at the trigger (the weapon spawning the instant hit) with an RPC for visuals,
/// we would not have been able to apply damage to the target because we don't have authority over that object in shared mode.
/// Because it now runs on all clients, it will also run on the client that owns the target that needs to take damage.
/// </summary>
public class AreaInstantBullet : Bullet
{
    [Header("Area Instant Bullet:")] [SerializeField]
    private float _range = 5;

    [SerializeField] private float _width = 1f;
    [SerializeField] private float _instantDamageModifier = 1.0f;
    [SerializeField] [Range(1, 32)] private short _maxTargetCount = 1;
    private short InstantDamage { get; set; }

    [Networked] [Capacity(32)] private NetworkArray<IntruderHitboxRoot> networkedTarget { get; }
    private NetworkArray<IntruderHitboxRoot> _predictedTarget;
    private NetworkArray<IntruderHitboxRoot> target => Object.IsPredictedSpawn ? _predictedTarget : networkedTarget;


    [Networked] private byte networkedTargetCount { get; set; }
    private byte _predictedTargetCount;

    private byte targetCount
    {
        get => Object.IsPredictedSpawn ? _predictedTargetCount : networkedTargetCount;
        set
        {
            if (Object.IsPredictedSpawn) _predictedTargetCount = value;
            else networkedTargetCount = value;
        }
    }

    private bool _executed = false;
    private List<LagCompensatedHit> _areaHits = new List<LagCompensatedHit>();

    public override void InitNetworkState(Vector3 ownerVelocity, IntruderCharacterController intruderCharacter, Vector3 aimTarget)
    {
        base.InitNetworkState(ownerVelocity, intruderCharacter, aimTarget);
        if (_maxTargetCount == 1 && CheckForImpact(transform.forward, transform.position, out var hitInfo))
        {
            target.Set(0, (IntruderHitboxRoot) hitInfo.Hitbox.Root);
        }
        else if (_maxTargetCount > 1 && CheckForImpacts(transform.forward, transform.position, out var hitInfos))
        {
            for (int i = 0; i < hitInfos.Count; i++)
            {
                target.Set(i, (IntruderHitboxRoot) hitInfos[i].Hitbox.Root);
            }
            targetCount = (byte) hitInfos.Count;
        }
    }

    public override void SetCharacter(IntruderCharacterController intruderCharacter)
    {
        base.SetCharacter(intruderCharacter);
        InstantDamage =
            (short) (GameplayConstants.GetStatForLevel(
                         _instantDamageModifier * intruderCharacter.GetCharacter().CharacterDamage, 1) *
                     GameplayConstants.GetPowerStatMultiplier(0, intruderCharacter.GetPowerUps()));
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!_executed)
        {
            _executed = true;
            for (int i = 0; i < targetCount; i++)
            {
                ApplyDamage(target.Get(i), InstantDamage);
            }
        }
    }

    private bool CheckForImpact(Vector3 direction, Vector3 position, out LagCompensatedHit hit)
    {
        int collisions = Runner.LagCompensation.OverlapBox(position + (_range * direction * 0.5f),
            new Vector3(_width / 2f, 1, _range / 2f),
            Quaternion.LookRotation(direction), IntruderCharacterController.GetControllingPlayer(), _areaHits,
            _collisionConfiguration.DamageLayer,
            HitOptions.None, false);
        if (collisions < 1)
        {
            hit = new LagCompensatedHit();
            return false;
        }

        hit = IntruderHelper.GetClosestLagCompensatedHit(transform.position, _areaHits, collisions);
        return true;
    }

    private bool CheckForImpacts(Vector3 direction, Vector3 position, out List<LagCompensatedHit> hits)
    {
        int collisions = Runner.LagCompensation.OverlapBox(position + (_range * direction * 0.5f),
            new Vector3(_width / 2f, 1, _range / 2f),
            Quaternion.LookRotation(direction), IntruderCharacterController.GetControllingPlayer(), _areaHits,
            _collisionConfiguration.DamageLayer,
            HitOptions.None);
        if (collisions < 1)
        {
            hits = null;
            return false;
        }
        hits = IntruderHelper.GetClosestLagCompensatedHits(transform.position, _areaHits, _maxTargetCount);
        return true;
    }
}