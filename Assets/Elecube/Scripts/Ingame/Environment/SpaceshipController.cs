using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SpaceshipController : IntruderCharacterController, ICanChangeHealth
{
    private static SpaceshipController _instance;
    
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private List<Transform> _collectionPoints;

    public override void Spawned()
    {
        base.Spawned();
    }

    public override Character GetCharacter()
    {
        return Characters.GetInstance().GetSpaceship();
    }

    protected override void Awake()
    {
        base.Awake();
        _instance = this;
    }

    public Transform GetSpawnPoint()
    {
        return _spawnPoint;
    }

    public static SpaceshipController GetInstance()
    {
        return _instance;
    }

    public void HealthChanged(HealthController healthController, float delta)
    {
        GameplayUiController.GetInstance().GetSpaceshipProgressVisual().UpdateVisual(GetComponent<HealthController>());
    }
    
    
    public List<Transform> GetCollectionPoints()
    {
        return _collectionPoints;
    }
}
