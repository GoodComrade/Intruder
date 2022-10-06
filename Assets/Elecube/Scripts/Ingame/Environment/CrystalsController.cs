using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrystalsController : MonoBehaviour
{
    private static CrystalsController _instance;

    private List<CrystalController> _allCrystals = new List<CrystalController>();
    public static CrystalsController GETInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
        PopulateCrystals();
    }

    private void PopulateCrystals()
    {
        _allCrystals.AddRange(GetComponentsInChildren<CrystalController>().ToList());
    }

    public bool GetCrystalToAIMine(AIWorkerController worker, out CrystalMiningSpot crystalMiningSpot, float maxRange)
    {
        if (GetRandomCrystalToAiMine(worker, out crystalMiningSpot, maxRange))
        {
            return true;
        }

        if (maxRange < float.PositiveInfinity)
        {
            return GetRandomCrystalToAiMine(worker, out crystalMiningSpot, float.PositiveInfinity);
        }

        return false;
    }

    private bool GetRandomCrystalToAiMine(AIWorkerController worker, out CrystalMiningSpot crystalMiningSpot, float maxRange)
    {
        IntruderHelper.ShuffleList(_allCrystals);
        foreach (var crystal in _allCrystals)
        {
            if(crystal.IsComingWorkerCapacityFull() || !crystal.CanBeMined())
                continue;
            foreach (var s in crystal.GetMiningSpots())
            {
                if (s.HasWorker())
                    continue;
                if(Vector3.Distance(worker.GetMovingTransform().position, s.transform.position) > maxRange)
                    continue;
                if (!worker.GetMovementController().CanGoTo(s.transform.position))
                    continue;
                crystalMiningSpot = s;
                return true;
            }
        }
        crystalMiningSpot = null;
        return false;
    }

    public CrystalMiningSpot GetClosestCrystalToMine(Transform worker)
    {
        float shortestDistance = float.MaxValue;
        CrystalMiningSpot closestCrystalSpot = null;
        foreach (var crystal in _allCrystals)
        {
            if (!crystal.CanBeMined())
                continue;
            float distance = Vector3.Distance(crystal.transform.position, worker.position);
            if (shortestDistance < distance)
            {
                continue;
            }

            if (GetClosestMiningSpotInCrystal(worker, crystal, out var spot))
            {
                shortestDistance = distance;
                closestCrystalSpot = spot;
            }
        }

        return closestCrystalSpot;
    }

    private bool GetClosestMiningSpotInCrystal(Transform place, CrystalController crystal, out CrystalMiningSpot spot)
    {
        spot = null;
        float shortestDistance = float.MaxValue;
        foreach (var s in crystal.GetMiningSpots())
        {
            if (s.HasWorker())
                continue;
            float distance = Vector3.Distance(place.position, s.transform.position);
            if (shortestDistance > distance)
            {
                shortestDistance = distance;
                spot = s;
            }
        }

        return shortestDistance < float.MaxValue;
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}