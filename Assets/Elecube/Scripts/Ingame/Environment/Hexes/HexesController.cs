using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class HexesController : MonoBehaviour
{
    private static HexesController _instance;
    
    [SerializeField] private NavMeshSurface _navMesh;
    
    private List<HexController> _hexes;

    private void Awake()
    {
        _instance = this;
        _hexes = GetComponentsInChildren<HexController>().ToList();
    }

    private void Start()
    {
        _navMesh.BuildNavMesh();
    }
    
    
    public HexController GetClosestHex(Vector3 position)
    {
        HexController bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach(HexController potentialTarget in _hexes)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }

    public static HexesController GetInstance()
    {
        return _instance;
    }

    public void IslandDestroyed()
    {
        _navMesh.BuildNavMesh();
        CharactersController.GetInstance().CheckForFallenCharacters();
        WorkersController.GetInstance().RecalculateWorkerSpawnCount();
    }

    public int GetMineableNotDestroyedHexCount()
    {
        int count = 0;
        foreach (var hex in _hexes)
        {
            if (!hex.IsHexDestroyed() && hex.IsMineable())
                count++;
        }
        return count;
    }
    
    public List<HexController> GetOuterHexes()
    {
        return _hexes.FindAll(hex =>
        {
            return !hex.IsDestroyed() && hex.IsOuterHex();
        });
    }

    public List<HexController> GetAllCritterSpawnableHexes()
    {
        return _hexes.FindAll(hex =>
        {
            return !hex.IsDestroyed() && hex.CanSpawnCritter();
        });
    }
}
