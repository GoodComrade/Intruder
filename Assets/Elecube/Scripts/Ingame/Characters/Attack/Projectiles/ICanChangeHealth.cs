using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// Used to implement logic related to changing current and max health - taking damage, healing, powerups etc.
/// </summary>
public interface ICanChangeHealth
{
    void HealthChanged(HealthController healthController, float delta);

}