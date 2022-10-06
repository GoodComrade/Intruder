using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class HitController : NetworkBehaviour
{
    public abstract void Hit(float damage, IntruderCharacterController source);
}
