using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HexController : NetworkBehaviour
{
    private int _crystalCount = 40;
    [SerializeField] private List<HexController> _outerHexes;
    [SerializeField] private bool _canSpawnCritter = true;
    private readonly List<HexController> _innerHexes = new List<HexController>();

    private IslandController _islandController;

    [Networked] private bool islandDestroyed { get; set; }
    private bool _mineable = false;
    private bool _spawned = false;

    private void Awake()
    {
        foreach (var hex in _outerHexes)
        {
            hex.AddInnerHex(this);
        }
    }

    private void Start()
    {
        _islandController = GetComponentInChildren<IslandController>();
        _islandController.SetParentHex(this);
        DecideMineable();
    }

    public override void Spawned()
    {
        base.Spawned();
        _spawned = true;
    }

    private void DecideMineable()
    {
        if (_mineable)
            return;
        _mineable = ResolveMineable();
        _islandController.SetMiningLocked(!_mineable);
    }

    private bool ResolveMineable()
    {
        if (_outerHexes.Count == 0)
            return true;
        bool hasDestroyedOuterHex = false;
        
        foreach (var hex in _outerHexes)
        {
            if (hex.IsDestroyed())
                hasDestroyedOuterHex = true;
            else if (hex.GetNotDestroyedNotMineableInnerHexCount() == 1)
            {
                return false;
            }
        }
        return hasDestroyedOuterHex;
    }

    public bool IsMineable()
    {
        return _mineable;
    }

    public bool IsHexDestroyed()
    {
        return islandDestroyed;
    }

    public int GetCrystalCount()
    {
        return _crystalCount;
    }

    public void IslandDestroyed()
    {
        islandDestroyed = true;
        foreach (var hex in _innerHexes)
        {
            hex.DecideMineable();
        }
        HexesController.GetInstance().IslandDestroyed();
    }

    public bool IsDestroyed()
    {
        if (!_spawned)
            return false;
        return islandDestroyed;
    }

    public bool IsOuterHex()
    {
        foreach (var hex in _outerHexes)
        {
            if (!hex.islandDestroyed)
                return false;
        }
        return true;
    }

    public IslandController GetIsland()
    {
        return _islandController;
    }

    private void AddInnerHex(HexController hex)
    {
        _innerHexes.Add(hex);
    }

    private int GetNotDestroyedNotMineableInnerHexCount()
    {
        int i = 0;
        foreach (var hex in _innerHexes)
        {
            if (!hex.IsDestroyed() && !hex._mineable)
                i++;
        }
        return i;
    }

    public bool CanSpawnCritter()
    {
        return _canSpawnCritter;
    }
}
