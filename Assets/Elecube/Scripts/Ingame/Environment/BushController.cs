using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BushController : MonoBehaviour
{
    private static readonly int Transparency = Shader.PropertyToID("Transparency");
    
    private List<IBushHide> _hideControllers;
    private readonly List<BushRevealController> _revealers = new List<BushRevealController>();
    private bool _isRevealed;
    private float _currentTransparency = 0;

    public bool IsRevealed => _isRevealed;

    public Action OnRevealChanged { get; set; }

    private void Awake()
    {
        _hideControllers = GetComponentsInChildren<IBushHide>(true).ToList();
    }

    public void SetRevealer(BushRevealController controller)
    {
        if(_revealers.Contains(controller))
            return;
        
        _revealers.Add(controller);
    }

    private void FixedUpdate()
    {
        if (_revealers.Count > 0)
        {
            CalculateRevealers();
        }
    }

    private void CalculateRevealers()
    {
        float shortestDistance = float.MaxValue;

        for (int i = _revealers.Count - 1; i >= 0; i--)
        {
            float distance = Vector3.Distance(_revealers[i].transform.position, transform.position);
            if(distance > GameplayConstants.BushVisionRadius * 1.2f || !_revealers[i].IsEnabled())
            {
                _revealers.RemoveAt(i);
                continue;
            }
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
            }
        }
        
        if(shortestDistance < GameplayConstants.BushVisionRadius)
        {
            Reveal(shortestDistance);
        }
        else
        {
            UnReveal();
        }
    }

    private void Reveal(float distance)
    {
        if (!_isRevealed)
        {
            _isRevealed = true;
            if (OnRevealChanged != null)
                OnRevealChanged();
        }
        var transparency = Mathf.Lerp(0.0f, 0.9f,distance / GameplayConstants.BushVisionRadius);
        SetTransparency(transparency);
    }

    public bool IsRevealedByCharacter(IntruderCharacterController character)
    {
        foreach (var revealer in _revealers)
        {
            if (revealer.GetCharacter() == character)
                return true;
        }

        return false;
    }

    private void UnReveal()
    {
        if (_isRevealed)
        {
            _isRevealed = false;
            if (OnRevealChanged != null)
                OnRevealChanged();
            
            SetTransparency(1);
        }
    }

    private void SetTransparency(float transparency)
    {
        if(Math.Abs(_currentTransparency - transparency) < 0.01f)
            return;
        _currentTransparency = transparency;
        foreach (var hide in _hideControllers)
        {
            hide.SetHidePercentage(1f - _currentTransparency);
        }
    }
}
