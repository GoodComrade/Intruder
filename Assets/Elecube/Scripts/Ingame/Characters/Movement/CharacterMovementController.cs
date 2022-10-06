using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class CharacterMovementController : NetworkTransform, ICharacterMovement
{
    private List<SpeedModifier> _speedModifiers = new List<SpeedModifier>();
    public abstract bool IsMoving();

    public abstract void SetMovementDisabled(bool disabled);


    protected abstract void SpeedModified();
    public void AddSpeedModifier(float modifier, float duration)
    {
        _speedModifiers.Add(new SpeedModifier(modifier, TickTimer.CreateFromSeconds(Runner, duration)));
        SpeedModified();
    }

    protected float GetSpeedModification()
    {
        float highestModifier = 1.0f;
        float lowestModifier = 1.0f;
        for (int i = _speedModifiers.Count - 1; i >= 0; i--)
        {
            if (_speedModifiers[i]._speed > highestModifier)
                highestModifier = _speedModifiers[i]._speed;
            else
                lowestModifier = _speedModifiers[i]._speed;
        }

        return highestModifier * lowestModifier;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        CheckSpeedModifications();
    }

    private void CheckSpeedModifications()
    {
        bool changed = false;
        for (int i = _speedModifiers.Count - 1; i >= 0; i--)
        {
            if (_speedModifiers[i]._timer.ExpiredOrNotRunning(Runner))
            {
                _speedModifiers.RemoveAt(i);
                changed = true;
            }
        }

        if (changed)
        {
            SpeedModified();
        }
    }

    private struct SpeedModifier
    {
        public float _speed;
        public TickTimer _timer;

        public SpeedModifier(float modifier, TickTimer timer)
        {
            _speed = modifier;
            _timer = timer;
        }
    }
}