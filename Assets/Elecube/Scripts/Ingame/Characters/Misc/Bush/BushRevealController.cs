using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BushRevealController : MonoBehaviour, IPersonDisable, ICanSetCharacter, ICanDie
{
    private const float BushRevealInterval = 0.25f;
    
    private IntruderCharacterController _characterController;
    private bool _disabled;
    private float _lastBushReveal;
    
    //used to avoid allocation during runtime
    private Collider[] _overlapResults = new Collider[(int) (Mathf.Pow(GameplayConstants.BushVisionRadius, 2) * 2)];
    private void FixedUpdate()
    {
        if(_disabled)
            return;
        
        if (_characterController.GetSide() == CharacterSide.PLAYER && _lastBushReveal + BushRevealInterval < Time.fixedTime)
        {
            _lastBushReveal = Time.fixedTime;
            BushRevealUpdate();
        }
    }
    
    
    private void BushRevealUpdate()
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, GameplayConstants.BushVisionRadius, _overlapResults, GameplayConstants.Instance.BushLayer, QueryTriggerInteraction.Collide);

        for (int i = 0; i < size; i++)
        { 
            _overlapResults[i].GetComponent<BushController>().SetRevealer(this);
        }
    }

    public bool IsEnabled()
    {
        return !_disabled && enabled;
    }

    public void PersonEnable()
    {
        _disabled = false;
    }

    public void PersonDisable()
    {
        _disabled = true;
    }

    public void SetCharacter(IntruderCharacterController intruderCharacterController)
    {
        _characterController = intruderCharacterController;
    }

    public IntruderCharacterController GetCharacter()
    {
        return _characterController;
    }

    public void Die()
    {
        this.enabled = false;
    }
}
