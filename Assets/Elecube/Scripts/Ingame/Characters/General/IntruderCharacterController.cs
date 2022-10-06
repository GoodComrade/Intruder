using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public abstract class IntruderCharacterController : NetworkBehaviour, ICanDie
{
    private CharacterSide _side;
    protected bool _dead = false;
    protected bool _fallenFromIsland = false;
    private HexController _currentHex;
    private Vector3 _lastPosition;
    private TickTimer despawnTimer { get; set; }
    private Action<IntruderCharacterController> _onDeathAction;

    [SerializeField] private GameObject _visualRoot;
    [SerializeField] private NetworkTransform _networkTransform;
    [SerializeField] protected CharacterIndicatorController _characterIndicator;
    [SerializeField] private HealthbarController _healthbarController;

    protected CharacterAnimationsController _animationsController;
    private ICharacterMovement _movement;
    private ICanDisableAttack[] _attackDisableListeners;
    private BushHidingController _bushHidingController;
    private bool _hasBushHidingController;

    public virtual void InvokeDeath(PlayerRef source)
    {
        Debug.Log(this.gameObject.name + " died.");
        foreach (var deathListener in GetComponentsInChildren<ICanDie>(true))
        {
            deathListener.Die();
        }
    }

    protected virtual void Awake()
    {
        _movement = GetComponentInChildren<ICharacterMovement>();
        _attackDisableListeners = GetComponentsInChildren<ICanDisableAttack>();
        _animationsController = GetComponentInChildren<CharacterAnimationsController>();
        _bushHidingController = GetComponentInChildren<BushHidingController>();
        _hasBushHidingController = _bushHidingController != null;
    }

    public override void Spawned()
    {
        base.Spawned();
        DecideSide();
        SetupCharacterIndicator();
        InitialiseCharacterBehaviours();
        CharactersController.GetInstance().AddCharacter(this);
        UpdateCurrentHex();
    }

    protected virtual void DecideSide()
    {
        if (GetControllingPlayer().PlayerId == Runner.LocalPlayer)
        {
            _side = CharacterSide.PLAYER;
        }
        else if (GetCharacter().GetCharacterType() is CharacterType.WORKER or CharacterType.TURRET or CharacterType.HUNTER && !GameController.GetInstance().IsLocalPlayerMonster())
        {
            _side = CharacterSide.ALLY;
        }
        else if (GetCharacter().GetCharacterType() == CharacterType.CRITTER && GameController.GetInstance().IsLocalPlayerMonster())
        {
            _side = CharacterSide.ALLY;
        }
        else
        {
            _side = CharacterSide.ENEMY;
        }
    }

    public virtual PlayerRef GetControllingPlayer()
    {
        return -1;
    }

    public abstract Character GetCharacter();

    protected virtual void InitialiseCharacterBehaviours()
    {
        foreach (var setter in GetComponentsInChildren<ICanSetCharacter>(true))
        {
            setter.SetCharacter(this);
        }
    }

    public NetworkTransform GetNetworkTransform()
    {
        return _networkTransform;
    }

    protected virtual void SetupCharacterIndicator()
    {
        if (_characterIndicator == null)
            return;

        _characterIndicator.Initialise(GetSide());
    }

    public CharacterSide GetSide()
    {
        return _side;
    }
    
    public override void FixedUpdateNetwork()
    {
        DespawnUpdate();
        HexUpdate();
    }

    public virtual PlayerWeaponController GetWeaponController()
    {
        return GetComponentInChildren<PlayerWeaponController>();
    }

    public void KillCharacter()
    {
        GetComponent<HealthController>().Kill(-1);
    }

    public virtual void FallFromIsland()
    {
        _fallenFromIsland = true;
        if (!_dead)
        {
            KillCharacter();   
        }
    }

    public HexController GetCurrentHex()
    {
        return _currentHex;
    }

    public virtual void Die()
    {
        _dead = true;
        despawnTimer = TickTimer.CreateFromSeconds(Runner, 2);
        _animationsController.SetDead(true);
        if(_onDeathAction != null)
            _onDeathAction(this);
    }

    public void AddOnDeathAction(Action<IntruderCharacterController> action)
    {
        _onDeathAction += action;
    }

    public void RemoveOnDeathAction(Action<IntruderCharacterController> action)
    {
        _onDeathAction -= action;
    }

    public bool IsDead()
    {
        return _dead;
    }

    public virtual bool IsEatable()
    {
        return false;
    }

    public virtual void CharacterEaten(IntruderCharacterController eater)
    {
        throw new NotImplementedException();
    }

    protected virtual void DieFinal()
    {
        CharactersController.GetInstance().RemoveCharacter(this);
        if (Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    public virtual int GetPowerUps()
    {
        return 0;
    }

    private void DespawnUpdate()
    {
        if (despawnTimer.Expired(Runner))
        {
            DieFinal();
        }
    }

    private void HexUpdate()
    {
        if(_dead)
            return;
        if(GetNetworkTransform().ReadPosition() == _lastPosition)
            return;
        _lastPosition = GetNetworkTransform().ReadPosition();
        UpdateCurrentHex();
    }

    private void UpdateCurrentHex()
    {
        _currentHex = HexesController.GetInstance().GetClosestHex(GetNetworkTransform().ReadPosition());
    }

    public HealthbarController GetHealthBar()
    {
        return _healthbarController;
    }

    public HealthController GetHealthController()
    {
        return GetComponent<HealthController>();
    }

    public virtual void SetAttackedByCritter(CritterCharacterController critter, bool on)
    {
        
    }
    
    public virtual void DoRevealAction()
    {
        _bushHidingController.ForceReveal();
    }

    public bool IsHiddenFromCharacter(IntruderCharacterController character)
    {
        if (!_hasBushHidingController)
            return false;
        return _bushHidingController.GetIsHiddenFromCharacter(character);
    }

    public ICharacterMovement GetMovement()
    {
        return _movement;
    }
    
    protected void SetAttackAndAbilityDisabled(bool disabled)
    {
        foreach (var listener in _attackDisableListeners)
        {
            listener.SetAttackAndAbilityDisabled(disabled);
        }
    }
}