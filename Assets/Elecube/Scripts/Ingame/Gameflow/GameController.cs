using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    protected static GameController _instance;

    [SerializeField] private NetworkPrefabRef _hunterWrapper;
    [SerializeField] private NetworkPrefabRef _monsterWrapper;
    [SerializeField] private SpawnpointsController _spawnpoints;

    private GameTimeController _gameTimeController;
    private bool _spawnedCharacters = false;

    private void Awake()
    {
        Debug.Log("GameController Awake.");
        _instance = this;
        _gameTimeController = GetComponent<GameTimeController>();
    }

    public override void Spawned()
    {
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!_spawnedCharacters)
        {
            SpawnCharacters();
        }
    }

    private void SpawnCharacters()
    {
        _spawnedCharacters = true;
        SpawnLocalPlayer();
        if (Object.HasStateAuthority)
        {
            WorkersController.GetInstance().SpawnWorkers();
        }

        if (Runner.GameMode == GameMode.Single)
        {
            SpawnEnemyAIPlayer();
        }
    }

    private void SpawnLocalPlayer()
    {
        SpawnLocalPlayer(Runner.LocalPlayer,
            IsLocalPlayerMonster()
                ? ProgressManager.Instance.GetCurrentMonster()
                : ProgressManager.Instance.GetCurrentHunter());
    }
    
    private void SpawnEnemyAIPlayer()
    {
        SpawnEnemyAiPlayer(!IsLocalPlayerMonster()
                ? ProgressManager.Instance.GetCurrentMonster()
                : ProgressManager.Instance.GetCurrentHunter());
    }
    

    public Vector3 GetPlayerSpawnPoint(Character character)
    {
        return _spawnpoints.GetPlayerSpawnPoint(character.GetCharacterType() == CharacterType.MONSTER);
    }

    private void SpawnLocalPlayer(PlayerRef player, Character character)
    {
        var playerObject = Runner.Spawn(character.GetCharacterType() == CharacterType.MONSTER ? _monsterWrapper : _hunterWrapper, Vector3.zero, Quaternion.Euler(0, 0, 0),
            player, ((runner, o) => BeforePlayerSpawn(player, o, character)));
        Runner.SetPlayerObject(player, playerObject);
        CharactersController.GetInstance().RPC_AddPlayer(playerObject.GetComponent<PlayerWrapperController>());
        if (player.PlayerId == Runner.LocalPlayer.PlayerId)
        {
            CharactersController.GetInstance().SetLocalPlayer(playerObject.GetComponent<PlayerWrapperController>());
        }
    }
    
    private void SpawnEnemyAiPlayer(Character character)
    {
        var playerObject = Runner.Spawn(character.GetCharacterType() == CharacterType.MONSTER ? _monsterWrapper : _hunterWrapper, Vector3.zero, Quaternion.Euler(0, 0, 0),
            -1, ((runner, o) => BeforePlayerSpawn(-1, o, character)));
        CharactersController.GetInstance().RPC_AddPlayer(playerObject.GetComponent<PlayerWrapperController>());
    }

    private void BeforePlayerSpawn(PlayerRef player, NetworkObject charObject, Character character)
    {
        charObject.GetComponent<PlayerWrapperController>().InitNetworkState(player, character);
    }

    public bool IsLocalPlayerMonster()
    {
        return ProgressManager.Instance.GetCurrentTeamIsMonster();
    }

    public static GameController GetInstance()
    {
        return _instance;
    }

    public GameTimeController GetGameTimeController()
    {
        return _gameTimeController;
    }
}