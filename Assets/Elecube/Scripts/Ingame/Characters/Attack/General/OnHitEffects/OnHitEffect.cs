using UnityEngine;

public abstract class OnHitEffect : ScriptableObject
{
    public abstract void ApplyEffect(Bullet bullet, IntruderHitboxRoot other);
}
