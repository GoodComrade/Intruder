using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class WorkersController : NetworkBehaviour
{
    private static WorkersController _instance;

    private readonly List<AIWorkerController> _aiWorkerControllers = new List<AIWorkerController>();
    
    [SerializeField] private GameObject _workerDiedAlertPrefab;

    private TickTimer workerSpawnTimer { get; set; }
    private int workersToSpawn { get; set; }
    private TickTimer workerAlertCooldown { get; set; }

    private void Awake()
    {
        _instance = this;
    }

    public static WorkersController GetInstance()
    {
        return _instance;
    }

    public void AddWorker(AIWorkerController worker)
    {
        GetWorkers().Add(worker);
    }
    
    public void SpawnWorkers()
    {
        RecalculateWorkerSpawnCount();
        workerSpawnTimer = TickTimer.CreateFromSeconds(Runner, GameplayConstants.SpaceshipSpawnInterval);
    }

    private void SpawnSpaceshipWorker()
    {
        Runner.Spawn(Characters.GetInstance().GetWorker().CharacterPrefab, SpaceshipController.GetInstance().GetSpawnPoint().position, Quaternion.Euler(0, 0, 0),
            Runner.LocalPlayer, ((r, worker) => BeforeWorkerSpawn(r, worker)));
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (Object.HasStateAuthority && workersToSpawn > 0 && workerSpawnTimer.ExpiredOrNotRunning(Runner))
        {
            workersToSpawn--;
            workerSpawnTimer = TickTimer.CreateFromSeconds(Runner, GameplayConstants.SpaceshipSpawnInterval);
            SpawnSpaceshipWorker();
        }
    }

    private void BeforeWorkerSpawn(NetworkRunner runner, NetworkObject worker)
    {
    }

    public void RemoveWorker(AIWorkerController worker)
    {
        GetWorkers().Remove(worker);
        RecalculateWorkerSpawnCount();
    }

    public void RecalculateWorkerSpawnCount()
    {
        workersToSpawn = HexesController.GetInstance().GetMineableNotDestroyedHexCount() *
                         GameplayConstants.WorkersPerIsland - GetWorkerCount();
    }

    public List<AIWorkerController> GetWorkers()
    {
        return _aiWorkerControllers;
    }

    public int GetWorkerCount()
    {
        return GetWorkers().Count;
    }
    
    public void TryDeathWorkerAlerting(Vector3 alertPosition)
    {
        if (Object.HasStateAuthority && IsWorkerDeathAlertOffCooldown())
        {
            Alert(alertPosition);
        }
    }

    public void ScareWorkersOutOfHex(HexController hexController)
    {
        if(!Object.HasStateAuthority)
            return;
        foreach (var worker in GetWorkers())
        {
            if (worker.GetCurrentHex() == hexController)
            {
                worker.ScareToPosition(Vector3.zero);
            }
        }
    }

    private void Alert(Vector3 alertPosition)
    {
        RPC_DoAlert(alertPosition);
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_DoAlert(Vector3 alertPosition, RpcInfo info = default)
    {
        StartCooldown();
        
        if(GameController.GetInstance().IsLocalPlayerMonster())
            return;

        Instantiate(_workerDiedAlertPrefab, alertPosition, Quaternion.identity);
    }

    private void StartCooldown()
    {
        workerAlertCooldown = TickTimer.CreateFromSeconds(Runner, GameplayConstants.WorkerAlertCooldown);
    }

    private bool IsWorkerDeathAlertOffCooldown()
    {
        return workerAlertCooldown.ExpiredOrNotRunning(Runner);
    }
}