using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CrittersController : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _critter;

    private const int CritterCount = 10;
    
    public override void Spawned()
    {
        base.Spawned();
        if ((Runner.GameMode == GameMode.Shared && !GameController.GetInstance().IsLocalPlayerMonster())
            || (Runner.GameMode != GameMode.Shared && Object.HasStateAuthority))
        {
            SpawnCritters();
        }
    }

    private void SpawnCritters()
    {
        foreach (var hex in GetCritterHexes())
        {
            SpawnCritter(hex);
        }
    }

    private void SpawnCritter(HexController hex)
    {
        Runner.Spawn(_critter, hex.GetIsland().GetMonsterSpawnPoint(), Quaternion.Euler(0, 0, 0),null, ((runner, o) => BeforeCritterSpawn(o, hex)));
    }
    
    private void BeforeCritterSpawn(NetworkObject charObject, HexController hex)
    {
        charObject.GetComponent<CritterCharacterController>().InitCritterNetworkState(hex);
    }
    
    private List<HexController> GetCritterHexes()
    {
        List<HexController> hexes = new List<HexController>(HexesController.GetInstance().GetAllCritterSpawnableHexes());
        IntruderHelper.ShuffleList(hexes);
        return hexes.GetRange(0, CritterCount);
    }
}
