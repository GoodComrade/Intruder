using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsUIController : MonoBehaviour
{
    [SerializeField] private EvolveControl _evolveControl;
    [SerializeField] private AbilityControl _ability1Control;
    [SerializeField] private AbilityControl _ability2Control;
    [SerializeField] private AbilityControl _ability3Control;

    public void SetupAbility1Button(PlayerAimSettings settings)
    {
        _ability1Control.Setup(settings);
    }

    public void SetAbility1Cooldown(float progress)
    {
        _ability1Control.SetCooldown(progress);
    }

    public void SetAbility1Enabled(bool e)
    {
        _ability1Control.SetEnabled(e);
    }
    
    public void SetupAbility2Button(PlayerAimSettings settings)
    {
        _ability2Control.Setup(settings);
    }

    public void SetAbility2Cooldown(float progress)
    {
        _ability2Control.SetCooldown(progress);
    }

    public void SetAbility2Enabled(bool e)
    {
        _ability2Control.SetEnabled(e);
    }
    
    public void SetupAbility3Button(PlayerAimSettings settings)
    {
        _ability3Control.Setup(settings);
    }

    public void SetAbility3Cooldown(float progress)
    {
        _ability3Control.SetCooldown(progress);
    }

    public void SetAbility3Enabled(bool e)
    {
        _ability3Control.SetEnabled(e);
    }


    public void SetEvolveEnabled(bool e)
    {
        _evolveControl.SetEnabled(e);
    }

    public void SetEvolveProgress(float progress)
    {
        _evolveControl.SetEvolveProgress(progress);
    }
}
