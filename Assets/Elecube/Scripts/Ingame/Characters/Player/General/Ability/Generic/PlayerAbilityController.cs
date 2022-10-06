using System;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

public abstract class PlayerAbilityController : AbilityController, ICanDisableAttack, ICanDie
{
    [SerializeField] private AbilityType _type;
    [SerializeField] private float _cooldown;
    [SerializeField] protected PlayerAimSettings _aimSettings;
    private TickTimer _cooldownReset;
    
    protected bool _isAbilityPressed = false;
    protected bool _isDisabled = false;

    public override void Spawned()
    {
        base.Spawned();
        if (IsLocalPlayer())
        {
            CooldownReset();
            SetupAbilityButton();
        }
    }

    protected virtual void SetupAbilityButton()
    {
        switch (_type)
        {
            case AbilityType.ABILITY_1: 
                GameplayUiController.GetInstance().GetControls().SetupAbility1Button(_aimSettings);
                GameplayUiController.GetInstance().GetControls().SetAbility1Enabled(true);
                break;
            case AbilityType.ABILITY_2: 
                GameplayUiController.GetInstance().GetControls().SetupAbility2Button(_aimSettings);
                GameplayUiController.GetInstance().GetControls().SetAbility2Enabled(true);
                break;
            case AbilityType.ABILITY_3:
                throw new NotImplementedException();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if(_isDisabled)
            return;
        base.FixedUpdateNetwork();

        if(Object.HasInputAuthority || Object.HasStateAuthority)
            QueryInput();
        if(IsLocalPlayer())
            CooldownUpdate();
    }

    private void QueryInput()
    {
        if (GetInput(out NetworkInputMaster data))
        {
            ProcessInput(data);
        }
    }
    
    protected virtual void ProcessInput(NetworkInputMaster data)
    {
        AbilityPressed(IsAbilityPressed(data));
    }

    protected bool IsAbilityPressed(NetworkInputMaster data)
    {
        switch (_type)
        {
            case AbilityType.WEAPON: 
                return data.shoot;
            case AbilityType.ABILITY_1: 
                return data.ability1;
            case AbilityType.ABILITY_2: 
                return data.ability2;
            case AbilityType.ABILITY_3: 
                return data.ability3;
        }
        throw new Exception("unknown ability type - " + _type);
    }

    protected virtual void AbilityPressed(bool on)
    {
        if(_isAbilityPressed == on)
            return;
        _isAbilityPressed = on;

        if (!on)
        {
            DoAbility();
        }
    }

    private void CooldownUpdate()
    {
        float progress = _cooldownReset.ExpiredOrNotRunning(Runner) ? 1f : (_cooldown - _cooldownReset.RemainingTime(Runner).Get<float>()) / _cooldown;
        switch (_type)
        {
            case AbilityType.ABILITY_1: 
                GameplayUiController.GetInstance().GetControls().SetAbility1Cooldown(progress);
                break;
            case AbilityType.ABILITY_2: 
                GameplayUiController.GetInstance().GetControls().SetAbility2Cooldown(progress);
                break;
            case AbilityType.ABILITY_3: 
                GameplayUiController.GetInstance().GetControls().SetAbility3Cooldown(progress);
                break;
        }
    }

    private void CooldownReset()
    {
        _cooldownReset = TickTimer.CreateFromSeconds(Runner, _cooldown);
    }

    public virtual void DoAbility()
    {
        CooldownReset();
    }

    public virtual void SetAttackAndAbilityDisabled(bool disabled)
    {
        _isDisabled = disabled;
    }

    public void Die()
    {
        _isDisabled = true;
    }
}

public enum AbilityType
{
    WEAPON, ABILITY_1, ABILITY_2, ABILITY_3
}
