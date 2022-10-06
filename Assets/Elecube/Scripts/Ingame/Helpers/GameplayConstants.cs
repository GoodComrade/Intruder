using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayConstants : Singleton<GameplayConstants>
{
    protected override void OnAwake()
    {
        base.OnAwake();
        BushLayer = LayerMask.GetMask("Bush");
    }

    public static readonly float EvolveDuration = 4f;
    public static readonly float SpaceshipSpawnInterval = 1f;
    public static readonly float MiningRange = 1.75f;
    public static readonly float MiningTime = 5f;
    public static readonly float VisualRange = 20f;
    public static readonly float MonsterScareRange = 15f;
    public static readonly float MonsterScareRangeInBush = 1f;
    public static readonly float MonsterScareDuration = 3.5f;
    public static readonly float IslandDestructionTime = 7f;
    public static readonly int WorkersPerIsland = 3;
    public static readonly float BushVisionRadius = 5f;
    public static readonly float WorkerAlertCooldown = 7f;
    public static readonly float ProjectileAltitude = 1.25f;

    public static readonly float LevelUpModifier = 1.05f;
    public static readonly float EvolveStatModifier = 1.4f;
    
    public static readonly float HunterEatenHealPercentage = 0.1f;
    public static readonly float HunterEatenHealFlat = 350f;
    
    public static readonly int WorkerLayer = 14;
    public int BushLayer;

    public static ushort GetStatForLevel(float stat, int lvl)
    {
        return (ushort) (stat * Mathf.Pow(LevelUpModifier, lvl - 1));
    }

    public static float GetPowerStatMultiplier(int currentPower, int targetPower)
    {
        float currentModifier = GetPowerStatMultiplier(currentPower);
        float targetModifier = GetPowerStatMultiplier(targetPower);
        return targetModifier / currentModifier;
    }

    public static float GetPowerStatMultiplier(int targetPower)
    {
        return 1 + ((EvolveStatModifier - 1f) * targetPower);
    }
}
