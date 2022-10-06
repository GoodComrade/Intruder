using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TurretShootingController : AbilityController
{
    [Networked(OnChanged = nameof(OnFireTickChanged))]
    private TickTimer FireCooldown { get; set; }

    [SerializeField] private float _shootInterval = 0.3f;
    [SerializeField] private List<Transform> _gunExits;

    private bool _firing;
    private int _currentGunExit;
    
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasStateAuthority || Object.HasInputAuthority)
        {
            FireUpdate();
        }
    }

    public void SetFiring(bool firing)
    {
        _firing = firing;
    }
    
    private void FireUpdate()
    {
        if (_firing && FireCooldown.ExpiredOrNotRunning(Runner))
        {
            Fire();
        }
    }
    
    private void Fire()
    {
        FireCooldown = TickTimer.CreateFromSeconds(Runner, _shootInterval);
        Transform exit = GetExitPoint();
        SpawnNetworkProjectile(Runner.LocalPlayer, new Vector3(exit.position.x, GameplayConstants.ProjectileAltitude, exit.position.z), exit.rotation, Vector3.zero, Vector3.zero); 
    }
    
    public static void OnFireTickChanged(Changed<TurretShootingController> changed)
    {
        changed.Behaviour.FireFx();
    }
    
    private void FireFx()
    {
    }
    
    protected virtual Transform GetExitPoint()
    {
        _currentGunExit++;
        if (_currentGunExit >= _gunExits.Count)
            _currentGunExit = 0;
        return _gunExits[_currentGunExit];
    }
}
