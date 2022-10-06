using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnpointsController : MonoBehaviour
{
    public Vector3 GetPlayerSpawnPoint(bool monster)
    {
        if (monster)
        {
            return GetRandomMonsterSpawnPoint();
        }
        else
        {
            return SpaceshipController.GetInstance().GetSpawnPoint().position;
        }
    }
    
    public Vector3 GetRandomMonsterSpawnPoint()
    {
        var hexes = HexesController.GetInstance().GetOuterHexes();
        return hexes[Random.Range(0, hexes.Count)].GetIsland().GetMonsterSpawnPoint();
    }
}
