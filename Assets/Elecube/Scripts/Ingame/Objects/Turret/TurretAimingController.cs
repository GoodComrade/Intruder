using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TurretAimingController : NetworkBehaviour, ICanDie, IPersonDisable
{
    private const float FindTargetInterval = 1.056f;
    [Networked(OnChanged = nameof(OnTargetChanged))] private IntruderHitboxRoot target { get; set; }
    [SerializeField] private CollisionConfiguration _collisionConfiguration;
    [SerializeField] private float _rotationSpeed = 90f;
    [SerializeField] private float _range;
    [SerializeField] private Transform _head;
    
    private List<LagCompensatedHit> _areaTargets = new List<LagCompensatedHit>();
    private TickTimer findTargetCooldown;
    private bool _hasTarget;
    private bool _disabled;
    private TurretShootingController _turretShooting;

    public override void Spawned()
    {
        base.Spawned();
        _turretShooting = GetComponent<TurretShootingController>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(!_disabled && Object.HasStateAuthority && findTargetCooldown.ExpiredOrNotRunning(Runner))
            AimUpdate();
        if(_hasTarget)
        {
            RotateToTarget();
        }
    }

    private void AimUpdate()
    {
        findTargetCooldown = TickTimer.CreateFromSeconds(Runner, FindTargetInterval);
        int collisions = Runner.LagCompensation.OverlapSphere(transform.position, _range, Runner.LocalPlayer,
            _areaTargets,
            _collisionConfiguration.DamageLayer);
        if (collisions > 0)
        {
            target = _areaTargets[0].Hitbox.Root as IntruderHitboxRoot;
            _hasTarget = true;
        }
        else if(_hasTarget)
        {
            target = null;
            _hasTarget = false;
        }
    }

    private void RotateToTarget()
    { 
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;
        direction = direction.normalized;
        _head.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(_head.eulerAngles.y, Quaternion.LookRotation(direction).eulerAngles.y, Runner.DeltaTime * _rotationSpeed), 0);
    }
    
    public static void OnTargetChanged(Changed<TurretAimingController> changed)
    {
        changed.LoadOld();
        if (changed.Behaviour.target != null)
        {
            changed.Behaviour.target.RemoveOnDisable(changed.Behaviour.TargetDisabled);
        }
        changed.LoadNew();
        changed.Behaviour.TargetChanged();
    }

    private void TargetChanged()
    {
        _hasTarget = target != null && target.HitboxRootActive;
        if (_hasTarget)
        {
            target.AddOnDisable(TargetDisabled);
        }
        _turretShooting.SetFiring(_hasTarget);
    }

    private void TargetDisabled()
    {
        if(target != null)
            target.RemoveOnDisable(TargetDisabled);
        target = null;
        _hasTarget = false;
    }

    public void Die()
    {
        TargetDisabled();
        PersonDisable();
    }

    public void PersonDisable()
    {
        TargetDisabled();
        _disabled = true;
    }

    public void PersonEnable()
    {
        _disabled = false;
    }
}
