using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CrystalgrabGameController : GameController
{
    private readonly List<Transform> _collectionPoints = new List<Transform>(64);


    public override void Spawned()
    {
        base.Spawned();
        
        _collectionPoints.AddRange(SpaceshipController.GetInstance().GetCollectionPoints());
    }

    public void WorkerDied(AIWorkerController worker, PlayerRef killer)
    {
        if(killer != default)
            WorkersController.GetInstance().TryDeathWorkerAlerting(worker.GetMovingTransform().position);
    }
    
    public List<Transform> GetCollectionPoints()
    {
        return _collectionPoints;
    }

    public static CrystalgrabGameController GetCrystalgrabInstance()
    {
        return (CrystalgrabGameController) _instance;
    }
}
