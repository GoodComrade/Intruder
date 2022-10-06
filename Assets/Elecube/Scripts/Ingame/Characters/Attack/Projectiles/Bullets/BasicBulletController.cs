using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BasicBulletController : Bullet
{
    [Header("Basic bullet:")] 
    [SerializeField] private float _impactDamageModifier;
    [Networked] private short ImpactDamage { get; set; }

    [SerializeField] private float _speed = 100;
    [SerializeField] private float _maxRange = 20f;
    [SerializeField] private float _length = 0.25f;
    [SerializeField] private float _width = 0.25f;
    [SerializeField] private float _ownerVelocityModifier = 0.1f;

    [Networked] private Vector3 networkedVelocity { get; set; }
    private Vector3 _predictedVelocity;

    protected Vector3 Velocity
    {
        get => Object.IsPredictedSpawn ? _predictedVelocity : networkedVelocity;
        set
        {
            if (Object.IsPredictedSpawn) _predictedVelocity = value;
            else networkedVelocity = value;
        }
    }

    [Networked(OnChanged = nameof(OnDestroyedChanged))]
    public NetworkBool networkedDestroyed { get; set; }

    private bool _predictedDestroyed;

    protected List<LagCompensatedHit> _areaHits = new List<LagCompensatedHit>();

    protected bool Destroyed
    {
        get => Object.IsPredictedSpawn ? _predictedDestroyed : (bool) networkedDestroyed;
        set
        {
            if (Object.IsPredictedSpawn) _predictedDestroyed = value;
            else networkedDestroyed = value;
        }
    }


    public override void InitNetworkState(Vector3 ownerVelocity, IntruderCharacterController intruderCharacter, Vector3 aimTarget)
    {
        base.InitNetworkState(ownerVelocity, intruderCharacter, aimTarget);

        Destroyed = false;
        ImpactDamage = (short) (GameplayConstants.GetStatForLevel(_impactDamageModifier * intruderCharacter.GetCharacter().CharacterDamage, 1) *
                                GameplayConstants.GetPowerStatMultiplier(0, intruderCharacter.GetPowerUps()));
        

        Vector3 fwd = transform.forward.normalized;
        Vector3 vel = ownerVelocity.normalized;
        vel.y = 0;
        fwd.y = 0;
        float multiplier = Mathf.Abs(Vector3.Dot(vel, fwd));

        Velocity = _speed * transform.forward +
                   ownerVelocity * multiplier * _ownerVelocityModifier;
    }

    /// <summary>
    /// Spawned() is invoked on all clients when the networked object is created. 
    /// Note that because Bullets are pooled, we need to reset every local property when spawning.
    /// It's entirely likely that this bullet instance has already been used and no longer has its default values.
    /// </summary>
    public override void Spawned()
    {
        base.Spawned();
        _bulletVisualParent.gameObject.SetActive(true);

        if (Velocity.sqrMagnitude > 0)
            _bulletVisualParent.forward = Velocity;

        _bulletVisualParent.forward = transform.forward;

        // We want bullet interpolation to use predicted data on all clients because we're moving them in FixedUpdateNetwork()
        GetComponent<NetworkTransform>().InterpolationDataSource = NetworkBehaviour.InterpolationDataSources.Predicted;
    }


    /// <summary>
    /// Simulate bullet movement and check for collision.
    /// This executes on all clients using the Velocity and last validated state to predict the correct state of the object
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        if (!IsTimeouted())
        {
            MoveBullet();
        }
        base.FixedUpdateNetwork();
    }

    protected virtual void MoveBullet()
    {
        float dt = Runner.DeltaTime;
        Vector3 vel = Velocity;
        float speed = vel.magnitude;
        Vector3 pos = transform.position;

        if (!Destroyed)
        {
            if (IsOutOfRange())
            {
                Detonate(transform.position);
            }
            else
            {
                if (CheckForImpact(vel, pos, speed, out var hitInfo))
                {
                    vel = HandleImpact(hitInfo);
                    pos = hitInfo.Point;
                }
            }
        }

        // If the bullet is destroyed, we stop the movement so we don't get a flying explosion
        if (Destroyed)
        {
            vel = Vector3.zero;
            dt = 0;
        }

        Velocity = vel;
        if (Velocity.sqrMagnitude > 0)
        {
            pos += dt * Velocity;
            _bulletVisualParent.forward = Velocity.normalized;
        }

        transform.position = pos;
    }

    protected virtual bool CheckForImpact(Vector3 vel, Vector3 pos, float speed, out LagCompensatedHit hit)
    {
        Vector3 dir = vel.normalized;

        if (Runner.LagCompensation.OverlapBox(pos - 0.5f * dir,
            new Vector3(_width, 2, Mathf.Max(_length, speed * Runner.DeltaTime)),
            Quaternion.LookRotation(dir), Object.InputAuthority, _areaHits, _collisionConfiguration.CollideLayer,
            HitOptions.IncludePhysX) > 0)
        {
            hit = _areaHits[0];
            return true;
        }
        hit = new LagCompensatedHit();
        return false;
    }

    private bool IsOutOfRange()
    {
        return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), startPosition) > _maxRange;
    }


    /// <summary>
    /// Bullets will detonate when they expire or on impact.
    /// After detonating, the mesh will disappear and it will no longer collide.
    /// If specified, an impact fx may play and area damage may be applied.
    /// </summary>
    protected virtual void Detonate(Vector3 hitPoint)
    {
        if (Destroyed)
            return;
        // Mark the bullet as destroyed.
        // This will trigger the OnDestroyedChanged callback which makes sure the explosion triggers correctly on all clients.
        // Using an OnChange callback instead of an RPC further ensures that we don't trigger the explosion in a different frame from
        // when the bullet stops moving (That would lead to moving explosions, or frozen bullets)
        Destroyed = true;
    }

    public static void OnDestroyedChanged(Changed<BasicBulletController> changed)
    {
        changed.Behaviour.OnDestroyedChanged();
    }

    private void OnDestroyedChanged()
    {
        if (Destroyed)
        {
            DestroyEffect();
        }
    }

    protected virtual void DestroyEffect()
    {
        _bulletVisualParent.gameObject.SetActive(false);
    }

    private Vector3 HandleImpact(LagCompensatedHit hit)
    {
        if(IntruderHelper.IsInLayerMask(hit.GameObject.layer, _collisionConfiguration.DamageLayer) 
           && hit.Hitbox != null)
            ApplyDamage(hit.Hitbox.Root as IntruderHitboxRoot, ImpactDamage);
        
        Detonate(hit.Point);
        return Vector3.zero;
    }
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector3(_width,1, _length));
    }
#endif
}
