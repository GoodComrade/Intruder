using Fusion;
using UnityEngine;


[DisallowMultipleComponent]
[OrderAfter(typeof(IntruderCharacterController))]
public class PlayerWeaponController : PlayerAimedAbilityController, ICanDie, ICanDisableAttack
{ 
    [SerializeField] private Transform _gunExit;
    [SerializeField] private Vector3[] _gunExitRotationOffsets;
    [SerializeField] private WeaponChargeSettings _chargeSettings;
    [SerializeField] private PlayerCharacterMovementController _movementController;
    [SerializeField] private BushHidingController _bushHiding;
    
    [Networked]
    private TickTimer StartFireCooldown { get; set; }
    [Networked(OnChanged = nameof(OnFireTickChanged))]
    private TickTimer FireCooldown { get; set; }
    [Networked, Accuracy(0.0001)]
    private float CurrentCharge { get; set; }
    [Networked]
    private byte ShotsToBeFired { get; set; }


    public float currentCharge => CurrentCharge;
    public int maxCharges => _chargeSettings.MaxCharges;

    private bool _dead;

    public override void DoAbility()
    {
        StartFire();
    }

    private void StartFire()
    {
        if(!CanStartFire())
            return;
        StartFireCooldown = TickTimer.CreateFromSeconds(Runner, _chargeSettings.Cooldown);
        CurrentCharge -= 1f;
        ShotsToBeFired += _chargeSettings.ShotsPerCharge;
        _bushHiding.ForceReveal();
        if(_intruderCharacterController.GetSide() == CharacterSide.PLAYER)
            CameraController.GetInstance().Shake();
    }

    private void Fire()
    {
        if(_chargeSettings.ShotsInterval > 0)
            FireCooldown = TickTimer.CreateFromSeconds(Runner, _chargeSettings.ShotsInterval);
        Transform exit = GetExitPoint();
        SpawnNetworkProjectile(Runner.LocalPlayer, new Vector3(exit.position.x, GameplayConstants.ProjectileAltitude, exit.position.z), 
            (_gunExitRotationOffsets != null && _gunExitRotationOffsets.Length > 0) ? _gunExit.rotation * Quaternion.Euler(_gunExitRotationOffsets[ShotsFired % _gunExitRotationOffsets.Length]) : _gunExit.rotation, 
            _movementController.Velocity, _aimTarget); 
    }

    private void FireUpdate()
    {
        while (HasShotToFire() && FireCooldown.ExpiredOrNotRunning(Runner))
        {
            Fire();
        }
    }

    private bool HasShotToFire()
    {
        if (ShotsToBeFired > ShotsFired)
            return true;
        //byte overlow workaround - better networking performance
        if (ShotsFired > ShotsToBeFired + byte.MaxValue / 2)
            return true;

        return false;
    }

    public bool HasCharge()
    {
        return CurrentCharge >= 1f;
    }

    public bool CanStartFire()
    {
        if (_isDisabled || _dead)
            return false;
        if (!HasCharge())
            return false;
        if (!StartFireCooldown.ExpiredOrNotRunning(Runner))
            return false;

        return true;
    }

    protected override void IndicateAim(Vector2 aim)
    {
        AdjustAimTargetUsingCollision(aim);
        _playerAbilityIndicator.AimAtTarget(_aimTarget, CanStartFire()); 
    }


    private void FireFx()
    {
    }
    
    public static void OnFireTickChanged(Changed<PlayerWeaponController> changed)
    {
        changed.Behaviour.FireFx();
    }
    
    
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (_dead)
        {
            return;
        }
        if (Object.HasStateAuthority)
        {
            ChargeWeapon();
        }

        if (Object.HasStateAuthority || Object.HasInputAuthority)
        {
            FireUpdate();
        }
    }

    private void ChargeWeapon()
    {
        if (CurrentCharge < _chargeSettings.MaxCharges)
        {
            CurrentCharge += Runner.DeltaTime / _chargeSettings.ChargeTime;
            if (CurrentCharge > _chargeSettings.MaxCharges)
            {
                CurrentCharge = _chargeSettings.MaxCharges;
            }
        }
    }

    protected virtual Transform GetExitPoint()
    {
        return _gunExit;
    }

    public void Die()
    {
        ShotsToBeFired = ShotsFired;
        _dead = true;
    }

    public override void SetAttackAndAbilityDisabled(bool disabled)
    {
        base.SetAttackAndAbilityDisabled(disabled);
        if (disabled)
        {
            ShotsToBeFired = ShotsFired;
        }
    }
}
