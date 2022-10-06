using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIndicatorController : MonoBehaviour, IPersonDisable
{
    [SerializeField] private MeshRenderer _circleMeshRenderer;

    private Color _indicatorColor;
    [SerializeField] private Material _playerCircleMaterial;
    [SerializeField] private Material _allyCircleMaterial;
    [SerializeField] private Material _enemyCircleMaterial;

    public void Initialise(CharacterSide type)
    {
        switch (type)
        {
            case CharacterSide.PLAYER:
                _circleMeshRenderer.material = _playerCircleMaterial;
                break;
            case CharacterSide.ENEMY:
                _circleMeshRenderer.material = _enemyCircleMaterial;
                break;
            default:
                _circleMeshRenderer.material = _allyCircleMaterial;
                break; 
        }
        _circleMeshRenderer.enabled = true;
    }

    public void PersonDisable()
    {
        _circleMeshRenderer.enabled = false;
    }

    public void PersonEnable()
    {
        _circleMeshRenderer.enabled = true;
    }
}
