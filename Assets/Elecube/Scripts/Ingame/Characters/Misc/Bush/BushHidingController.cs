using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

[OrderAfter(typeof(PlayerWrapperController))]
public class BushHidingController : NetworkBehaviour, IPersonDisable, ICanSetCharacter, ICanChangeHealth
{
    private const float BushHideInterval = 0.15f;
    private const float BushRevealTime = 3f;
    private const float HidingSpeed = 3f;
    
    private CharacterSide _side;
    private bool _disabled;
    private float _transparency;

    private List<IBushHide> _hideControllers;

    //used to save performance and calculate only in certain interval
    private TickTimer _bushHideInterval;
    
    private TickTimer _bushRevealedTime;
    
    [Networked(OnChanged = nameof(HideRecalculate))] private NetworkBool IsHidden { get; set; }
    private float _localHideTargetPercentage;
    private float _localHideCurrentPercentage;
    [Networked(OnChanged = nameof(CurrentBushChanged))] private NetworkObject CurrentBush { get; set; }
    private BushController _currentBush;


    private void Awake()
    {
        _hideControllers = GetComponentsInChildren<IBushHide>(true).ToList();
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    public override void FixedUpdateNetwork()
    {
        if(_disabled || !Object.HasStateAuthority)
            return; 
        if(_bushHideInterval.ExpiredOrNotRunning(Runner))
            BushHidingUpdate();
    }

    private void Update()
    {
        if(_disabled)
            return;
            
        CurrentHidePercentageUpdate();
    }


    private void BushHidingUpdate()
    {
        _bushHideInterval = TickTimer.CreateFromSeconds(Runner, BushHideInterval);
        bool hidden = !IsRevealForced() && DetectBush();
        if (hidden != IsHidden)
        {
            IsHidden = hidden;
        }
    }

    private bool IsRevealForced()
    {
        return !_bushRevealedTime.ExpiredOrNotRunning(Runner);
    }

    private void CalculateLocalHidePercentage()
    {
        if(_disabled)
            return;
        _localHideTargetPercentage = Mathf.Clamp(IsHidden ? 1f : 0f, 0, GetMaxHiddenPercentage());
    }
    
    private void CurrentHidePercentageUpdate()
    {
        if(Math.Abs(_localHideCurrentPercentage - _localHideTargetPercentage) < 0.001f)
            return;

        _localHideCurrentPercentage =
            Mathf.MoveTowards(_localHideCurrentPercentage, _localHideTargetPercentage, Time.deltaTime * HidingSpeed);
        
        foreach (var hide in _hideControllers)
        {
            hide.SetHidePercentage(_localHideCurrentPercentage);
        }
    }
    
    public static void HideRecalculate(Changed<BushHidingController> worker)
    {
        worker.Behaviour.CalculateLocalHidePercentage();
    }
    
    public static void CurrentBushChanged(Changed<BushHidingController> worker)
    {
        if(worker.Behaviour._currentBush != null)
            worker.Behaviour._currentBush.OnRevealChanged -= worker.Behaviour.CalculateLocalHidePercentage;
        if (worker.Behaviour.CurrentBush != null)
        {
            worker.Behaviour._currentBush = worker.Behaviour.CurrentBush.GetComponent<BushController>();
            worker.Behaviour._currentBush.OnRevealChanged += worker.Behaviour.CalculateLocalHidePercentage;
        }
        worker.Behaviour.CalculateLocalHidePercentage();
    }

    private float GetMaxHiddenPercentage()
    {
        // Do not fully hide local player.
        if (_side == CharacterSide.PLAYER)
        {
            return 0.6f;
        }
        
        if(IsInLocallyRevealedBush())
        {
            return 0f;
        }
        return 1f;
    }

    private bool IsInLocallyRevealedBush()
    {
        if (_currentBush == null)
            return false;

        if (_currentBush.IsRevealed)
            return true;
        
        return false;
    }
    
    public void ForceReveal()
    {
        _bushRevealedTime = TickTimer.CreateFromSeconds(Runner, BushRevealTime);
    }

    private bool DetectBush()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, -3, 0), Vector3.up, out RaycastHit hit, 6,
            GameplayConstants.Instance.BushLayer, QueryTriggerInteraction.Collide))
        {
            CurrentBush = hit.collider.gameObject.GetComponent<NetworkObject>();
            return true;
        }
        return false;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        PersonDisable();
        if (_currentBush != null)
        {
            _currentBush.OnRevealChanged -= CalculateLocalHidePercentage;
        }
    }

    public bool GetIsHiddenFromCharacter(IntruderCharacterController character)
    {
        if (!IsHidden)
        {
            return false;
        }

        if (_currentBush.IsRevealedByCharacter(character))
        {
            return false;
        }
        return true;
}

    public void PersonEnable()
    {
        _disabled = false;
    }

    public void PersonDisable()
    {
        _disabled = true;
    }

    public void SetCharacter(IntruderCharacterController intruderCharacterController)
    {
        _side = intruderCharacterController.GetSide();
    }

    public void HealthChanged(HealthController healthController, float delta)
    {
        if (delta < 0)
        {
            ForceReveal();
        }
    }
}
