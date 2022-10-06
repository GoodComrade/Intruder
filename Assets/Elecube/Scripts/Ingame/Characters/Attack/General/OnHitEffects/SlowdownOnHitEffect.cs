using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Intruder/On-Hit Effects/Slowdown")]
public class SlowdownOnHitEffect : OnHitEffect
{
    [SerializeField] [Range(0.01f, 1f)] private float _slowDownPercentage;
    [SerializeField] private float _duration;
    public override void ApplyEffect(Bullet bullet, IntruderHitboxRoot other)
    {
        other.GetCharacter().GetMovement()?.AddSpeedModifier(1f - _slowDownPercentage, _duration);
    }
}
