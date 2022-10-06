using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class CrystalController : MonoBehaviour, IIslandDegradationListener
{
    private List<CrystalMiningSpot> _miningPoints;
    private IslandController _parentIsland;

    private bool _minedOut = false;

    private void Awake()
    {
        _miningPoints = GetComponentsInChildren<CrystalMiningSpot>().ToList();
        foreach (var miningPoint in _miningPoints)
        {
            miningPoint.SetParentCrystal(this);
        }
    }

    public bool CanBeMined()
    {
        return !_parentIsland.IsMiningLocked() && !_minedOut;
    }

    public List<CrystalMiningSpot> GetMiningSpots()
    {
        return _miningPoints;
    }
    
    public bool IsComingWorkerCapacityFull()
    {
        int workers = 0;
        foreach (var spot in _miningPoints)
        {
            if (spot.HasWorker())
            {
                workers++;
            }
        }
        return workers >= _miningPoints.Count();
    }

    public void MiningDone()
    {
        _parentIsland.CrystalMined();
    }

    public void SetParentIsland(IslandController island)
    {
        _parentIsland = island;
    }

    public void SetIslandDegradationPercentage(float percentage)
    {
        _minedOut = percentage >= 1f;
    }

    public void SetMiningLocked(bool on)
    {
    }
}