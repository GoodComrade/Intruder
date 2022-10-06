using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameTimeController : NetworkBehaviour
{
    [Networked(OnChanged = nameof(PausedChanged), OnChangedTargets = OnChangedTargets.All)]
    public NetworkBool Paused { get; set; }

    private float _startTime;
    private bool _started = false;
    
    public static void PausedChanged(Changed<GameTimeController> time)
    {
        Time.timeScale = time.Behaviour.Paused ? 0 : 1;
    }

    public override void Spawned()
    {
        base.Spawned();
        _startTime = Runner.SimulationTime;
        _started = true;
    }

    public float GetGameTime()
    {
        if (!_started)
            return 0;
       return Runner.SimulationTime - _startTime;
    }
}
