using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class MonsterPlayerWrapperController : PlayerWrapperController, ICanChangeHealth
{
    [Networked(OnChanged = nameof(OnMeatEatenChange))]
    private ushort meatEaten { get; set; }
    
    [Networked(OnChanged = nameof(OnEvolveStageChange))]
    private ushort evolveStage { get; set; }


    public override void Spawned()
    {
        base.Spawned();
        if (playerCharacterController.GetSide() == CharacterSide.PLAYER)
        {
            GameplayUiController.GetInstance().GetControls().SetEvolveEnabled(true);
        }
    }

    protected override void CharacterChanged()
    {
        base.CharacterChanged();
        if (evolveStage > 0)
        {
            EvolveStageChanged();
        }
        playerCharacterController.GetHealthController().AddHealthChangeListener(this);
    }

    public void AddMeatEaten()
    {
        if(meatEaten < GetMeatCountForEvolve())
            meatEaten++;
    }

    public void RemoveMeatEaten()
    {
        meatEaten = 0;
    }

    public void FinishEvolve()
    {
        evolveStage++;
    }

    private int GetMeatCountForEvolve()
    {
        return 3 + evolveStage * 2;
    }

    public bool IsReadyToEvolve()
    {
        return GetMeatCountForEvolve() <= meatEaten;
    }

    public static void OnMeatEatenChange(Changed<MonsterPlayerWrapperController> changed)
    {
        changed.Behaviour.MeatEatenChanged();
    }
    
    public static void OnEvolveStageChange(Changed<MonsterPlayerWrapperController> changed)
    {
        changed.Behaviour.EvolveStageChanged();
    }

    private void MeatEatenChanged()
    {
        if (playerCharacterController.GetSide() == CharacterSide.PLAYER)
        {
            GameplayUiController.GetInstance().GetControls().SetEvolveProgress(((float)meatEaten) / GetMeatCountForEvolve());
        }
    }

    private void EvolveStageChanged()
    {
        ((MonsterPlayerCharacterController)playerCharacterController).SetEvolveStage(evolveStage);
    }

    public void HealthChanged(HealthController healthController, float delta)
    {
        GameplayUiController.GetInstance().GetMinsterHealthProgressVisual().SetMonsterHealth((int) healthController.currentHealth, (int) healthController.MaximumHealth);
    }
}